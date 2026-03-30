using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IRSGenerator.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNoteToCharacterAndPartLabelToNumericResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add Note to Characters
            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Characters",
                type: "text",
                nullable: true);

            // Alter NumericPartResults.Actual from double to string
            migrationBuilder.AlterColumn<string>(
                name: "Actual",
                table: "NumericPartResults",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(double),
                oldType: "double precision");

            // Add PartLabel to NumericPartResults
            migrationBuilder.AddColumn<string>(
                name: "PartLabel",
                table: "NumericPartResults",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                table: "Characters");

            migrationBuilder.DropColumn(
                name: "PartLabel",
                table: "NumericPartResults");

            migrationBuilder.AlterColumn<double>(
                name: "Actual",
                table: "NumericPartResults",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "");
        }
    }
}
