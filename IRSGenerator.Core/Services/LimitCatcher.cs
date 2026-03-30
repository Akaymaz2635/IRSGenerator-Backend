namespace IRSGenerator.Core.Services;

/// <summary>
/// Uses OlcuYakalayici to extract lower/upper limits from a dimension string.
/// Returns double[2] = [lower, upper], or empty on failure.
/// </summary>
public static class LimitCatcherService
{
    private static readonly OlcuYakalayici _parser = new();

    public static double[] CatchMeasurement(string? measurement)
    {
        if (string.IsNullOrWhiteSpace(measurement))
            return Array.Empty<double>();

        var result = _parser.Isle(measurement);
        if (result is null) return Array.Empty<double>();

        // Geometric / profile / surface: [0, tolerance]
        if (result.Format == "geometrik" || result.Format == "Yüzey Pürüzlülüğü")
        {
            if (result.UstLimit.HasValue)
                return [0, Math.Round(result.UstLimit.Value, 4)];
            return Array.Empty<double>();
        }

        // MIN format: lower = AltLimit, upper = +infinity
        if (result.Format == "minimum" && result.AltLimit.HasValue)
            return [Math.Round(result.AltLimit.Value, 4), double.MaxValue];

        // MAX format: lower = 0, upper = UstLimit
        if (result.Format == "maksimum" && result.UstLimit.HasValue)
            return [0, Math.Round(result.UstLimit.Value, 4)];

        // Both limits defined
        if (result.AltLimit.HasValue && result.UstLimit.HasValue)
            return [Math.Round(result.AltLimit.Value, 4), Math.Round(result.UstLimit.Value, 4)];

        // Only upper
        if (result.UstLimit.HasValue)
            return [0, Math.Round(result.UstLimit.Value, 4)];

        // Only lower
        if (result.AltLimit.HasValue)
            return [Math.Round(result.AltLimit.Value, 4), double.MaxValue];

        return Array.Empty<double>();
    }
}
