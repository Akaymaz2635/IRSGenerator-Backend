using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FluentAssertions;
using IRSGenerator.Core.Services;
using Xunit;

namespace IRSGenerator.Tests.Services;

/// <summary>
/// WP-11 — WordOpSheetParser servis birim testleri.
/// .docx op-sheet parse işlevini in-memory OpenXML dökümanlarıyla doğrular.
/// </summary>
public class WordOpSheetParserTests
{
    private readonly WordOpSheetParser _parser = new();

    // ─── YARDIMCI METOTLAR ────────────────────────────────────────────────────────

    /// <summary>
    /// Belirtilen başlık ve satırlarla tek tablolu bir .docx dökümanı oluşturur.
    /// </summary>
    private static Stream CreateDocxWithTable(string[] headers, string[][] rows)
    {
        var ms = new MemoryStream();
        using (var doc = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document, true))
        {
            var mainPart = doc.AddMainDocumentPart();
            mainPart.Document = new Document(new Body());

            var table = BuildTable(headers, rows);
            mainPart.Document.Body!.Append(table);
            mainPart.Document.Save();
        }
        ms.Position = 0;
        return ms;
    }

    /// <summary>
    /// İki tablo içeren .docx dökümanı oluşturur.
    /// </summary>
    private static Stream CreateDocxWithTwoTables(
        string[] headers1, string[][] rows1,
        string[] headers2, string[][] rows2)
    {
        var ms = new MemoryStream();
        using (var doc = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document, true))
        {
            var mainPart = doc.AddMainDocumentPart();
            mainPart.Document = new Document(new Body());

            mainPart.Document.Body!.Append(BuildTable(headers1, rows1));
            mainPart.Document.Body!.Append(new Paragraph());   // tablolar arası boşluk
            mainPart.Document.Body!.Append(BuildTable(headers2, rows2));
            mainPart.Document.Save();
        }
        ms.Position = 0;
        return ms;
    }

    /// <summary>
    /// Tablo yok, boş Body içeren .docx dökümanı oluşturur.
    /// </summary>
    private static Stream CreateEmptyDocx()
    {
        var ms = new MemoryStream();
        using (var doc = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document, true))
        {
            var mainPart = doc.AddMainDocumentPart();
            mainPart.Document = new Document(new Body());
            mainPart.Document.Save();
        }
        ms.Position = 0;
        return ms;
    }

    /// <summary>
    /// ITEM NO / DIMENSION sütunları olmayan tabloya sahip .docx oluşturur.
    /// </summary>
    private static Stream CreateDocxWithNonMatchingTable()
    {
        return CreateDocxWithTable(
            ["NAME", "VALUE", "UNIT"],
            [["PartA", "100", "mm"], ["PartB", "200", "mm"]]);
    }

    private static Table BuildTable(string[] headers, string[][] rows)
    {
        var table = new Table();

        // Başlık satırı
        var headerRow = new TableRow();
        foreach (var h in headers)
            headerRow.Append(MakeCell(h));
        table.Append(headerRow);

        // Veri satırları
        foreach (var rowData in rows)
        {
            var row = new TableRow();
            foreach (var cell in rowData)
                row.Append(MakeCell(cell));
            table.Append(row);
        }

        return table;
    }

    private static TableCell MakeCell(string text)
        => new TableCell(new Paragraph(new Run(new Text(text))));

    // ─── TEST 1: Geçerli .docx — karakter sayısı beklenenle eşleşiyor ────────────

    [Fact]
    public void Parse_ValidDocx_ReturnsExpectedCharacterCount()
    {
        var stream = CreateDocxWithTable(
            ["ITEM NO", "DIMENSION", "BADGE"],
            [
                ["1", "10.0 ± 0.5", ""],
                ["2", "MAX 15.0",   "KC"],
                ["3", "MIN 5.0",    ""],
            ]);

        var result = _parser.Parse(stream);

        result.Should().HaveCount(3);
    }

    // ─── TEST 2: LOT satırı — badge="LOT" atandı ─────────────────────────────────

    [Fact]
    public void Parse_LotKeywordDimensions_AssignBadgeLot()
    {
        var stream = CreateDocxWithTable(
            ["ITEM NO", "DIMENSION"],
            [
                ["1", "VISUAL CHECK"],
                ["2", "COATING COLOR"],
                ["3", "MARKING REQUIRED"],
                ["4", "SURFACE FINISH"],
            ]);

        var result = _parser.Parse(stream);

        result.Should().HaveCount(4);
        result.Should().OnlyContain(c => c.Badge == "LOT",
            "LOT anahtar kelimesi içeren tüm boyutlar 'LOT' badge almalıdır");
    }

    [Fact]
    public void Parse_MixedDimensions_OnlyLotRowsGetBadgeLot()
    {
        var stream = CreateDocxWithTable(
            ["ITEM NO", "DIMENSION"],
            [
                ["1", "10.0 ± 0.5"],    // sayısal — LOT değil
                ["2", "VISUAL CHECK"],   // LOT
                ["3", "Ra 1.6"],         // sayısal — LOT değil
            ]);

        var result = _parser.Parse(stream);

        result.Should().HaveCount(3);
        result[0].Badge.Should().BeNull();
        result[1].Badge.Should().Be("LOT");
        result[2].Badge.Should().BeNull();
    }

    // ─── TEST 3: Atlanması gereken satırlar parse dışı ───────────────────────────

    [Fact]
    public void Parse_SkipItemNos_ExcludesMatchingRows()
    {
        var stream = CreateDocxWithTable(
            ["ITEM NO", "DIMENSION"],
            [
                ["ITEM NO",   "DIMENSION"],      // başlık tekrarı — atla
                ["INSPECTOR", "NAME"],            // atla
                ["RECORD",    "NUMBER"],          // atla
                ["**",        "SOME VALUE"],      // atla
                ["PAGE NO",   "1"],               // atla
                ["ITEM NO KC","1"],               // atla
                ["/0",        "SOMETHING"],       // atla
                ["1",         "10.0 ± 0.5"],     // tut
                ["2",         "MIN 5.0"],         // tut
            ]);

        var result = _parser.Parse(stream);

        result.Should().HaveCount(2);
        result[0].ItemNo.Should().Be("1");
        result[1].ItemNo.Should().Be("2");
    }

    [Fact]
    public void Parse_DimensionEndsWithInch_SkipsRow()
    {
        var stream = CreateDocxWithTable(
            ["ITEM NO", "DIMENSION"],
            [
                ["1", "0.5 INCH"],     // atla
                ["2", "1.5 INCHES"],   // atla
                ["3", "10.0 ± 0.5"],   // tut
            ]);

        var result = _parser.Parse(stream);

        result.Should().HaveCount(1);
        result[0].ItemNo.Should().Be("3");
    }

    // ─── TEST 4: Birden fazla tablo — hepsi parse edildi ─────────────────────────

    [Fact]
    public void Parse_TwoMatchingTables_ParsesBothTables()
    {
        var stream = CreateDocxWithTwoTables(
            ["ITEM NO", "DIMENSION"],
            [["1", "10.0 ± 0.5"], ["2", "MAX 5.0"]],
            ["ITEM NO", "DIMENSION", "REMARKS"],
            [["3", "MIN 3.0", "Check"], ["4", "Ra 1.6", ""]]);

        var result = _parser.Parse(stream);

        result.Should().HaveCount(4);
        result.Select(c => c.ItemNo).Should().BeEquivalentTo(["1", "2", "3", "4"]);
    }

    // ─── TEST 5: Boş dosya — boş liste döndü, exception yok ─────────────────────

    [Fact]
    public void Parse_EmptyDocx_ReturnsEmptyList_NoException()
    {
        var stream = CreateEmptyDocx();

        var act = () => _parser.Parse(stream);

        act.Should().NotThrow();
        act().Should().BeEmpty();
    }

    // ─── TEST 6: Tablo yok — boş liste döndü ────────────────────────────────────

    [Fact]
    public void Parse_NoMatchingTable_ReturnsEmptyList()
    {
        var stream = CreateDocxWithNonMatchingTable();

        var result = _parser.Parse(stream);

        result.Should().BeEmpty("ITEM NO ve DIMENSION başlıkları olmayan tablo parse edilmemelidir");
    }

    // ─── TEST 7: LimitCatcher entegrasyon — limits dolu ─────────────────────────

    [Fact]
    public void Parse_LimitCatcherIntegration_LimitsExtractedCorrectly()
    {
        var stream = CreateDocxWithTable(
            ["ITEM NO", "DIMENSION"],
            [
                ["1", "10.0 ± 0.5"],
                ["2", "MIN 5.0"],
                ["3", "MAX 15.0"],
                ["4", "5.0 / 15.0"],
                ["5", "Ra 1.6"],
            ]);

        var result = _parser.Parse(stream);

        result.Should().HaveCount(5);

        // ± tolerans
        result[0].LowerLimit.Should().BeApproximately(9.5, 0.0001);
        result[0].UpperLimit.Should().BeApproximately(10.5, 0.0001);

        // MIN
        result[1].LowerLimit.Should().BeApproximately(5.0, 0.0001);
        result[1].UpperLimit.Should().Be(double.MaxValue);

        // MAX
        result[2].LowerLimit.Should().Be(0);
        result[2].UpperLimit.Should().BeApproximately(15.0, 0.0001);

        // limit/limit
        result[3].LowerLimit.Should().BeApproximately(5.0, 0.0001);
        result[3].UpperLimit.Should().BeApproximately(15.0, 0.0001);

        // pürüzlülük
        result[4].LowerLimit.Should().Be(0);
        result[4].UpperLimit.Should().BeApproximately(1.6, 0.0001);
    }

    [Fact]
    public void Parse_UnparseableDimension_SetsZeroLimits()
    {
        // Sayısal limit çıkarılamayan boyut (LOT/diş) → LowerLimit=0, UpperLimit=0
        var stream = CreateDocxWithTable(
            ["ITEM NO", "DIMENSION"],
            [["1", "VISUAL CHECK"]]);

        var result = _parser.Parse(stream);

        result.Should().HaveCount(1);
        result[0].LowerLimit.Should().Be(0, "parse edilemeyen boyut için sınır 0 olmalıdır");
        result[0].UpperLimit.Should().Be(0, "parse edilemeyen boyut için sınır 0 olmalıdır");
    }

    // ─── TEST 8: InspectionResult her zaman "Unidentified" ────────────────────────

    [Fact]
    public void Parse_AllCharacters_HaveUnidentifiedInspectionResult()
    {
        var stream = CreateDocxWithTable(
            ["ITEM NO", "DIMENSION"],
            [["1", "10.0 ± 0.5"], ["2", "MAX 5.0"]]);

        var result = _parser.Parse(stream);

        result.Should().OnlyContain(c => c.InspectionResult == "Unidentified");
    }

    // ─── TEST 9: ItemNo boşlukları normalize ediliyor ─────────────────────────────

    [Fact]
    public void Parse_ItemNoWithSpaces_WhitespaceCollapsed()
    {
        var stream = CreateDocxWithTable(
            ["ITEM NO", "DIMENSION"],
            [["1  A", "10.0 ± 0.5"]]);

        var result = _parser.Parse(stream);

        result.Should().HaveCount(1);
        result[0].ItemNo.Should().Be("1A", "ItemNo içindeki boşluklar kaldırılmalıdır");
    }

    // ─── TEST 10: Ek sütunlar (BADGE, TOOLING, REMARKS, B/P ZONE, INSP LEVEL) ────

    [Fact]
    public void Parse_AllOptionalColumns_ExtractedCorrectly()
    {
        var stream = CreateDocxWithTable(
            ["ITEM NO", "DIMENSION", "BADGE", "TOOLING", "REMARKS", "B/P ZONE", "INSP LEVEL"],
            [["1", "10.0 ± 0.5", "KC", "Gauge-1", "Check carefully", "ZoneA", "I-3"]]);

        var result = _parser.Parse(stream);

        result.Should().HaveCount(1);
        var c = result[0];
        c.Badge.Should().Be("KC");
        c.Tooling.Should().Be("Gauge-1");
        c.Remarks.Should().Be("Check carefully");
        c.BPZone.Should().Be("ZoneA");
        c.InspectionLevel.Should().Be("I-3");
    }

    [Fact]
    public void Parse_EmptyOptionalColumns_ReturnNullNotEmptyString()
    {
        var stream = CreateDocxWithTable(
            ["ITEM NO", "DIMENSION", "BADGE", "TOOLING"],
            [["1", "10.0 ± 0.5", "", ""]]);

        var result = _parser.Parse(stream);

        result.Should().HaveCount(1);
        result[0].Badge.Should().BeNull("boş string null'a dönüştürülmelidir");
        result[0].Tooling.Should().BeNull("boş string null'a dönüştürülmelidir");
    }

    // ─── TEST 11: Satır tamamen boşsa atlanır ────────────────────────────────────

    [Fact]
    public void Parse_EmptyRows_Skipped()
    {
        var stream = CreateDocxWithTable(
            ["ITEM NO", "DIMENSION"],
            [
                ["",  ""],               // boş satır — atla
                ["1", "10.0 ± 0.5"],    // tut
                ["",  ""],               // boş satır — atla
            ]);

        var result = _parser.Parse(stream);

        result.Should().HaveCount(1);
    }
}
