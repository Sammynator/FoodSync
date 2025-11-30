using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodSyncAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderItemPrimaryKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MenuItem",
                table: "OrderItems");

            migrationBuilder.AddColumn<int>(
                name: "MenuItemId",
                table: "OrderItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MenuItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItems", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_MenuItemId",
                table: "OrderItems",
                column: "MenuItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderItems_MenuItems_MenuItemId",
                table: "OrderItems",
                column: "MenuItemId",
                principalTable: "MenuItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderItems_MenuItems_MenuItemId",
                table: "OrderItems");

            migrationBuilder.DropTable(
                name: "MenuItems");

            migrationBuilder.DropIndex(
                name: "IX_OrderItems_MenuItemId",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "MenuItemId",
                table: "OrderItems");

            migrationBuilder.AddColumn<string>(
                name: "MenuItem",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
