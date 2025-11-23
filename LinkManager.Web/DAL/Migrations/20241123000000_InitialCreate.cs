using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LinkManager.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PageLinks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Pending"),
                    HttpStatusCode = table.Column<int>(type: "int", nullable: true),
                    ResponseTimeMs = table.Column<long>(type: "bigint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    LastCheckedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageLinks", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PageLinks_Category",
                table: "PageLinks",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "IX_PageLinks_CreatedAt",
                table: "PageLinks",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_PageLinks_Status",
                table: "PageLinks",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_PageLinks_Url",
                table: "PageLinks",
                column: "Url",
                unique: true);

            migrationBuilder.InsertData(
                table: "PageLinks",
                columns: new[] { "Id", "Category", "CreatedAt", "Description", "ErrorMessage", "HttpStatusCode", "IsActive", "LastCheckedAt", "Notes", "ResponseTimeMs", "Status", "Title", "Url" },
                values: new object[,]
                {
                    { 1, "Search Engine", new DateTime(2024, 11, 23, 0, 0, 0, 0, DateTimeKind.Utc), "Mecanismo de busca", null, null, true, null, null, null, "Pending", "Google", "https://www.google.com" },
                    { 2, "Development", new DateTime(2024, 11, 23, 0, 0, 0, 0, DateTimeKind.Utc), "Plataforma de desenvolvimento", null, null, true, null, null, null, "Pending", "GitHub", "https://www.github.com" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PageLinks");
        }
    }
}
