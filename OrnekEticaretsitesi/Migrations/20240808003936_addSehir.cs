using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrnekEticaretsitesi.Migrations
{
    /// <inheritdoc />
    public partial class addSehir : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Sehir",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sehir",
                table: "OrderHeaders");
        }
    }
}
