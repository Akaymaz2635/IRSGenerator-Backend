using FluentAssertions;
using IRSGenerator.Core.Services;
using Xunit;

namespace IRSGenerator.Tests.Services;

/// <summary>
/// WP-10 — OlcuParser / LimitCatcher servis birim testleri.
/// LimitCatcherService.CatchMeasurement() ve OlcuYakalayici.Isle() için kapsamlı testler.
/// </summary>
public class OlcuParserTests
{
    // ─── CatchMeasurement — TEMEL FORMAT TESTLERİ ───────────────────────────────

    [Fact]
    public void CatchMeasurement_EsitToleransli_ReturnsCorrectLimits()
    {
        // 10.0 ± 0.5  →  [9.5, 10.5]
        var result = LimitCatcherService.CatchMeasurement("10.0 ± 0.5");

        result.Should().HaveCount(2);
        result[0].Should().BeApproximately(9.5, 0.0001);
        result[1].Should().BeApproximately(10.5, 0.0001);
    }

    [Fact]
    public void CatchMeasurement_ArtiEksi_ReturnsCorrectLimits()
    {
        // 10.0 +0.3/-0.2  →  [9.8, 10.3]
        var result = LimitCatcherService.CatchMeasurement("10.0 +0.3/-0.2");

        result.Should().HaveCount(2);
        result[0].Should().BeApproximately(9.8, 0.0001);
        result[1].Should().BeApproximately(10.3, 0.0001);
    }

    [Fact]
    public void CatchMeasurement_MinFormat_ReturnsLowerWithMaxValue()
    {
        // MIN 5.0  →  [5.0, double.MaxValue]
        var result = LimitCatcherService.CatchMeasurement("MIN 5.0");

        result.Should().HaveCount(2);
        result[0].Should().BeApproximately(5.0, 0.0001);
        result[1].Should().Be(double.MaxValue);
    }

    [Fact]
    public void CatchMeasurement_MaxFormat_ReturnsZeroToUpper()
    {
        // MAX 15.0  →  [0, 15.0]
        var result = LimitCatcherService.CatchMeasurement("MAX 15.0");

        result.Should().HaveCount(2);
        result[0].Should().Be(0);
        result[1].Should().BeApproximately(15.0, 0.0001);
    }

    [Fact]
    public void CatchMeasurement_LimitTolerans_ReturnsBothLimits()
    {
        // 5.0 / 15.0  →  [5.0, 15.0]
        var result = LimitCatcherService.CatchMeasurement("5.0 / 15.0");

        result.Should().HaveCount(2);
        result[0].Should().BeApproximately(5.0, 0.0001);
        result[1].Should().BeApproximately(15.0, 0.0001);
    }

    [Fact]
    public void CatchMeasurement_GeometrikTruePosition_ReturnsZeroToTolerance()
    {
        // Geometrik tolerans bracket notasyonu ile
        // [TRUE POSITION | 0.1 | A B]  →  [0, 0.1]
        var result = LimitCatcherService.CatchMeasurement("[TRUE POSITION | 0.1 | A B]");

        result.Should().HaveCount(2);
        result[0].Should().Be(0);
        result[1].Should().BeApproximately(0.1, 0.0001);
    }

    [Fact]
    public void CatchMeasurement_PuruzlulukRa_ReturnsZeroToValue()
    {
        // Ra 1.6  →  [0, 1.6]  (Yüzey Pürüzlülüğü formatı)
        var result = LimitCatcherService.CatchMeasurement("Ra 1.6");

        result.Should().HaveCount(2);
        result[0].Should().Be(0);
        result[1].Should().BeApproximately(1.6, 0.0001);
    }

    // ─── CatchMeasurement — BOŞ / GEÇERSİZ GİRİŞLER ────────────────────────────

    [Fact]
    public void CatchMeasurement_NullInput_ReturnsEmpty()
    {
        var result = LimitCatcherService.CatchMeasurement(null);
        result.Should().BeEmpty();
    }

    [Fact]
    public void CatchMeasurement_EmptyString_ReturnsEmpty()
    {
        var result = LimitCatcherService.CatchMeasurement("");
        result.Should().BeEmpty();
    }

    [Fact]
    public void CatchMeasurement_WhitespaceOnly_ReturnsEmpty()
    {
        var result = LimitCatcherService.CatchMeasurement("   ");
        result.Should().BeEmpty();
    }

    [Fact]
    public void CatchMeasurement_VisualKeyword_ReturnsEmpty()
    {
        // LOT / kategorik boyutlar sayısal parse edilemez
        var result = LimitCatcherService.CatchMeasurement("VISUAL");
        result.Should().BeEmpty();
    }

    [Fact]
    public void CatchMeasurement_ArbitraryText_ReturnsEmpty()
    {
        var result = LimitCatcherService.CatchMeasurement("LOREM IPSUM CHECK");
        result.Should().BeEmpty();
    }

    // ─── CatchMeasurement — DİŞ TOLERANSI ──────────────────────────────────────

    [Fact]
    public void CatchMeasurement_MetricThread_ReturnsEmpty()
    {
        // IplikToleransi eşleşir ancak limit değeri üretmez (diş formatı)
        var result = LimitCatcherService.CatchMeasurement("M10x1.5");
        result.Should().BeEmpty("diş toleransında sayısal sınır tanımlı değildir");
    }

    [Fact]
    public void CatchMeasurement_UNCThread_ReturnsEmpty()
    {
        var result = LimitCatcherService.CatchMeasurement("1/4-20 UNC");
        result.Should().BeEmpty("diş toleransında sayısal sınır tanımlı değildir");
    }

    // ─── CatchMeasurement — GEOMETRİK PREFIX SEMBOLLERİ ─────────────────────────

    [Theory]
    [InlineData("⊕ 0.1 A B",  0.1)]   // True Position
    [InlineData("⊕ 0.05",     0.05)]
    [InlineData("⊘ 0.02 A",   0.02)]   // Concentricity
    public void CatchMeasurement_PrefixSymbol_ReturnsZeroToTolerance(string input, double expectedUpper)
    {
        var result = LimitCatcherService.CatchMeasurement(input);

        result.Should().HaveCount(2, $"'{input}' prefix sembolü limit üretmelidir");
        result[0].Should().Be(0);
        result[1].Should().BeApproximately(expectedUpper, 0.0001);
    }

    // ─── CatchMeasurement — ÇOK SAYIDA EŞİT TOLERANS DEĞERİ ────────────────────

    [Theory]
    [InlineData("10 ± 1",      9.0,   11.0)]
    [InlineData("25.5 ± 0.05", 25.45, 25.55)]
    [InlineData("0.5 ± 0.5",   0.0,   1.0)]
    [InlineData("100 ± 2.5",   97.5,  102.5)]
    public void CatchMeasurement_EsitToleransli_MultipleValues(
        string input, double expectedLower, double expectedUpper)
    {
        var result = LimitCatcherService.CatchMeasurement(input);

        result.Should().HaveCount(2);
        result[0].Should().BeApproximately(expectedLower, 0.0001);
        result[1].Should().BeApproximately(expectedUpper, 0.0001);
    }

    // ─── CatchMeasurement — MIN / MAX VARYANTLARI ───────────────────────────────

    [Theory]
    [InlineData("MAX R 12.5",          0,    12.5)]
    [InlineData("R 3.5 MAX",           0,    3.5)]
    [InlineData("MIN 0.5 / MAX 2.5",   0.5,  2.5)]
    public void CatchMeasurement_MinMaxVariants_ReturnCorrectBounds(
        string input, double expectedLower, double expectedUpper)
    {
        var result = LimitCatcherService.CatchMeasurement(input);

        result.Should().HaveCount(2);
        result[0].Should().BeApproximately(expectedLower, 0.0001);
        result[1].Should().BeApproximately(expectedUpper, 0.0001);
    }

    [Theory]
    [InlineData("MIN R 3.0")]
    [InlineData("MIN 5.0")]
    public void CatchMeasurement_MinOnly_ReturnsDoubleMaxValue(string input)
    {
        var result = LimitCatcherService.CatchMeasurement(input);

        result.Should().HaveCount(2);
        result[1].Should().Be(double.MaxValue);
    }

    // ─── CatchMeasurement — GEOMETRİK FORMAT TESTLERİ ───────────────────────────

    [Theory]
    [InlineData("FLATNESS 0.05")]
    [InlineData("[FLATNESS | 0.05]")]
    [InlineData("PERPENDICULARITY 0.1 A")]
    [InlineData("[PARALLELISM | 0.05 | A]")]
    [InlineData("[TRUE POSITION | ø 0.2 | A B C]")]
    [InlineData("RUNOUT 0.05 A")]
    public void CatchMeasurement_GeometrikFormat_ReturnsZeroToTolerance(string input)
    {
        var result = LimitCatcherService.CatchMeasurement(input);

        result.Should().HaveCount(2, $"geometrik tolerans '{input}' limit üretmelidir");
        result[0].Should().Be(0);
        result[1].Should().BeGreaterThan(0);
    }

    [Theory]
    [InlineData("Ra 1.6")]
    [InlineData("0.8 Ra")]
    [InlineData("3.2 Ra")]
    public void CatchMeasurement_PuruzlulukFormatlari_ReturnsZeroToValue(string input)
    {
        var result = LimitCatcherService.CatchMeasurement(input);

        result.Should().HaveCount(2, $"pürüzlülük ölçüsü '{input}' limit üretmelidir");
        result[0].Should().Be(0);
        result[1].Should().BeGreaterThan(0);
    }

    [Theory]
    [InlineData("Rz 6.3")]
    [InlineData("Rz 12.5")]
    [InlineData("3.2 Rz")]
    public void CatchMeasurement_RzStandalone_ReturnsZeroToValue(string input)
    {
        var result = LimitCatcherService.CatchMeasurement(input);

        result.Should().HaveCount(2, $"'Rz N' formatı artık PuruzlulukOlcu'da desteklenmektedir");
        result[0].Should().Be(0);
        result[1].Should().BeGreaterThan(0);
    }

    // ─── CatchMeasurement — VIRGÜL ONDALIK AYIRICI ──────────────────────────────

    [Fact]
    public void CatchMeasurement_CommaDecimalSeparator_HandledCorrectly()
    {
        // Türkçe ondalık: virgül → nokta dönüşümü bekleniyor
        var result = LimitCatcherService.CatchMeasurement("10,0 ± 0,5");

        result.Should().HaveCount(2);
        result[0].Should().BeApproximately(9.5, 0.0001);
        result[1].Should().BeApproximately(10.5, 0.0001);
    }

    // ─── CatchMeasurement — PREFIX SOYMA TESTLERİ ───────────────────────────────

    [Theory]
    [InlineData("DIA 10.0 ± 0.5",    9.5,  10.5)]
    [InlineData("Ø 10.0 ± 0.5",      9.5,  10.5)]
    [InlineData("CF DIA 10.0 ± 0.5", 9.5,  10.5)]
    public void CatchMeasurement_WithPrefix_StripsPrefixAndParses(
        string input, double expectedLower, double expectedUpper)
    {
        var result = LimitCatcherService.CatchMeasurement(input);

        result.Should().HaveCount(2);
        result[0].Should().BeApproximately(expectedLower, 0.0001);
        result[1].Should().BeApproximately(expectedUpper, 0.0001);
    }

    // ─── CatchMeasurement — YUVARLAMA ────────────────────────────────────────────

    [Fact]
    public void CatchMeasurement_Result_IsRoundedTo4DecimalPlaces()
    {
        // 1.23456789 ± 0.00001  →  sınırlar 4 ondalıkla yuvarlanmış olmalı
        var result = LimitCatcherService.CatchMeasurement("1.23456789 ± 0.00001");

        result.Should().HaveCount(2);
        result[0].Should().Be(Math.Round(result[0], 4));
        result[1].Should().Be(Math.Round(result[1], 4));
    }

    // ─── OlcuYakalayici.Isle() — DOĞRUDAN TEST ───────────────────────────────────

    [Fact]
    public void OlcuYakalayici_Isle_ReturnsNullForUnknownFormat()
    {
        var parser = new OlcuYakalayici();
        var result = parser.Isle("LOREM IPSUM");
        result.Should().BeNull();
    }

    [Fact]
    public void OlcuYakalayici_Isle_EsitToleransli_CorrectFields()
    {
        var parser = new OlcuYakalayici();
        var result = parser.Isle("10.0 ± 0.5");

        result.Should().NotBeNull();
        result!.Format.Should().Be("toleranslı");
        result.Nominal.Should().BeApproximately(10.0, 0.0001);
        result.AltLimit.Should().BeApproximately(9.5, 0.0001);
        result.UstLimit.Should().BeApproximately(10.5, 0.0001);
    }

    [Fact]
    public void OlcuYakalayici_Isle_ArtiEksi_CorrectFields()
    {
        var parser = new OlcuYakalayici();
        var result = parser.Isle("10.0 +0.3/-0.2");

        result.Should().NotBeNull();
        result!.Format.Should().Be("arti-eksi");
        result.AltLimit.Should().BeApproximately(9.8, 0.0001);
        result.UstLimit.Should().BeApproximately(10.3, 0.0001);
    }

    [Fact]
    public void OlcuYakalayici_Isle_Geometrik_FormatIsGeometrik()
    {
        var parser = new OlcuYakalayici();
        var result = parser.Isle("FLATNESS 0.05");

        result.Should().NotBeNull();
        result!.Format.Should().Be("geometrik");
        result.UstLimit.Should().BeApproximately(0.05, 0.0001);
    }

    [Fact]
    public void OlcuYakalayici_Isle_Puruzluluk_FormatIsPuruzluluk()
    {
        var parser = new OlcuYakalayici();
        var result = parser.Isle("Ra 1.6");

        result.Should().NotBeNull();
        result!.Format.Should().Be("Yüzey Pürüzlülüğü");
        result.UstLimit.Should().BeApproximately(1.6, 0.0001);
    }

    [Fact]
    public void OlcuYakalayici_Isle_Thread_FormatIsDis()
    {
        var parser = new OlcuYakalayici();
        var result = parser.Isle("M10x1.5");

        result.Should().NotBeNull();
        result!.Format.Should().Be("diş");
        result.AltLimit.Should().BeNull();
        result.UstLimit.Should().BeNull();
    }

    [Fact]
    public void OlcuYakalayici_Isle_StripsDiaPrefix_ThenParses()
    {
        var parser = new OlcuYakalayici();
        var result = parser.Isle("DIA 10.0 ± 0.5");

        result.Should().NotBeNull();
        result!.Nominal.Should().BeApproximately(10.0, 0.0001);
    }

    [Fact]
    public void OlcuYakalayici_Isle_MinFormat_CorrectFields()
    {
        var parser = new OlcuYakalayici();
        var result = parser.Isle("MIN 5.0");

        result.Should().NotBeNull();
        result!.Format.Should().Be("minimum");
        result.AltLimit.Should().BeApproximately(5.0, 0.0001);
        result.UstLimit.Should().BeNull();
    }

    [Fact]
    public void OlcuYakalayici_Isle_MaxFormat_CorrectFields()
    {
        var parser = new OlcuYakalayici();
        var result = parser.Isle("MAX 15.0");

        result.Should().NotBeNull();
        result!.Format.Should().Be("maksimum");
        result.UstLimit.Should().BeApproximately(15.0, 0.0001);
        result.AltLimit.Should().BeNull();
    }
}
