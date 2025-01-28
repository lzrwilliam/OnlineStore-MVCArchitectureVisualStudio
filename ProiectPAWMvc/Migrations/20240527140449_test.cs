using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProiectPAWMvc.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nume = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParentCategorieID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Categories_Categories_ParentCategorieID",
                        column: x => x.ParentCategorieID,
                        principalTable: "Categories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Utilizator",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nume = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Parola = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipUtilizator = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilizator", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "AtributCategorii",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nume = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tip = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategorieID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AtributCategorii", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AtributCategorii_Categories_CategorieID",
                        column: x => x.CategorieID,
                        principalTable: "Categories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Produs",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nume = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descriere = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pret = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PreviousPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Stoc = table.Column<int>(type: "int", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Poza = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategorieID = table.Column<int>(type: "int", nullable: false),
                    SubcategorieID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Produs_Categories_CategorieID",
                        column: x => x.CategorieID,
                        principalTable: "Categories",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Produs_Categories_SubcategorieID",
                        column: x => x.SubcategorieID,
                        principalTable: "Categories",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Comenzi",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UtilizatorID = table.Column<int>(type: "int", nullable: false),
                    DataComanda = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalComanda = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comenzi", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Comenzi_Utilizator_UtilizatorID",
                        column: x => x.UtilizatorID,
                        principalTable: "Utilizator",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AlertaPret",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UtilizatorID = table.Column<int>(type: "int", nullable: false),
                    ProdusID = table.Column<int>(type: "int", nullable: false),
                    PretAlerta = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertaPret", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AlertaPret_Produs_ProdusID",
                        column: x => x.ProdusID,
                        principalTable: "Produs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AlertaPret_Utilizator_UtilizatorID",
                        column: x => x.UtilizatorID,
                        principalTable: "Utilizator",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CosCumparaturi",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UtilizatorID = table.Column<int>(type: "int", nullable: false),
                    ProdusID = table.Column<int>(type: "int", nullable: false),
                    Cantitate = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CosCumparaturi", x => x.ID);
                    table.ForeignKey(
                        name: "FK_CosCumparaturi_Produs_ProdusID",
                        column: x => x.ProdusID,
                        principalTable: "Produs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CosCumparaturi_Utilizator_UtilizatorID",
                        column: x => x.UtilizatorID,
                        principalTable: "Utilizator",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Favorite",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UtilizatorID = table.Column<int>(type: "int", nullable: false),
                    ProdusID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorite", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Favorite_Produs_ProdusID",
                        column: x => x.ProdusID,
                        principalTable: "Produs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Favorite_Utilizator_UtilizatorID",
                        column: x => x.UtilizatorID,
                        principalTable: "Utilizator",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notificari",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UtilizatorID = table.Column<int>(type: "int", nullable: false),
                    ProdusID = table.Column<int>(type: "int", nullable: false),
                    ProdusNume = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PretNou = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PretVechi = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DataNotificare = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Citita = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificari", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Notificari_Produs_ProdusID",
                        column: x => x.ProdusID,
                        principalTable: "Produs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notificari_Utilizator_UtilizatorID",
                        column: x => x.UtilizatorID,
                        principalTable: "Utilizator",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProdusVizualizat",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UtilizatorID = table.Column<int>(type: "int", nullable: false),
                    ProdusID = table.Column<int>(type: "int", nullable: false),
                    DataVizualizare = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProdusVizualizat", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ProdusVizualizat_Produs_ProdusID",
                        column: x => x.ProdusID,
                        principalTable: "Produs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProdusVizualizat_Utilizator_UtilizatorID",
                        column: x => x.UtilizatorID,
                        principalTable: "Utilizator",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Recenzie",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UtilizatorID = table.Column<int>(type: "int", nullable: false),
                    ProdusID = table.Column<int>(type: "int", nullable: false),
                    TextRecenzie = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Evaluare = table.Column<int>(type: "int", nullable: false),
                    DataRecenzie = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recenzie", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Recenzie_Produs_ProdusID",
                        column: x => x.ProdusID,
                        principalTable: "Produs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Recenzie_Utilizator_UtilizatorID",
                        column: x => x.UtilizatorID,
                        principalTable: "Utilizator",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ValoareAtribute",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProdusID = table.Column<int>(type: "int", nullable: false),
                    AtributCategorieID = table.Column<int>(type: "int", nullable: false),
                    Valoare = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValoareAtribute", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ValoareAtribute_AtributCategorii_AtributCategorieID",
                        column: x => x.AtributCategorieID,
                        principalTable: "AtributCategorii",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ValoareAtribute_Produs_ProdusID",
                        column: x => x.ProdusID,
                        principalTable: "Produs",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "DetaliiComenzi",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ComandaID = table.Column<int>(type: "int", nullable: false),
                    ProdusID = table.Column<int>(type: "int", nullable: true),
                    Cantitate = table.Column<int>(type: "int", nullable: false),
                    PretUnitate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NumeProdus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetaliiComenzi", x => x.ID);
                    table.ForeignKey(
                        name: "FK_DetaliiComenzi_Comenzi_ComandaID",
                        column: x => x.ComandaID,
                        principalTable: "Comenzi",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetaliiComenzi_Produs_ProdusID",
                        column: x => x.ProdusID,
                        principalTable: "Produs",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlertaPret_ProdusID",
                table: "AlertaPret",
                column: "ProdusID");

            migrationBuilder.CreateIndex(
                name: "IX_AlertaPret_UtilizatorID",
                table: "AlertaPret",
                column: "UtilizatorID");

            migrationBuilder.CreateIndex(
                name: "IX_AtributCategorii_CategorieID",
                table: "AtributCategorii",
                column: "CategorieID");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentCategorieID",
                table: "Categories",
                column: "ParentCategorieID");

            migrationBuilder.CreateIndex(
                name: "IX_Comenzi_UtilizatorID",
                table: "Comenzi",
                column: "UtilizatorID");

            migrationBuilder.CreateIndex(
                name: "IX_CosCumparaturi_ProdusID",
                table: "CosCumparaturi",
                column: "ProdusID");

            migrationBuilder.CreateIndex(
                name: "IX_CosCumparaturi_UtilizatorID",
                table: "CosCumparaturi",
                column: "UtilizatorID");

            migrationBuilder.CreateIndex(
                name: "IX_DetaliiComenzi_ComandaID",
                table: "DetaliiComenzi",
                column: "ComandaID");

            migrationBuilder.CreateIndex(
                name: "IX_DetaliiComenzi_ProdusID",
                table: "DetaliiComenzi",
                column: "ProdusID");

            migrationBuilder.CreateIndex(
                name: "IX_Favorite_ProdusID",
                table: "Favorite",
                column: "ProdusID");

            migrationBuilder.CreateIndex(
                name: "IX_Favorite_UtilizatorID",
                table: "Favorite",
                column: "UtilizatorID");

            migrationBuilder.CreateIndex(
                name: "IX_Notificari_ProdusID",
                table: "Notificari",
                column: "ProdusID");

            migrationBuilder.CreateIndex(
                name: "IX_Notificari_UtilizatorID",
                table: "Notificari",
                column: "UtilizatorID");

            migrationBuilder.CreateIndex(
                name: "IX_Produs_CategorieID",
                table: "Produs",
                column: "CategorieID");

            migrationBuilder.CreateIndex(
                name: "IX_Produs_SubcategorieID",
                table: "Produs",
                column: "SubcategorieID");

            migrationBuilder.CreateIndex(
                name: "IX_ProdusVizualizat_ProdusID",
                table: "ProdusVizualizat",
                column: "ProdusID");

            migrationBuilder.CreateIndex(
                name: "IX_ProdusVizualizat_UtilizatorID",
                table: "ProdusVizualizat",
                column: "UtilizatorID");

            migrationBuilder.CreateIndex(
                name: "IX_Recenzie_ProdusID",
                table: "Recenzie",
                column: "ProdusID");

            migrationBuilder.CreateIndex(
                name: "IX_Recenzie_UtilizatorID",
                table: "Recenzie",
                column: "UtilizatorID");

            migrationBuilder.CreateIndex(
                name: "IX_ValoareAtribute_AtributCategorieID",
                table: "ValoareAtribute",
                column: "AtributCategorieID");

            migrationBuilder.CreateIndex(
                name: "IX_ValoareAtribute_ProdusID",
                table: "ValoareAtribute",
                column: "ProdusID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlertaPret");

            migrationBuilder.DropTable(
                name: "CosCumparaturi");

            migrationBuilder.DropTable(
                name: "DetaliiComenzi");

            migrationBuilder.DropTable(
                name: "Favorite");

            migrationBuilder.DropTable(
                name: "Notificari");

            migrationBuilder.DropTable(
                name: "ProdusVizualizat");

            migrationBuilder.DropTable(
                name: "Recenzie");

            migrationBuilder.DropTable(
                name: "ValoareAtribute");

            migrationBuilder.DropTable(
                name: "Comenzi");

            migrationBuilder.DropTable(
                name: "AtributCategorii");

            migrationBuilder.DropTable(
                name: "Produs");

            migrationBuilder.DropTable(
                name: "Utilizator");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
