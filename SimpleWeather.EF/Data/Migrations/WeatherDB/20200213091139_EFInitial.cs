using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleWeather.Data.Migrations.WeatherDB
{
    public partial class EFInitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*
            migrationBuilder.DropColumn(
                name: "forecastblob",
                table: "weatherdata");

            migrationBuilder.DropColumn(
                name: "hrforecastblob",
                table: "weatherdata");

            migrationBuilder.DropColumn(
                name: "txtforecastblob",
                table: "weatherdata");
            */

            migrationBuilder.CreateTable(
                name: "forecasts",
                columns: table => new
                {
                    query = table.Column<string>(nullable: false),
                    forecast = table.Column<string>(nullable: true),
                    txt_forecast = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_forecasts", x => x.query);
                });

            migrationBuilder.CreateTable(
                name: "hr_forecasts",
                columns: table => new
                {
                    query = table.Column<string>(nullable: false),
                    hr_forecast = table.Column<string>(nullable: true),
                    dateblob = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hr_forecasts", x => new { x.query, x.dateblob });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "forecasts");

            migrationBuilder.DropTable(
                name: "hr_forecasts");

            migrationBuilder.AddColumn<string>(
                name: "forecastblob",
                table: "weatherdata",
                type: "varchar",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "hrforecastblob",
                table: "weatherdata",
                type: "varchar",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "txtforecastblob",
                table: "weatherdata",
                type: "varchar",
                nullable: true);
        }
    }
}