using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Blog.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedDummyPosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Posts",
                columns: new[] { "Id", "Content", "CreatedAt", "Published", "Slug", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "# Welcome\n\nThis is the very first post on this deliberately small blog. It's built with **ASP.NET Core** on the backend and **React + TypeScript** on the front end.\n\nFeel free to look around!", new DateTime(2026, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), true, "welcome-to-my-blog", "Welcome to My Blog", new DateTime(2026, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, "# Markdown 101\n\nPosts here are authored in Markdown. A few basics:\n\n- **Bold** and _italic_ text\n- Lists, like this one\n- `inline code` and fenced code blocks\n\n```csharp\nConsole.WriteLine(\"Hello, blog!\");\n```\n", new DateTime(2026, 1, 2, 12, 0, 0, 0, DateTimeKind.Utc), true, "getting-started-with-markdown", "Getting Started with Markdown", new DateTime(2026, 1, 2, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, "# Motivation\n\nI wanted a minimal place to write, without a heavyweight CMS. This project pairs a small Web API with Azure SQL storage and Entra ID sign-in so only I can publish, while everyone can read.", new DateTime(2026, 1, 3, 12, 0, 0, 0, DateTimeKind.Utc), true, "why-i-built-this-blog", "Why I Built This Blog", new DateTime(2026, 1, 3, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, "# Backlog\n\nA scratchpad of things I might write about:\n\n1. Deploying to Azure\n2. EF Core migrations tips\n3. Securing an API with app roles\n\n_This is an unpublished draft._", new DateTime(2026, 1, 4, 12, 0, 0, 0, DateTimeKind.Utc), false, "draft-ideas-for-upcoming-posts", "Draft: Ideas for Upcoming Posts", new DateTime(2026, 1, 4, 12, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
