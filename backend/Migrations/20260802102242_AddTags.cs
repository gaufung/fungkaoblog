using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Blog.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PostTag",
                columns: table => new
                {
                    PostsId = table.Column<int>(type: "int", nullable: false),
                    TagsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostTag", x => new { x.PostsId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_PostTag_Posts_PostsId",
                        column: x => x.PostsId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostTag_Tags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "Id", "Name", "Slug" },
                values: new object[,]
                {
                    { 1, ".NET", "dotnet" },
                    { 2, "C#", "csharp" },
                    { 3, "ASP.NET Core", "aspnet-core" },
                    { 4, "React", "react" },
                    { 5, "TypeScript", "typescript" },
                    { 6, "CSS", "css" },
                    { 7, "Azure", "azure" },
                    { 8, "DevOps", "devops" },
                    { 9, "Testing", "testing" },
                    { 10, "Performance", "performance" },
                    { 11, "Best Practices", "best-practices" },
                    { 12, "Git", "git" },
                    { 13, "Docker", "docker" },
                    { 14, "Database", "database" },
                    { 15, "Tutorial", "tutorial" }
                });

            migrationBuilder.InsertData(
                table: "PostTag",
                columns: new[] { "PostsId", "TagsId" },
                values: new object[,]
                {
                    { 1, 2 },
                    { 1, 12 },
                    { 1, 15 },
                    { 2, 4 },
                    { 2, 5 },
                    { 3, 2 },
                    { 3, 15 },
                    { 4, 4 },
                    { 4, 5 },
                    { 4, 12 },
                    { 5, 2 },
                    { 5, 8 },
                    { 6, 7 },
                    { 6, 11 },
                    { 7, 6 },
                    { 7, 12 },
                    { 8, 7 },
                    { 9, 1 },
                    { 9, 3 },
                    { 9, 12 },
                    { 10, 9 },
                    { 10, 14 },
                    { 11, 3 },
                    { 11, 6 },
                    { 11, 7 },
                    { 12, 8 },
                    { 12, 15 },
                    { 13, 11 },
                    { 14, 9 },
                    { 15, 9 },
                    { 15, 11 },
                    { 16, 2 },
                    { 17, 3 },
                    { 17, 4 },
                    { 18, 5 },
                    { 18, 8 },
                    { 18, 9 },
                    { 19, 3 },
                    { 19, 9 },
                    { 20, 3 },
                    { 20, 7 },
                    { 21, 4 },
                    { 21, 7 },
                    { 22, 13 },
                    { 22, 14 },
                    { 23, 15 },
                    { 24, 2 },
                    { 24, 9 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostTag_TagsId",
                table: "PostTag",
                column: "TagsId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Slug",
                table: "Tags",
                column: "Slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostTag");

            migrationBuilder.DropTable(
                name: "Tags");
        }
    }
}
