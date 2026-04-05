using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace IRSGenerator.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNcmEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dispositions_Defects_DefectId",
                table: "Dispositions");

            migrationBuilder.AlterColumn<string>(
                name: "Actual",
                table: "NumericPartResults",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "");

            migrationBuilder.CreateTable(
                name: "CauseCodes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedByUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauseCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CauseCodes_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CauseCodes_Users_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NcmDispositionTypes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Label = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    TemplateFileName = table.Column<string>(type: "text", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    SortOrder = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedById = table.Column<long>(type: "bigint", nullable: true),
                    CreatedByUserId = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedById = table.Column<long>(type: "bigint", nullable: true),
                    UpdatedByUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NcmDispositionTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NcmDispositionTypes_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NcmDispositionTypes_Users_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 6L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 7L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 8L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 9L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 10L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 11L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 12L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 13L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 14L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 15L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 16L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 17L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 18L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 19L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 20L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 21L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 22L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 23L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 24L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 25L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 26L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 27L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 28L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 29L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 30L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 31L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 32L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 33L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 34L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 35L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 36L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 37L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 38L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 39L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 40L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 41L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 42L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 43L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 44L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 45L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 46L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 47L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 48L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 6L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 7L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 8L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 9L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 10L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 11L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 12L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 13L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 7L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 8L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 9L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 10L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 11L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 12L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 6L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 7L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 8L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 9L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 10L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 11L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 12L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 13L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 14L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 15L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 16L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 17L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 6L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032), new DateTime(2026, 4, 3, 23, 3, 33, 140, DateTimeKind.Utc).AddTicks(9032) });

            migrationBuilder.CreateIndex(
                name: "IX_CauseCodes_CreatedByUserId",
                table: "CauseCodes",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CauseCodes_UpdatedByUserId",
                table: "CauseCodes",
                column: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_NcmDispositionTypes_CreatedByUserId",
                table: "NcmDispositionTypes",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_NcmDispositionTypes_UpdatedByUserId",
                table: "NcmDispositionTypes",
                column: "UpdatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dispositions_Defects_DefectId",
                table: "Dispositions",
                column: "DefectId",
                principalTable: "Defects",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dispositions_Defects_DefectId",
                table: "Dispositions");

            migrationBuilder.DropTable(
                name: "CauseCodes");

            migrationBuilder.DropTable(
                name: "NcmDispositionTypes");

            migrationBuilder.AlterColumn<string>(
                name: "Actual",
                table: "NumericPartResults",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 6L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 7L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 8L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 9L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 10L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 11L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 12L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 13L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 14L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 15L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 16L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 17L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 18L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 19L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 20L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 21L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 22L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 23L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 24L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 25L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 26L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 27L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 28L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 29L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 30L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 31L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 32L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 33L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 34L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 35L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 36L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 37L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 38L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 39L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 40L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 41L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 42L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 43L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 44L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 45L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 46L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 47L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTransitions",
                keyColumn: "Id",
                keyValue: 48L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 6L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 7L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 8L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 9L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 10L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 11L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 12L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "DispositionTypes",
                keyColumn: "Id",
                keyValue: 13L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 6L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 7L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 8L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 9L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 10L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 11L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: 12L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 6L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 7L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 8L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 9L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 10L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 11L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 12L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 13L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 14L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 15L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 16L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: 17L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 3L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 4L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 5L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: 6L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2L,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446), new DateTime(2026, 3, 28, 11, 25, 45, 136, DateTimeKind.Utc).AddTicks(7446) });

            migrationBuilder.AddForeignKey(
                name: "FK_Dispositions_Defects_DefectId",
                table: "Dispositions",
                column: "DefectId",
                principalTable: "Defects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
