using Microsoft.EntityFrameworkCore.Migrations;

namespace ClickQuest.Migrations
{
	public partial class FixIngots : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_Ingots_Heroes_HeroId",
				table: "Ingots");

			migrationBuilder.DropIndex(
				name: "IX_Ingots_HeroId",
				table: "Ingots");

			migrationBuilder.DropColumn(
				name: "HeroId",
				table: "Ingots");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<int>(
				name: "HeroId",
				table: "Ingots",
				type: "int",
				nullable: true);

			migrationBuilder.CreateIndex(
				name: "IX_Ingots_HeroId",
				table: "Ingots",
				column: "HeroId");

			migrationBuilder.AddForeignKey(
				name: "FK_Ingots_Heroes_HeroId",
				table: "Ingots",
				column: "HeroId",
				principalTable: "Heroes",
				principalColumn: "Id",
				onDelete: ReferentialAction.Restrict);
		}
	}
}
