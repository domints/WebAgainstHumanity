using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WebAgainstHumanity.Migrations
{
    public partial class AddCardsTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cardset",
                columns: table => new
                {
                    cdsid = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    cdsadded = table.Column<DateTime>(nullable: false),
                    cdsauthor = table.Column<string>(nullable: true),
                    cdsguid = table.Column<string>(nullable: true),
                    cdslang = table.Column<string>(nullable: true),
                    cdsname = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cardset", x => x.cdsid);
                });

            migrationBuilder.CreateTable(
                name: "card",
                columns: table => new
                {
                    crdid = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    crdcdsid = table.Column<int>(nullable: false),
                    crdcontent = table.Column<string>(nullable: true),
                    crdguid = table.Column<string>(nullable: true),
                    crdisquestion = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_card", x => x.crdid);
                    table.ForeignKey(
                        name: "FK_card_cardset_crdcdsid",
                        column: x => x.crdcdsid,
                        principalTable: "cardset",
                        principalColumn: "cdsid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_card_crdcdsid",
                table: "card",
                column: "crdcdsid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "card");

            migrationBuilder.DropTable(
                name: "cardset");
        }
    }
}
