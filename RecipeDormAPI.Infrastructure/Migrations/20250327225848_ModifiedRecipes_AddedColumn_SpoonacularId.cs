using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipeDormAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedRecipes_AddedColumn_SpoonacularId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SpoonacularId",
                table: "Recipes",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpoonacularId",
                table: "Recipes");
        }
    }
}
