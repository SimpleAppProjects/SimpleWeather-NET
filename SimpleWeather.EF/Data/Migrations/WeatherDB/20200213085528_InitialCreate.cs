using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleWeather.Data.Migrations.WeatherDB
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "CREATE TABLE IF NOT EXISTS `weatheralerts` (`query` varchar primary key not null, `weather_alerts` varchar)");

            migrationBuilder.Sql(
                "CREATE TABLE IF NOT EXISTS `weatherdata` (`ttl` varchar, `source` varchar, `query` varchar primary key not null, `locale` varchar, `locationblob` varchar, `update_time` varchar, `forecastblob` varchar, `hrforecastblob` varchar, `txtforecastblob` varchar, `conditionblob` varchar, `atmosphereblob` varchar, `astronomyblob` varchar, `precipitationblob` varchar)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "weatheralerts");

            migrationBuilder.DropTable(
                name: "weatherdata");
        }
    }
}