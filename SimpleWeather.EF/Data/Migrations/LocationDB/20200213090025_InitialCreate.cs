using Microsoft.EntityFrameworkCore.Migrations;

namespace SimpleWeather.Data.Migrations.LocationDB
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "CREATE TABLE IF NOT EXISTS `favorites` (`query` varchar primary key not null, `position` integer)");

            migrationBuilder.Sql(
                "CREATE TABLE IF NOT EXISTS `locations` (`query` varchar primary key not null, `name` varchar, `latitude` float, `longitude` float, `tz_long` varchar, `locationType` integer, `source` varchar, `locsource` varchar)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "favorites");

            migrationBuilder.DropTable(
                name: "locations");
        }
    }
}