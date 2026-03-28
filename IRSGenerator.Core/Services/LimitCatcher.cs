using System.Text.RegularExpressions;

namespace IRSGenerator.Core.Services;

/// <summary>
/// Tolerans tipini otomatik tanıyarak lower/upper limit döndürür.
/// Dönüş: double[2] → [lowerLimit, upperLimit]
/// Boş dizi → tanınamadı (WrongFormat)
/// </summary>
public static class LimitCatcher
{
    public static double[] CatchMeasurement(string? measurement)
    {
        if (string.IsNullOrWhiteSpace(measurement))
            return Array.Empty<double>();

        string text = measurement.Replace(',', '.').ToUpper();

        if (FormToleranceControl(text, out var r))        return r;
        if (OrientationToleranceControl(text, out r))     return r;
        if (LocationToleranceControl(text, out r))        return r;
        if (ProfileToleranceControl(text, out r))         return r;
        if (RunoutToleranceControl(text, out r))          return r;
        if (SymbolToleranceControl(text, out r))          return r;
        if (EqualToleranceControl(text, out r))           return r;
        if (PlusMinusControl(text, out r))                return r;
        if (MaxControl(text, out r))                      return r;
        if (MinControl(text, out r))                      return r;

        return Array.Empty<double>();
    }

    // ── 1. Form Toleransları (STRAIGHTNESS, FLATNESS, CIRCULARITY, CYLINDRICITY) ──
    private static bool FormToleranceControl(string text, out double[] result)
    {
        string[] keywords = ["STRAIGHTNESS", "FLATNESS", "CIRCULARITY", "CYLINDRICITY"];
        foreach (var kw in keywords)
        {
            if (!text.Contains(kw)) continue;
            var m = Regex.Match(text, @"([\d]+\.?[\d]*)", RegexOptions.None, TimeSpan.FromMilliseconds(200));
            if (m.Success && double.TryParse(m.Groups[1].Value, out var val))
            {
                result = [0, Math.Round(val, 2)];
                return true;
            }
        }
        // Köşeli parantez formatı: [symbol | value]
        var bracket = Regex.Match(text, @"\[\s*.*?\s*\|\s*([\d]+\.?[\d]*)\s*\]", RegexOptions.None, TimeSpan.FromMilliseconds(200));
        if (bracket.Success && double.TryParse(bracket.Groups[1].Value, out var bVal))
        {
            result = [0, Math.Round(bVal, 2)];
            return true;
        }
        result = Array.Empty<double>();
        return false;
    }

    // ── 2. Oryantasyon Toleransları (PERPENDICULARITY, PARALLELISM, ANGULARITY) ──
    private static bool OrientationToleranceControl(string text, out double[] result)
    {
        string[] keywords = ["PERPENDICULARITY", "PARALLELISM", "ANGULARITY", "ANG"];
        foreach (var kw in keywords)
        {
            if (!text.Contains(kw)) continue;
            var m = Regex.Match(text, @"([\d]+\.?[\d]*)", RegexOptions.None, TimeSpan.FromMilliseconds(200));
            if (m.Success && double.TryParse(m.Groups[1].Value, out var val))
            {
                result = [0, Math.Round(val, 2)];
                return true;
            }
        }
        if (text.Contains("//")) // Parallelism sembolü
        {
            var m = Regex.Match(text, @"([\d]+\.?[\d]*)", RegexOptions.None, TimeSpan.FromMilliseconds(200));
            if (m.Success && double.TryParse(m.Groups[1].Value, out var val))
            {
                result = [0, Math.Round(val, 2)];
                return true;
            }
        }
        result = Array.Empty<double>();
        return false;
    }

    // ── 3. Lokasyon Toleransları (POSITION, TRUE POSITION, CONCENTRICITY, SYMMETRY) ──
    private static bool LocationToleranceControl(string text, out double[] result)
    {
        string[] keywords = ["POSITION", "TRUE_POSITION", "TRUE POSITION", "TP", "CONCENTRICITY", "SYMMETRY"];
        foreach (var kw in keywords)
        {
            if (!text.Contains(kw)) continue;
            var m = Regex.Match(text, @"([\d]+\.?[\d]*)", RegexOptions.None, TimeSpan.FromMilliseconds(200));
            if (m.Success && double.TryParse(m.Groups[1].Value, out var val))
            {
                result = [0, Math.Round(val, 2)];
                return true;
            }
        }
        result = Array.Empty<double>();
        return false;
    }

    // ── 4. Profil Toleransları ──
    private static bool ProfileToleranceControl(string text, out double[] result)
    {
        // Unilateral: ⌓ value1 value2
        var uni = Regex.Match(text, @"⌓\s*([\d]+\.?[\d]*)", RegexOptions.None, TimeSpan.FromMilliseconds(200));
        if (uni.Success && double.TryParse(uni.Groups[1].Value, out var uniVal))
        {
            result = [0, Math.Round(uniVal, 2)];
            return true;
        }
        string[] keywords = ["PROFILE OF A LINE", "PROFILE OF A SURFACE", " LP ", " SP "];
        foreach (var kw in keywords)
        {
            if (!text.Contains(kw)) continue;
            var m = Regex.Match(text, @"([\d]+\.?[\d]*)", RegexOptions.None, TimeSpan.FromMilliseconds(200));
            if (m.Success && double.TryParse(m.Groups[1].Value, out var val))
            {
                result = [0, Math.Round(val, 2)];
                return true;
            }
        }
        result = Array.Empty<double>();
        return false;
    }

    // ── 5. Runout Toleransları ──
    private static bool RunoutToleranceControl(string text, out double[] result)
    {
        string[] keywords = ["CIRCULAR RUNOUT", "TOTAL RUNOUT", "RUNOUT"];
        foreach (var kw in keywords)
        {
            if (!text.Contains(kw)) continue;
            var m = Regex.Match(text, @"([\d]+\.?[\d]*)", RegexOptions.None, TimeSpan.FromMilliseconds(200));
            if (m.Success && double.TryParse(m.Groups[1].Value, out var val))
            {
                result = [0, Math.Round(val, 2)];
                return true;
            }
        }
        result = Array.Empty<double>();
        return false;
    }

    // ── 6. Sembol Toleransları ──
    private static bool SymbolToleranceControl(string text, out double[] result)
    {
        string[] symbols = ["⏤", "⏥", "⌖", "⌒", "↗"];
        foreach (var sym in symbols)
        {
            if (!text.Contains(sym)) continue;
            var m = Regex.Match(text, @"([\d]+\.?[\d]*)", RegexOptions.None, TimeSpan.FromMilliseconds(200));
            if (m.Success && double.TryParse(m.Groups[1].Value, out var val))
            {
                result = [0, Math.Round(val, 2)];
                return true;
            }
        }
        result = Array.Empty<double>();
        return false;
    }

    // ── 7. Eşit Tolerans (nominal ± tolerance) ──
    private static bool EqualToleranceControl(string text, out double[] result)
    {
        var m = Regex.Match(text, @"([\d]+\.?[\d]*)\s*[±]\s*([\d]+\.?[\d]*)",
            RegexOptions.None, TimeSpan.FromMilliseconds(200));
        if (m.Success
            && double.TryParse(m.Groups[1].Value, out var nominal)
            && double.TryParse(m.Groups[2].Value, out var tol))
        {
            result = [Math.Round(nominal - tol, 2), Math.Round(nominal + tol, 2)];
            return true;
        }
        result = Array.Empty<double>();
        return false;
    }

    // ── 8. Artı-Eksi (nominal +upper / -lower) ──
    private static bool PlusMinusControl(string text, out double[] result)
    {
        var m = Regex.Match(text,
            @"([\d]+\.?[\d]*)\s*\+\s*([\d]+\.?[\d]*)\s*/\s*-\s*([\d]+\.?[\d]*)",
            RegexOptions.None, TimeSpan.FromMilliseconds(200));
        if (m.Success
            && double.TryParse(m.Groups[1].Value, out var nominal)
            && double.TryParse(m.Groups[2].Value, out var upper)
            && double.TryParse(m.Groups[3].Value, out var lower))
        {
            result = [Math.Round(nominal - lower, 2), Math.Round(nominal + upper, 2)];
            return true;
        }
        result = Array.Empty<double>();
        return false;
    }

    // ── 9. MAX ──
    private static bool MaxControl(string text, out double[] result)
    {
        string[] patterns = [@"MAX\s*([\d]+\.?[\d]*)", @"([\d]+\.?[\d]*)\s*MAX"];
        foreach (var pat in patterns)
        {
            var m = Regex.Match(text, pat, RegexOptions.None, TimeSpan.FromMilliseconds(200));
            if (m.Success && double.TryParse(m.Groups[1].Value, out var val))
            {
                result = [0, Math.Round(val, 2)];
                return true;
            }
        }
        result = Array.Empty<double>();
        return false;
    }

    // ── 10. MIN ──
    private static bool MinControl(string text, out double[] result)
    {
        string[] patterns = [@"MIN\s*([\d]+\.?[\d]*)", @"([\d]+\.?[\d]*)\s*MIN"];
        foreach (var pat in patterns)
        {
            var m = Regex.Match(text, pat, RegexOptions.None, TimeSpan.FromMilliseconds(200));
            if (m.Success && double.TryParse(m.Groups[1].Value, out var val))
            {
                result = [Math.Round(val, 2), double.MaxValue];
                return true;
            }
        }
        result = Array.Empty<double>();
        return false;
    }
}
