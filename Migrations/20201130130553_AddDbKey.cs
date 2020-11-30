using Microsoft.EntityFrameworkCore.Migrations;

namespace ClickQuest.Migrations
{
	public partial class AddDbKey : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropPrimaryKey(
				name: "PK_Recipes",
				table: "Recipes");

			migrationBuilder.DropPrimaryKey(
				name: "PK_Materials",
				table: "Materials");

			migrationBuilder.DropPrimaryKey(
				name: "PK_Artifacts",
				table: "Artifacts");

			migrationBuilder.AddColumn<int>(
				name: "DbKey",
				table: "Recipes",
				type: "int",
				nullable: false,
				defaultValue: 0)
				.Annotation("SqlServer:Identity", "1, 1");

			migrationBuilder.AddColumn<int>(
				name: "DbKey",
				table: "Materials",
				type: "int",
				nullable: false,
				defaultValue: 0)
				.Annotation("SqlServer:Identity", "1, 1");

			migrationBuilder.AddColumn<int>(
				name: "DbKey",
				table: "Artifacts",
				type: "int",
				nullable: false,
				defaultValue: 0)
				.Annotation("SqlServer:Identity", "1, 1");

			migrationBuilder.AddPrimaryKey(
				name: "PK_Recipes",
				table: "Recipes",
				column: "DbKey");

			migrationBuilder.AddPrimaryKey(
				name: "PK_Materials",
				table: "Materials",
				column: "DbKey");

			migrationBuilder.AddPrimaryKey(
				name: "PK_Artifacts",
				table: "Artifacts",
				column: "DbKey");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropPrimaryKey(
				name: "PK_Recipes",
				table: "Recipes");

			migrationBuilder.DropPrimaryKey(
				name: "PK_Materials",
				table: "Materials");

			migrationBuilder.DropPrimaryKey(
				name: "PK_Artifacts",
				table: "Artifacts");

			migrationBuilder.DropColumn(
				name: "DbKey",
				table: "Recipes");

			migrationBuilder.DropColumn(
				name: "DbKey",
				table: "Materials");

			migrationBuilder.DropColumn(
				name: "DbKey",
				table: "Artifacts");

			migrationBuilder.AddPrimaryKey(
				name: "PK_Recipes",
				table: "Recipes",
				column: "Id");

			migrationBuilder.AddPrimaryKey(
				name: "PK_Materials",
				table: "Materials",
				column: "Id");

			migrationBuilder.AddPrimaryKey(
				name: "PK_Artifacts",
				table: "Artifacts",
				column: "Id");
		}
	}
}
