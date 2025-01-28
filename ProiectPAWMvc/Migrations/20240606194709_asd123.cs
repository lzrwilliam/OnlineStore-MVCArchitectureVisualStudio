using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProiectPAWMvc.Migrations
{
    /// <inheritdoc />
    public partial class asd123 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ValoareAtribute_Produs_ProdusID",
                table: "ValoareAtribute");

            migrationBuilder.AddForeignKey(
                name: "FK_ValoareAtribute_Produs_ProdusID",
                table: "ValoareAtribute",
                column: "ProdusID",
                principalTable: "Produs",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ValoareAtribute_Produs_ProdusID",
                table: "ValoareAtribute");

            migrationBuilder.AddForeignKey(
                name: "FK_ValoareAtribute_Produs_ProdusID",
                table: "ValoareAtribute",
                column: "ProdusID",
                principalTable: "Produs",
                principalColumn: "ID");
        }
    }
}
