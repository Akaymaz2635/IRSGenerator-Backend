using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IRSGenerator.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddInspectionIdToCharacterAndOpSheetPath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add OpSheetPath to Inspections
            migrationBuilder.AddColumn<string>(
                name: "OpSheetPath",
                table: "Inspections",
                type: "text",
                nullable: true);

            // Make IRSProjectId nullable in Characters
            migrationBuilder.AlterColumn<long>(
                name: "IRSProjectId",
                table: "Characters",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            // Add InspectionId to Characters
            migrationBuilder.AddColumn<long>(
                name: "InspectionId",
                table: "Characters",
                type: "bigint",
                nullable: true);

            // Add FK from Characters to Inspections
            migrationBuilder.CreateIndex(
                name: "IX_Characters_InspectionId",
                table: "Characters",
                column: "InspectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Characters_Inspections_InspectionId",
                table: "Characters",
                column: "InspectionId",
                principalTable: "Inspections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Characters_Inspections_InspectionId",
                table: "Characters");

            migrationBuilder.DropIndex(
                name: "IX_Characters_InspectionId",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "InspectionId",
                table: "Characters");

            migrationBuilder.AlterColumn<long>(
                name: "IRSProjectId",
                table: "Characters",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "OpSheetPath",
                table: "Inspections");
        }
    }
}
