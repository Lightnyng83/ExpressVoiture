using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpressVoitures.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixDbFieldName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Car_CarBrandModelId",
                table: "Car");

            migrationBuilder.DropTable(
                name: "CarBrandModelId");

            migrationBuilder.CreateTable(
                name: "CarBrandModel",
                columns: table => new
                {
                    CarBrandModelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarBrandId = table.Column<int>(type: "int", nullable: false),
                    CarModelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarBrandModelId_1", x => x.CarBrandModelId);
                    table.ForeignKey(
                        name: "FK_CarBrandModelId_CarBrand1",
                        column: x => x.CarBrandId,
                        principalTable: "CarBrand",
                        principalColumn: "CarBrandId");
                    table.ForeignKey(
                        name: "FK_CarBrandModelId_CarModel1",
                        column: x => x.CarModelId,
                        principalTable: "CarModel",
                        principalColumn: "CarModelId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarBrandModel_CarBrandId",
                table: "CarBrandModel",
                column: "CarBrandId");

            migrationBuilder.CreateIndex(
                name: "IX_CarBrandModel_CarModelId",
                table: "CarBrandModel",
                column: "CarModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Car_CarBrandModelId",
                table: "Car",
                column: "CarBrandModelId",
                principalTable: "CarBrandModel",
                principalColumn: "CarBrandModelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Car_CarBrandModelId",
                table: "Car");

            migrationBuilder.DropTable(
                name: "CarBrandModel");

            migrationBuilder.CreateTable(
                name: "CarBrandModelId",
                columns: table => new
                {
                    CarBrandModelId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CarBrandId = table.Column<int>(type: "int", nullable: false),
                    CarModelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CarBrandModelId_1", x => x.CarBrandModelId);
                    table.ForeignKey(
                        name: "FK_CarBrandModelId_CarBrand1",
                        column: x => x.CarBrandId,
                        principalTable: "CarBrand",
                        principalColumn: "CarBrandId");
                    table.ForeignKey(
                        name: "FK_CarBrandModelId_CarModel1",
                        column: x => x.CarModelId,
                        principalTable: "CarModel",
                        principalColumn: "CarModelId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CarBrandModelId_CarBrandId",
                table: "CarBrandModelId",
                column: "CarBrandId");

            migrationBuilder.CreateIndex(
                name: "IX_CarBrandModelId_CarModelId",
                table: "CarBrandModelId",
                column: "CarModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Car_CarBrandModelId",
                table: "Car",
                column: "CarBrandModelId",
                principalTable: "CarBrandModelId",
                principalColumn: "CarBrandModelId");
        }
    }
}
