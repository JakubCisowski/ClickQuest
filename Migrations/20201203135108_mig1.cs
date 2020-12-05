using Microsoft.EntityFrameworkCore.Migrations;

namespace ClickQuest.Migrations
{
	public partial class mig1 : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<int>(
				name: "ArtifactId",
				table: "Recipes",
				type: "int",
				nullable: false,
				defaultValue: 0);

			migrationBuilder.AddColumn<string>(
				name: "Description",
				table: "Recipes",
				type: "nvarchar(max)",
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "Description",
				table: "Materials",
				type: "nvarchar(max)",
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "Description",
				table: "Artifacts",
				type: "nvarchar(max)",
				nullable: true);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "ArtifactId",
				table: "Recipes");

			migrationBuilder.DropColumn(
				name: "Description",
				table: "Recipes");

			migrationBuilder.DropColumn(
				name: "Description",
				table: "Materials");

			migrationBuilder.DropColumn(
				name: "Description",
				table: "Artifacts");
		}
	}
}
