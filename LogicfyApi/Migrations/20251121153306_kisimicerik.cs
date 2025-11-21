using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogicfyApi.Migrations
{
    /// <inheritdoc />
    public partial class kisimicerik : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KisimIcerikler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProgramlamaDiliId = table.Column<int>(type: "int", nullable: false),
                    UniteId = table.Column<int>(type: "int", nullable: false),
                    KisimId = table.Column<int>(type: "int", nullable: false),
                    Baslik = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AciklamaHtml = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrnekKod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EkstraJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KisimIcerikler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KisimIcerikler_Kisimlar_KisimId",
                        column: x => x.KisimId,
                        principalTable: "Kisimlar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KisimIcerikler_ProgramlamaDilleri_ProgramlamaDiliId",
                        column: x => x.ProgramlamaDiliId,
                        principalTable: "ProgramlamaDilleri",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KisimIcerikler_Uniteler_UniteId",
                        column: x => x.UniteId,
                        principalTable: "Uniteler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KisimIcerikler_KisimId",
                table: "KisimIcerikler",
                column: "KisimId");

            migrationBuilder.CreateIndex(
                name: "IX_KisimIcerikler_ProgramlamaDiliId",
                table: "KisimIcerikler",
                column: "ProgramlamaDiliId");

            migrationBuilder.CreateIndex(
                name: "IX_KisimIcerikler_UniteId",
                table: "KisimIcerikler",
                column: "UniteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KisimIcerikler");
        }
    }
}
