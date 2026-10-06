using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Blog.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgreSql : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Slug = table.Column<string>(type: "character varying(220)", maxLength: 220, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Published = table.Column<bool>(type: "boolean", nullable: false),
                    SourceNumber = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Slug = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PostTag",
                columns: table => new
                {
                    PostsId = table.Column<int>(type: "integer", nullable: false),
                    TagsId = table.Column<int>(type: "integer", nullable: false)
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
                table: "Posts",
                columns: new[] { "Id", "Content", "CreatedAt", "Published", "Slug", "SourceNumber", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, "# Welcome\n\nThis is the very first post on this deliberately small blog. It's built with **ASP.NET Core** on the backend and **React + TypeScript** on the front end.\n\nFeel free to look around!", new DateTime(2026, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), true, "welcome-to-my-blog", null, "Welcome to My Blog", new DateTime(2026, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, "# Markdown 101\n\nPosts here are authored in Markdown. A few basics:\n\n- **Bold** and _italic_ text\n- Lists, like this one\n- `inline code` and fenced code blocks\n\n```csharp\nConsole.WriteLine(\"Hello, blog!\");\n```\n", new DateTime(2026, 1, 2, 12, 0, 0, 0, DateTimeKind.Utc), true, "getting-started-with-markdown", null, "Getting Started with Markdown", new DateTime(2026, 1, 2, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, "# Motivation\n\nI wanted a minimal place to write, without a heavyweight CMS. This project pairs a small Web API with PostgreSQL storage and GitHub content synchronization, while everyone can read.", new DateTime(2026, 1, 3, 12, 0, 0, 0, DateTimeKind.Utc), true, "why-i-built-this-blog", null, "Why I Built This Blog", new DateTime(2026, 1, 3, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, "# Backlog\n\nA scratchpad of things I might write about:\n\n1. Deploying to Azure\n2. EF Core migrations tips\n3. Securing an API with app roles\n\n_This is an unpublished draft._", new DateTime(2026, 1, 4, 12, 0, 0, 0, DateTimeKind.Utc), false, "draft-ideas-for-upcoming-posts", null, "Draft: Ideas for Upcoming Posts", new DateTime(2026, 1, 4, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, "# Understanding Async and Await in C#\n\nThis is a placeholder post about understanding async and await in c#. Real content coming soon — for now it exists to demonstrate pagination.", new DateTime(2026, 1, 5, 12, 0, 0, 0, DateTimeKind.Utc), true, "understanding-async-and-await-in-c", null, "Understanding Async and Await in C#", new DateTime(2026, 1, 5, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 6, "# A Practical Tour of LINQ\n\nThis is a placeholder post about a practical tour of linq. Real content coming soon — for now it exists to demonstrate pagination.", new DateTime(2026, 1, 6, 12, 0, 0, 0, DateTimeKind.Utc), true, "a-practical-tour-of-linq", null, "A Practical Tour of LINQ", new DateTime(2026, 1, 6, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, "# Dependency Injection Explained\n\nThis is a placeholder post about dependency injection explained. Real content coming soon — for now it exists to demonstrate pagination.", new DateTime(2026, 1, 7, 12, 0, 0, 0, DateTimeKind.Utc), true, "dependency-injection-explained", null, "Dependency Injection Explained", new DateTime(2026, 1, 7, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, "# Entity Framework Core Migrations Tips\n\nThis is a placeholder post about entity framework core migrations tips. Real content coming soon — for now it exists to demonstrate pagination.", new DateTime(2026, 1, 8, 12, 0, 0, 0, DateTimeKind.Utc), true, "entity-framework-core-migrations-tips", null, "Entity Framework Core Migrations Tips", new DateTime(2026, 1, 8, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 9, "# Building Minimal APIs in ASP.NET Core\n\nThis is a placeholder post about building minimal apis in asp.net core. Real content coming soon — for now it exists to demonstrate pagination.", new DateTime(2026, 1, 9, 12, 0, 0, 0, DateTimeKind.Utc), true, "building-minimal-apis-in-aspnet-core", null, "Building Minimal APIs in ASP.NET Core", new DateTime(2026, 1, 9, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 10, "# React Hooks You Should Know\n\nThis is a placeholder post about react hooks you should know. Real content coming soon — for now it exists to demonstrate pagination.", new DateTime(2026, 1, 10, 12, 0, 0, 0, DateTimeKind.Utc), true, "react-hooks-you-should-know", null, "React Hooks You Should Know", new DateTime(2026, 1, 10, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 11, "# TypeScript Generics Made Simple\n\nThis is a placeholder post about typescript generics made simple. Real content coming soon — for now it exists to demonstrate pagination.", new DateTime(2026, 1, 11, 12, 0, 0, 0, DateTimeKind.Utc), true, "typescript-generics-made-simple", null, "TypeScript Generics Made Simple", new DateTime(2026, 1, 11, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 12, "# State Management Without a Library\n\nThis is a placeholder post about state management without a library. Real content coming soon — for now it exists to demonstrate pagination.", new DateTime(2026, 1, 12, 12, 0, 0, 0, DateTimeKind.Utc), true, "state-management-without-a-library", null, "State Management Without a Library", new DateTime(2026, 1, 12, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 13, "# Styling with Modern CSS\n\nThis is a placeholder post about styling with modern css. Real content coming soon — for now it exists to demonstrate pagination.", new DateTime(2026, 1, 13, 12, 0, 0, 0, DateTimeKind.Utc), true, "styling-with-modern-css", null, "Styling with Modern CSS", new DateTime(2026, 1, 13, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 14, "# Debugging Like a Pro\n\nThis is a placeholder post about debugging like a pro. Real content coming soon — for now it exists to demonstrate pagination.", new DateTime(2026, 1, 14, 12, 0, 0, 0, DateTimeKind.Utc), true, "debugging-like-a-pro", null, "Debugging Like a Pro", new DateTime(2026, 1, 14, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 15, "# Writing Better Git Commit Messages\n\nThis is a placeholder post about writing better git commit messages. Real content coming soon — for now it exists to demonstrate pagination.", new DateTime(2026, 1, 15, 12, 0, 0, 0, DateTimeKind.Utc), true, "writing-better-git-commit-messages", null, "Writing Better Git Commit Messages", new DateTime(2026, 1, 15, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 16, "# An Introduction to Docker\n\nThis is a placeholder post about an introduction to docker. Real content coming soon — for now it exists to demonstrate pagination.", new DateTime(2026, 1, 16, 12, 0, 0, 0, DateTimeKind.Utc), true, "an-introduction-to-docker", null, "An Introduction to Docker", new DateTime(2026, 1, 16, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 17, "# Deploying to Azure App Service\n\nThis is a placeholder post about deploying to azure app service. Real content coming soon — for now it exists to demonstrate pagination.", new DateTime(2026, 1, 17, 12, 0, 0, 0, DateTimeKind.Utc), true, "deploying-to-azure-app-service", null, "Deploying to Azure App Service", new DateTime(2026, 1, 17, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 18, "# Caching Strategies for Web Apps\n\nThis is a placeholder post about caching strategies for web apps. Real content coming soon — for now it exists to demonstrate pagination.", new DateTime(2026, 1, 18, 12, 0, 0, 0, DateTimeKind.Utc), true, "caching-strategies-for-web-apps", null, "Caching Strategies for Web Apps", new DateTime(2026, 1, 18, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 19, "# Securing Your REST API\n\nThis is a placeholder post about securing your rest api. Real content coming soon — for now it exists to demonstrate pagination.", new DateTime(2026, 1, 19, 12, 0, 0, 0, DateTimeKind.Utc), true, "securing-your-rest-api", null, "Securing Your REST API", new DateTime(2026, 1, 19, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 20, "# Unit Testing Fundamentals\n\nThis is a placeholder post about unit testing fundamentals. Real content coming soon — for now it exists to demonstrate pagination.", new DateTime(2026, 1, 20, 12, 0, 0, 0, DateTimeKind.Utc), true, "unit-testing-fundamentals", null, "Unit Testing Fundamentals", new DateTime(2026, 1, 20, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 21, "# Working with JSON in .NET\n\nThis is a placeholder post about working with json in .net. Real content coming soon — for now it exists to demonstrate pagination.", new DateTime(2026, 1, 21, 12, 0, 0, 0, DateTimeKind.Utc), true, "working-with-json-in-net", null, "Working with JSON in .NET", new DateTime(2026, 1, 21, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 22, "# Optimizing Frontend Performance\n\nThis is a placeholder post about optimizing frontend performance. Real content coming soon — for now it exists to demonstrate pagination.", new DateTime(2026, 1, 22, 12, 0, 0, 0, DateTimeKind.Utc), true, "optimizing-frontend-performance", null, "Optimizing Frontend Performance", new DateTime(2026, 1, 22, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 23, "# Clean Code Principles\n\nThis is a placeholder post about clean code principles. Real content coming soon — for now it exists to demonstrate pagination.", new DateTime(2026, 1, 23, 12, 0, 0, 0, DateTimeKind.Utc), true, "clean-code-principles", null, "Clean Code Principles", new DateTime(2026, 1, 23, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 24, "# My Favorite Developer Tools\n\nThis is a placeholder post about my favorite developer tools. Real content coming soon — for now it exists to demonstrate pagination.", new DateTime(2026, 1, 24, 12, 0, 0, 0, DateTimeKind.Utc), true, "my-favorite-developer-tools", null, "My Favorite Developer Tools", new DateTime(2026, 1, 24, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 25, "# 你好，世界！\n\n这是一篇用于测试**中文字符兼容性**的文章。如果你能正常阅读这段文字，说明数据库、接口和前端都能正确处理 UTF-8 编码。\n\n## 常见标点\n\n逗号，句号。感叹号！问号？分号；冒号：引号「你好」『世界』，还有省略号……\n\n## 列表\n\n- 苹果 🍎\n- 香蕉 🍌\n- 西瓜 🍉\n\n## 代码\n\n```csharp\nConsole.WriteLine(\"你好，世界\");\n```\n\n> 引用：路漫漫其修远兮，吾将上下而求索。\n", new DateTime(2026, 1, 25, 12, 0, 0, 0, DateTimeKind.Utc), true, "chinese-hello-world", null, "你好，世界：中文博客测试", new DateTime(2026, 1, 25, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 26, "# 前端开发笔记\n\n在现代前端项目中，**React** 搭配 **TypeScript** 已经成为主流选择。类型系统能在编译期发现许多潜在的错误。\n\n## 一个简单的组件\n\n```tsx\ninterface Props {\n  名称: string;\n}\n\nexport function 问候({ 名称 }: Props) {\n  return <p>你好，{名称}！</p>;\n}\n```\n\n## 小结\n\n1. 组件应保持单一职责。\n2. 尽量复用逻辑。\n3. 为公共接口编写类型。\n", new DateTime(2026, 1, 26, 12, 0, 0, 0, DateTimeKind.Utc), true, "frontend-notes-react-typescript", null, "前端开发笔记：React 与 TypeScript", new DateTime(2026, 1, 26, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 27, "# 异步编程\n\n在 .NET 中，`async` 和 `await` 让编写非阻塞代码变得简单直观。\n\n| 关键字 | 含义 |\n| --- | --- |\n| `async` | 标记一个异步方法 |\n| `await` | 等待异步操作完成 |\n\n```csharp\npublic async Task<string> 获取数据Async()\n{\n    await Task.Delay(100);\n    return \"完成\";\n}\n```\n\n**注意**：不要在异步方法中使用 `.Result`，否则可能导致死锁。\n", new DateTime(2026, 1, 27, 12, 0, 0, 0, DateTimeKind.Utc), true, "dotnet-csharp-async", null, "深入理解 .NET 与 C# 的异步编程", new DateTime(2026, 1, 27, 12, 0, 0, 0, DateTimeKind.Utc) }
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
                    { 15, "Tutorial", "tutorial" },
                    { 16, "教程", "jiaocheng" },
                    { 17, "前端", "qianduan" }
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
                    { 24, 9 },
                    { 25, 15 },
                    { 25, 16 },
                    { 26, 4 },
                    { 26, 5 },
                    { 26, 17 },
                    { 27, 1 },
                    { 27, 2 },
                    { 27, 16 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Posts_Slug",
                table: "Posts",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_SourceNumber",
                table: "Posts",
                column: "SourceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_Title",
                table: "Posts",
                column: "Title",
                unique: true);

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
                name: "Posts");

            migrationBuilder.DropTable(
                name: "Tags");
        }
    }
}
