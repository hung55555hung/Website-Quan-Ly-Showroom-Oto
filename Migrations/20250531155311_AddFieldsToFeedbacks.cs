using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebPBL3.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldsToFeedbacks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Thêm các cột vào bảng Feedbacks
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Feedbacks",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Feedbacks",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Feedbacks",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Xóa các cột khỏi bảng Feedbacks
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "Feedbacks");
        }
    }
}

