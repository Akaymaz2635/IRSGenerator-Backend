using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IRSGenerator.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUpdateNoteToNumericPartResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UpdateNote",
                table: "NumericPartResults",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 6L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 7L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 8L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 9L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 10L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 11L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 12L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 13L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 14L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 15L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 16L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 17L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 18L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 19L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 20L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 21L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 22L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 23L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 24L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 25L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 26L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 27L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 28L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 29L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 30L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 31L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 32L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 33L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 34L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 35L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 36L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 37L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 38L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 39L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 40L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 41L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 42L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 43L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 44L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 45L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 46L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 47L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 48L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 6L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 7L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 8L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 9L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 10L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 11L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 12L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 13L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 7L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 8L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 9L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 10L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 11L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 12L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 6L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 7L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 8L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 9L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 10L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 11L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 12L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 13L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 14L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 15L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 16L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 17L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 6L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085), new DateTime(2026, 4, 4, 21, 11, 36, 191, DateTimeKind.Utc).AddTicks(2085) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdateNote",
                table: "NumericPartResults");

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 6L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 7L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 8L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 9L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 10L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 11L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 12L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 13L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 14L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 15L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 16L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 17L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 18L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 19L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 20L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 21L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 22L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 23L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 24L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 25L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 26L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 27L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 28L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 29L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 30L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 31L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 32L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 33L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 34L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 35L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 36L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 37L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 38L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 39L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 40L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 41L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 42L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 43L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 44L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 45L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 46L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 47L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 48L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 6L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 7L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 8L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 9L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 10L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 11L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 12L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 13L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 7L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 8L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 9L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 10L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 11L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 12L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 6L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 7L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 8L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 9L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 10L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 11L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 12L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 13L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 14L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 15L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 16L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 17L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 6L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088), new DateTime(2026, 4, 4, 19, 18, 53, 925, DateTimeKind.Utc).AddTicks(8088) });
        }
    }
}
