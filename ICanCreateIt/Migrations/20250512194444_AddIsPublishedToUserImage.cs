using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ICanCreateIt.Migrations
{
    /// <inheritdoc />
    public partial class AddIsPublishedToUserImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "UserImages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "UserImages");
        }
    }
}
