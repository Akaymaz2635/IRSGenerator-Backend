using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace IRSGenerator.Core.Services
{
    public class OlcuSonucu
    {
        public double? Nominal { get; set; }
        public double? AltLimit { get; set; }
        public double? UstLimit { get; set; }
        public double? Tolerans { get; set; }
        public string? Tip { get; set; }
        public string? AltTip { get; set; }
        public string? Format { get; set; }
        public string? Sembol { get; set; }
        public List<string> Referanslar { get; set; } = new();
        public List<string> Ozellikler { get; set; } = new();
        public double? UnilateralDeger { get; set; }
    }

    public abstract class OlcuFormati
    {
        public abstract bool Eslestir(string olcu);
        public abstract OlcuSonucu Degerler();

        protected static double? ParseDouble(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
            s = s.Replace(',', '.');
            return double.TryParse(s,
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out var result) ? result : null;
        }
    }

    public class EsitToleransliOlcu : OlcuFormati
    {
        private static readonly Regex Desen = new Regex(
            @"(\d+(?:[.,]\d+)?)\s*(?:[±]|\+[-/])\s*(\d+(?:[.,]\d+)?)\s*(?:DEGREE|°|MM|mm)?$",
            RegexOptions.IgnoreCase);

        private double? _nominal;
        private double? _tolerans;

        public override bool Eslestir(string olcu)
        {
            var m = Desen.Match(olcu.Trim());
            if (m.Success)
            {
                _nominal  = ParseDouble(m.Groups[1].Value);
                _tolerans = ParseDouble(m.Groups[2].Value);
                return true;
            }
            return false;
        }

        public override OlcuSonucu Degerler() => new OlcuSonucu
        {
            Nominal  = _nominal,
            AltLimit = (_nominal.HasValue && _tolerans.HasValue) ? _nominal - _tolerans : null,
            UstLimit = (_nominal.HasValue && _tolerans.HasValue) ? _nominal + _tolerans : null,
            Format   = "toleranslı"
        };
    }

    public class ArtiEksiOlcu : OlcuFormati
    {
        private static readonly Regex Desen = new Regex(
            @"(\d+(?:[.,]\d+)?)\s*\+\s*(\d+(?:[.,]\d+)?)\s*[/\-]\s*-?\s*(\d+(?:[.,]\d+)?)",
            RegexOptions.IgnoreCase);

        private double? _nominal;
        private double? _ustTol;
        private double? _altTol;

        public override bool Eslestir(string olcu)
        {
            var m = Desen.Match(olcu);
            if (m.Success)
            {
                _nominal = ParseDouble(m.Groups[1].Value);
                _ustTol  = ParseDouble(m.Groups[2].Value);
                _altTol  = ParseDouble(m.Groups[3].Value);
                return true;
            }
            return false;
        }

        public override OlcuSonucu Degerler() => new OlcuSonucu
        {
            Nominal  = _nominal,
            AltLimit = (_nominal.HasValue && _altTol.HasValue) ? _nominal - _altTol : null,
            UstLimit = (_nominal.HasValue && _ustTol.HasValue) ? _nominal + _ustTol : null,
            Format   = "arti-eksi"
        };
    }

    public class ArtiEksiOlcu2 : OlcuFormati
    {
        private static readonly Regex Desen = new Regex(
            @"(\d+(?:[.,]\d+)?)\s*\+(\d+(?:[.,]\d+)?)\s*-(\d+(?:[.,]\d+)?)",
            RegexOptions.IgnoreCase);

        private double? _nominal;
        private double? _ustTol;
        private double? _altTol;

        public override bool Eslestir(string olcu)
        {
            var m = Desen.Match(olcu);
            if (m.Success)
            {
                _nominal = ParseDouble(m.Groups[1].Value);
                _ustTol  = ParseDouble(m.Groups[2].Value);
                _altTol  = ParseDouble(m.Groups[3].Value);
                return true;
            }
            return false;
        }

        public override OlcuSonucu Degerler() => new OlcuSonucu
        {
            Nominal  = _nominal,
            AltLimit = (_nominal.HasValue && _altTol.HasValue) ? _nominal - _altTol : null,
            UstLimit = (_nominal.HasValue && _ustTol.HasValue) ? _nominal + _ustTol : null,
            Format   = "arti-eksi"
        };
    }

    public class PuruzlulukOlcu : OlcuFormati
    {
        private static readonly List<Regex> Desenler = new List<Regex>
        {
            new Regex(@"(?:i)?Ra\s*(\d+(?:[.,]\d+)?)\s*Ra",          RegexOptions.IgnoreCase),
            new Regex(@"(?:i)?\s*(\d+(?:[.,]\d+)?)\s*Ra",            RegexOptions.IgnoreCase),
            new Regex(@"(?:i)?Rz\s*(\d+(?:[.,]\d+)?)\s*Ra",          RegexOptions.IgnoreCase),
            new Regex(@"Ra\s*(\d+(?:[.,]\d+)?)",                      RegexOptions.IgnoreCase),
            new Regex(@"(\d+(?:[.,]\d+)?)\s*Ra",                      RegexOptions.IgnoreCase),
            new Regex(@"SURFACE QUALITY\s*(\d+(?:[.,]\d+)?)",         RegexOptions.IgnoreCase),
            new Regex(@"(\d+(?:[.,]\d+)?)\s*SURFACE QUALITY",         RegexOptions.IgnoreCase),
        };

        private double? _deger;

        public override bool Eslestir(string olcu)
        {
            foreach (var desen in Desenler)
            {
                var m = desen.Match(olcu);
                if (m.Success) { _deger = ParseDouble(m.Groups[1].Value); return true; }
            }
            return false;
        }

        public override OlcuSonucu Degerler() => new OlcuSonucu
        {
            Nominal  = null,
            AltLimit = null,
            UstLimit = _deger,
            Format   = "Yüzey Pürüzlülüğü"
        };
    }

    public class MaxOlcu : OlcuFormati
    {
        private static readonly List<Regex> Desenler = new List<Regex>
        {
            new Regex(@"(?:i)?MAX\s*(\d+(?:[.,]\d+)?)",         RegexOptions.IgnoreCase),
            new Regex(@"(?:i)?R\s*(\d+(?:[.,]\d+)?)\s*MAX",    RegexOptions.IgnoreCase),
            new Regex(@"(?:i)?B\s*(\d+(?:[.,]\d+)?)\s*MAX",    RegexOptions.IgnoreCase),
            new Regex(@"(?:i)?(\d+(?:[.,]\d+)?)\s*MAX",        RegexOptions.IgnoreCase),
            new Regex(@"(?:i)?MAX\s*\w*\s*(\d+(?:[.,]\d+)?)",  RegexOptions.IgnoreCase),
        };

        private double? _deger;

        public override bool Eslestir(string olcu)
        {
            foreach (var desen in Desenler)
            {
                var m = desen.Match(olcu);
                if (m.Success) { _deger = ParseDouble(m.Groups[1].Value); return true; }
            }
            return false;
        }

        public override OlcuSonucu Degerler() => new OlcuSonucu
        {
            Nominal  = null,
            AltLimit = null,
            UstLimit = _deger,
            Format   = "maksimum"
        };
    }

    public class MinOlcu : OlcuFormati
    {
        private static readonly List<Regex> Desenler = new List<Regex>
        {
            new Regex(@"(?:i)?MIN\s*(\d+(?:[.,]\d+)?)",         RegexOptions.IgnoreCase),
            new Regex(@"(?:i)?R\s*(\d+(?:[.,]\d+)?)\s*MIN",    RegexOptions.IgnoreCase),
            new Regex(@"(?:i)?P\s*(\d+(?:[.,]\d+)?)\s*MIN",    RegexOptions.IgnoreCase),
            new Regex(@"(?:i)?(\d+(?:[.,]\d+)?)\s*MIN",        RegexOptions.IgnoreCase),
            new Regex(@"(?:i)?MIN\s*R\s*(\d+(?:[.,]\d+)?)",    RegexOptions.IgnoreCase),
            new Regex(@"(?:i)?R\s*MIN\s*(\d+(?:[.,]\d+)?)",    RegexOptions.IgnoreCase),
            new Regex(@"(?:i)?MIN\s*\w*\s*(\d+(?:[.,]\d+)?)",  RegexOptions.IgnoreCase),
            new Regex(@"(?:i)?R\s*(\d+(?:[.,]\d+)?)\s*Min",    RegexOptions.IgnoreCase),
        };

        private double? _deger;

        public override bool Eslestir(string olcu)
        {
            foreach (var desen in Desenler)
            {
                var m = desen.Match(olcu);
                if (m.Success) { _deger = ParseDouble(m.Groups[1].Value); return true; }
            }
            return false;
        }

        public override OlcuSonucu Degerler() => new OlcuSonucu
        {
            Nominal  = null,
            AltLimit = _deger,
            UstLimit = null,
            Format   = "minimum"
        };
    }

    public class MaxMinOlcu : OlcuFormati
    {
        private static readonly Regex Desen = new Regex(
            @"(?:i)?MIN\s*(\d+(?:[.,]\d+)?)\s*(?:\S*)\s*[-/|]\s*[-|]?\s*MAX\s*(\d+(?:[.,]\d+)?)",
            RegexOptions.IgnoreCase);

        private double? _minDeger;
        private double? _maxDeger;

        public override bool Eslestir(string olcu)
        {
            var m = Desen.Match(olcu);
            if (m.Success)
            {
                _minDeger = ParseDouble(m.Groups[1].Value);
                _maxDeger = ParseDouble(m.Groups[2].Value);
                return true;
            }
            return false;
        }

        public override OlcuSonucu Degerler() => new OlcuSonucu
        {
            Nominal  = null,
            AltLimit = _minDeger,
            UstLimit = _maxDeger,
            Format   = "minimum - maksimum"
        };
    }

    public class LimitToleransliOlcu : OlcuFormati
    {
        private static readonly Regex Desen = new Regex(
            @"(\d+(?:[.,]\d+)?)\s*[/|]\s*[-/|]?\s*(\d+(?:[.,]\d+)?)",
            RegexOptions.IgnoreCase);

        private double? _altLimit;
        private double? _ustLimit;

        public override bool Eslestir(string olcu)
        {
            var m = Desen.Match(olcu);
            if (m.Success)
            {
                _altLimit = ParseDouble(m.Groups[1].Value);
                _ustLimit = ParseDouble(m.Groups[2].Value);
                return true;
            }
            return false;
        }

        public override OlcuSonucu Degerler() => new OlcuSonucu
        {
            Nominal  = null,
            AltLimit = _altLimit,
            UstLimit = _ustLimit,
            Format   = "limit tolerans"
        };
    }

    public abstract class GeometrikTolerans : OlcuFormati
    {
        protected double? ToleransDegeri;
        protected List<string> Referanslar = new();
        protected string? Sembol;
        protected string? Tip;
        protected List<string> Ozellikler = new();
        protected string? AltTip;

        protected List<string> OzellikAyikla(string metin)
        {
            var ozellikler = new List<string>();
            var matches = Regex.Matches(metin, @"\(([MLPF])\)", RegexOptions.IgnoreCase);
            foreach (Match m in matches)
                ozellikler.Add(m.Groups[1].Value.ToUpper());
            return ozellikler;
        }

        protected List<string> ReferansAyikla(string metin)
        {
            var birlestik = Regex.Match(metin, @"([A-Z])-([A-Z])", RegexOptions.IgnoreCase);
            if (birlestik.Success)
                return new List<string> { birlestik.Groups[1].Value.ToUpper(), birlestik.Groups[2].Value.ToUpper() };

            var referanslar = new List<string>();
            var pipeMatches = Regex.Matches(metin, @"\|\s*([A-Z])\s*(?:[A-Z])?\s*(?:\([MLPF]\))?\s*(?:\w*[A-Z])?", RegexOptions.IgnoreCase);
            if (pipeMatches.Count > 0)
            {
                referanslar.AddRange(pipeMatches.Cast<Match>().Select(m => m.Groups[1].Value.ToUpper()));
            }
            else
            {
                var otherMatches = Regex.Matches(metin, @"\b([A-Z])\b(?!\s*\d)", RegexOptions.None);
                referanslar.AddRange(otherMatches.Cast<Match>()
                    .Select(m => m.Groups[1].Value.ToUpper())
                    .Where(r => !new[] { "M", "L", "P", "F" }.Contains(r)));
            }
            return new List<string>(new HashSet<string>(referanslar));
        }

        protected double? ToleransAyikla(string metin)
        {
            var pipeMatch = Regex.Match(metin, @"\|\s*(?:ø\s*)?(\d+(?:[.,]\d+)?)", RegexOptions.IgnoreCase);
            if (pipeMatch.Success) return ParseDouble(pipeMatch.Groups[1].Value);

            var diaMatch = Regex.Match(metin, @"ø\s*(\d+(?:[.,]\d+)?)", RegexOptions.IgnoreCase);
            if (diaMatch.Success) return ParseDouble(diaMatch.Groups[1].Value);

            var xMatch = Regex.Match(metin, @"X\s*\w*\s*(\d+(?:[.,]\d+)?)", RegexOptions.IgnoreCase);
            if (xMatch.Success) return ParseDouble(xMatch.Groups[1].Value);

            var allMatches = Regex.Matches(metin, @"(\d+(?:[.,]\d+)?)");
            if (allMatches.Count > 0) return ParseDouble(allMatches[allMatches.Count - 1].Groups[1].Value);

            return null;
        }

        protected string? SembolKontrol(string metin)
            => metin.Contains("ø") || metin.Contains("Ø") ? "ø" : null;
    }

    public class FormToleransi : GeometrikTolerans
    {
        private static readonly Dictionary<string, string> FormKeywords = new(StringComparer.OrdinalIgnoreCase)
        {
            { "STRAIGHTNESS", "Straightness" }, { "FLATNESS",     "Flatness"     },
            { "CIRCULARITY",  "Circularity"  }, { "CYLINDRICITY", "Cylindricity" },
            { "FLAT",         "Flatness"     }, { "FLT",          "Flatness"     },
            { "CIR",          "Circularity"  },
        };

        public FormToleransi() { Tip = "Form"; }

        public override bool Eslestir(string olcu)
        {
            var olcuUpper = olcu.ToUpper();
            var koseli = Regex.Match(olcu, @"\[\s*([^\[|\]]+?)\s*\|\s*([^\[|\]]+?)\s*\]", RegexOptions.IgnoreCase);
            if (koseli.Success)
            {
                var tk = koseli.Groups[1].Value.Trim().ToUpper();
                var dk = koseli.Groups[2].Value.Trim();
                foreach (var (kw, tip) in FormKeywords)
                    if (tk.Contains(kw)) { ToleransDegeri = ToleransAyikla(dk); if (ToleransDegeri.HasValue) { Sembol = SembolKontrol(olcu); Ozellikler = OzellikAyikla(olcu); AltTip = tip; return true; } }
            }
            foreach (var (kw, tip) in FormKeywords)
                if (olcuUpper.Contains(kw)) { ToleransDegeri = ToleransAyikla(olcu); if (ToleransDegeri.HasValue) { Sembol = SembolKontrol(olcu); Ozellikler = OzellikAyikla(olcu); AltTip = tip; return true; } }
            return false;
        }

        public override OlcuSonucu Degerler() => new OlcuSonucu { Tolerans = ToleransDegeri, Nominal = 0, UstLimit = ToleransDegeri, Tip = AltTip, AltTip = AltTip, Sembol = Sembol, Ozellikler = Ozellikler, Format = "geometrik" };
    }

    public class OryantasyonToleransi : GeometrikTolerans
    {
        private static readonly Dictionary<string, string> Keys = new(StringComparer.OrdinalIgnoreCase)
        {
            { "PERPENDICULARITY","Perpendicularity"},{"PERP.","Perpendicularity"},{"PERP","Perpendicularity"},
            { "PARALLELISM","Parallelism"},{"PAR","Parallelism"},{"ANGULARITY","Angularity"},{"ANG","Angularity"},
        };
        public OryantasyonToleransi() { Tip = "Oryantasyon"; }

        public override bool Eslestir(string olcu)
        {
            var up = olcu.ToUpper();
            var k = Regex.Match(olcu, @"\[\s*([^\[|\]]+?)\s*\|\s*([^\[|\]]+?)\s*(?:\|\s*(.+?))?\s*\]", RegexOptions.IgnoreCase);
            if (k.Success)
            {
                var tk = k.Groups[1].Value.Trim().ToUpper(); var dk = k.Groups[2].Value.Trim(); var rk = k.Groups[3].Success ? k.Groups[3].Value : "";
                foreach (var (kw, tip) in Keys) if (tk.Contains(kw)) { ToleransDegeri = ToleransAyikla(dk); if (ToleransDegeri.HasValue) { Sembol = SembolKontrol(olcu); Ozellikler = OzellikAyikla(olcu); Referanslar = ReferansAyikla(rk.Length > 0 ? rk : olcu); AltTip = tip; return true; } }
            }
            foreach (var (kw, tip) in Keys) if (up.Contains(kw)) { ToleransDegeri = ToleransAyikla(olcu); if (ToleransDegeri.HasValue) { Sembol = SembolKontrol(olcu); Ozellikler = OzellikAyikla(olcu); Referanslar = ReferansAyikla(olcu); AltTip = tip; return true; } }
            return false;
        }

        public override OlcuSonucu Degerler() => new OlcuSonucu { Tolerans = ToleransDegeri, Nominal = 0, UstLimit = ToleransDegeri, Tip = AltTip, AltTip = AltTip, Sembol = Sembol, Referanslar = Referanslar, Ozellikler = Ozellikler, Format = "geometrik" };
    }

    public class LokasyonToleransi : GeometrikTolerans
    {
        private static readonly Dictionary<string, string> Keys = new(StringComparer.OrdinalIgnoreCase)
        {
            {"TRUE POSITION","True Position"},{"TRUE POSITON","True Position"},{"T.P.","True Position"},{"TP","True Position"},
            {"POSITION","Position"},{"CONCENTRICITY","Concentricity"},{"SYMMETRY","Symmetry"},
        };
        public LokasyonToleransi() { Tip = "Lokasyon"; }

        public override bool Eslestir(string olcu)
        {
            var up = olcu.ToUpper();
            var k = Regex.Match(olcu, @"\[\s*([^\[|\]]+?)\s*\|\s*([^\[|\]]+?)\s*(?:\|\s*(.+?))?\s*\]", RegexOptions.IgnoreCase);
            if (k.Success)
            {
                var tk = k.Groups[1].Value.Trim().ToUpper(); var dk = k.Groups[2].Value.Trim(); var rk = k.Groups[3].Success ? k.Groups[3].Value : "";
                foreach (var (kw, tip) in Keys) if (tk.Contains(kw)) { ToleransDegeri = ToleransAyikla(dk); if (ToleransDegeri.HasValue) { Sembol = SembolKontrol(olcu); Ozellikler = OzellikAyikla(olcu); Referanslar = ReferansAyikla(rk.Length > 0 ? rk : olcu); AltTip = tip; return true; } }
            }
            foreach (var (kw, tip) in Keys) if (up.Contains(kw)) { ToleransDegeri = ToleransAyikla(olcu); if (ToleransDegeri.HasValue) { Sembol = SembolKontrol(olcu); Ozellikler = OzellikAyikla(olcu); Referanslar = ReferansAyikla(olcu); AltTip = tip; return true; } }
            return false;
        }

        public override OlcuSonucu Degerler() => new OlcuSonucu { Tolerans = ToleransDegeri, Nominal = 0, UstLimit = ToleransDegeri, Tip = AltTip, AltTip = AltTip, Sembol = Sembol, Referanslar = Referanslar, Ozellikler = Ozellikler, Format = "geometrik" };
    }

    public class ProfilToleransi : GeometrikTolerans
    {
        private static readonly Dictionary<string, string> Keys = new(StringComparer.OrdinalIgnoreCase)
        {
            {"PROFILE OF A LINE","Profile of a Line"},{"PROFILE OF A SURFACE","Profile of a Surface"},
            {"LP","Profile of a Line"},{"SP","Profile of a Surface"},
            {"SURFACE PROFILE","Profile of a Surface"},{"LINE PROFILE","Profile of a Line"},
        };
        private double? _unilateralDeger;
        public ProfilToleransi() { Tip = "Profil"; }

        public override bool Eslestir(string olcu)
        {
            var up = olcu.ToUpper();
            var uni = Regex.Match(olcu, @"(\d+(?:[.,]\d+)?)\s*\|\s*(\d+(?:[.,]\d+)?)", RegexOptions.IgnoreCase);
            if (uni.Success) { ToleransDegeri = ParseDouble(uni.Groups[1].Value); _unilateralDeger = ParseDouble(uni.Groups[2].Value); Ozellikler = OzellikAyikla(olcu); Referanslar = ReferansAyikla(olcu); AltTip = "Profile of a Line"; return true; }

            var k = Regex.Match(olcu, @"\[\s*([^\[|\]]+?)\s*\|\s*([^\[|\]]+?)\s*(?:\|\s*(.+?))?\s*\]", RegexOptions.IgnoreCase);
            if (k.Success)
            {
                var tk = k.Groups[1].Value.Trim().ToUpper(); var dk = k.Groups[2].Value.Trim(); var rk = k.Groups[3].Success ? k.Groups[3].Value : "";
                foreach (var (kw, tip) in Keys) if (tk.Contains(kw)) { ToleransDegeri = ToleransAyikla(dk); if (ToleransDegeri.HasValue) { Ozellikler = OzellikAyikla(olcu); Referanslar = ReferansAyikla(rk.Length > 0 ? rk : olcu); AltTip = tip; return true; } }
            }
            foreach (var (kw, tip) in Keys) if (up.Contains(kw)) { ToleransDegeri = ToleransAyikla(olcu); if (ToleransDegeri.HasValue) { Ozellikler = OzellikAyikla(olcu); Referanslar = ReferansAyikla(olcu); AltTip = tip; return true; } }
            return false;
        }

        public override OlcuSonucu Degerler()
        {
            var r = new OlcuSonucu { Tolerans = ToleransDegeri, Nominal = 0, UstLimit = ToleransDegeri, Tip = AltTip, AltTip = AltTip, Sembol = Sembol, Referanslar = Referanslar, Ozellikler = Ozellikler, Format = "geometrik" };
            if (_unilateralDeger.HasValue) r.UnilateralDeger = _unilateralDeger;
            return r;
        }
    }

    public class RunoutToleransi : GeometrikTolerans
    {
        private static readonly Dictionary<string, string> Keys = new(StringComparer.OrdinalIgnoreCase)
        {
            {"CIRCULAR RUNOUT","Circular Runout"},{"TOTAL RUNOUT","Total Runout"},{"RUNOUT","Runout"},
        };
        public RunoutToleransi() { Tip = "Runout"; }

        public override bool Eslestir(string olcu)
        {
            var up = olcu.ToUpper();
            var k = Regex.Match(olcu, @"\[\s*([^\[|\]]+?)\s*\|\s*([^\[|\]]+?)\s*(?:\|\s*(.+?))?\s*\]", RegexOptions.IgnoreCase);
            if (k.Success)
            {
                var tk = k.Groups[1].Value.Trim().ToUpper(); var dk = k.Groups[2].Value.Trim(); var rk = k.Groups[3].Success ? k.Groups[3].Value : "";
                foreach (var (kw, tip) in Keys) if (tk.Contains(kw)) { ToleransDegeri = ToleransAyikla(dk); if (ToleransDegeri.HasValue) { Referanslar = ReferansAyikla(rk.Length > 0 ? rk : olcu); AltTip = tip; return true; } }
            }
            foreach (var (kw, tip) in Keys) if (up.Contains(kw)) { ToleransDegeri = ToleransAyikla(olcu); if (ToleransDegeri.HasValue) { Referanslar = ReferansAyikla(olcu); AltTip = tip; return true; } }
            return false;
        }

        public override OlcuSonucu Degerler() => new OlcuSonucu { Tolerans = ToleransDegeri, Nominal = 0, UstLimit = ToleransDegeri, Tip = AltTip, AltTip = AltTip, Referanslar = Referanslar, Format = "geometrik" };
    }

    public class SembolTolerans : OlcuFormati
    {
        private static readonly Dictionary<string, string> SembolMap = new()
        {
            { "⊥", "Perpendicularity" }, { "○", "Position" }, { "↗", "Runout" },
        };
        private double? _toleransDegeri;
        private string? _tip;

        public override bool Eslestir(string olcu)
        {
            foreach (var (sembol, tip) in SembolMap)
            {
                var match = Regex.Match(olcu, @"(\d+(?:[.,]\d+)?)\s*" + Regex.Escape(sembol), RegexOptions.IgnoreCase);
                if (match.Success) { _toleransDegeri = ParseDouble(match.Groups[1].Value); _tip = tip; return true; }
            }
            return false;
        }

        public override OlcuSonucu Degerler() => new OlcuSonucu { Tolerans = _toleransDegeri, Nominal = 0, UstLimit = _toleransDegeri, Tip = _tip, Format = "sembol" };
    }

    public class IplikToleransi : OlcuFormati
    {
        private static readonly List<Regex> Desenler = new()
        {
            new Regex(@"\b(NPT|NPTF|NPS|UNJ|UNEF|UNC|UNF|UNS|UNR|BSPT|BSPP|ACME)\b",
                      RegexOptions.IgnoreCase),
            // Metric thread: M6, M10x1.5, M8-6H
            new Regex(@"\bM\d+(?:[.,]\d+)?(?:\s*[xX×]\s*\d+(?:[.,]\d+)?)?(?:\s*-\s*\d[A-Z]{0,2})?\b"),
            // Fractional UN threads: 1/4-20 UNC, 3/8-16 UNF
            new Regex(@"\b\d+(?:[/-]\d+)?\s*-\s*\d+\s*(?:UNC|UNF|UNEF|UNS|UNR|UNJ)\b",
                      RegexOptions.IgnoreCase),
        };

        public override bool Eslestir(string olcu)
        {
            foreach (var d in Desenler)
                if (d.IsMatch(olcu)) return true;
            return false;
        }

        public override OlcuSonucu Degerler() => new OlcuSonucu
        {
            Nominal = null, AltLimit = null, UstLimit = null, Format = "diş"
        };
    }

    /// <summary>
    /// Ana ölçü yakalayıcı.
    /// </summary>
    public class OlcuYakalayici
    {
        private readonly List<OlcuFormati> _formatTipleri;

        public OlcuYakalayici()
        {
            _formatTipleri = new List<OlcuFormati>
            {
                new IplikToleransi(),
                new FormToleransi(),
                new OryantasyonToleransi(),
                new LokasyonToleransi(),
                new ProfilToleransi(),
                new RunoutToleransi(),
                new SembolTolerans(),
                new EsitToleransliOlcu(),
                new ArtiEksiOlcu(),
                new ArtiEksiOlcu2(),
                new MaxMinOlcu(),
                new LimitToleransliOlcu(),
                new MinOlcu(),
                new MaxOlcu(),
                new PuruzlulukOlcu(),
            };
        }

        public OlcuSonucu? Isle(string olcu)
        {
            olcu = olcu.Replace(",", ".");
            // Strip leading prefixes: CF DIA, DIA, TR, CF, Ø, 4x, -6X, etc.
            olcu = Regex.Replace(olcu,
                @"^(?:CF\s+DIA|DIA|TR|CF|Ø|ø|[0-9]+\s*[xX]|-?[0-9]+\s*[xX])\s*",
                "", RegexOptions.IgnoreCase).Trim();
            foreach (var format in _formatTipleri)
            {
                if (format.Eslestir(olcu))
                    return format.Degerler();
            }
            return null;
        }
    }
}
