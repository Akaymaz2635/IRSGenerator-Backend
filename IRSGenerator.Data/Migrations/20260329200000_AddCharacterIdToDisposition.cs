using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IRSGenerator.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCharacterIdToDisposition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Make DefectId nullable
            migrationBuilder.AlterColumn<long>(
                name: "DefectId",
                table: "Dispositions",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            // Add nullable CharacterId FK
            migrationBuilder.AddColumn<long>(
                name: "CharacterId",
                table: "Dispositions",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dispositions_CharacterId",
                table: "Dispositions",
                column: "CharacterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Dispositions_Characters_CharacterId",
                table: "Dispositions",
                column: "CharacterId",
                principalTable: "Characters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dispositions_Characters_CharacterId",
                table: "Dispositions");

            migrationBuilder.DropIndex(
                name: "IX_Dispositions_CharacterId",
                table: "Dispositions");

            migrationBuilder.DropColumn(
                name: "CharacterId",
                table: "Dispositions");

            migrationBuilder.AlterColumn<long>(
                name: "DefectId",
                table: "Dispositions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }
    }
}
