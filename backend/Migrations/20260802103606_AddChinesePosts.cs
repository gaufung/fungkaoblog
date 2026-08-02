using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Blog.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddChinesePosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Posts",
                columns: new[] { "Id", "Content", "CreatedAt", "Published", "Slug", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { 25, "# 你好，世界！\n\n这是一篇用于测试**中文字符兼容性**的文章。如果你能正常阅读这段文字，说明数据库、接口和前端都能正确处理 UTF-8 编码。\n\n## 常见标点\n\n逗号，句号。感叹号！问号？分号；冒号：引号「你好」『世界』，还有省略号……\n\n## 列表\n\n- 苹果 🍎\n- 香蕉 🍌\n- 西瓜 🍉\n\n## 代码\n\n```csharp\nConsole.WriteLine(\"你好，世界\");\n```\n\n> 引用：路漫漫其修远兮，吾将上下而求索。\n", new DateTime(2026, 1, 25, 12, 0, 0, 0, DateTimeKind.Utc), true, "chinese-hello-world", "你好，世界：中文博客测试", new DateTime(2026, 1, 25, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 26, "# 前端开发笔记\n\n在现代前端项目中，**React** 搭配 **TypeScript** 已经成为主流选择。类型系统能在编译期发现许多潜在的错误。\n\n## 一个简单的组件\n\n```tsx\ninterface Props {\n  名称: string;\n}\n\nexport function 问候({ 名称 }: Props) {\n  return <p>你好，{名称}！</p>;\n}\n```\n\n## 小结\n\n1. 组件应保持单一职责。\n2. 尽量复用逻辑。\n3. 为公共接口编写类型。\n", new DateTime(2026, 1, 26, 12, 0, 0, 0, DateTimeKind.Utc), true, "frontend-notes-react-typescript", "前端开发笔记：React 与 TypeScript", new DateTime(2026, 1, 26, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 27, "# 异步编程\n\n在 .NET 中，`async` 和 `await` 让编写非阻塞代码变得简单直观。\n\n| 关键字 | 含义 |\n| --- | --- |\n| `async` | 标记一个异步方法 |\n| `await` | 等待异步操作完成 |\n\n```csharp\npublic async Task<string> 获取数据Async()\n{\n    await Task.Delay(100);\n    return \"完成\";\n}\n```\n\n**注意**：不要在异步方法中使用 `.Result`，否则可能导致死锁。\n", new DateTime(2026, 1, 27, 12, 0, 0, 0, DateTimeKind.Utc), true, "dotnet-csharp-async", "深入理解 .NET 与 C# 的异步编程", new DateTime(2026, 1, 27, 12, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "Id", "Name", "Slug" },
                values: new object[,]
                {
                    { 16, "教程", "jiaocheng" },
                    { 17, "前端", "qianduan" }
                });

            migrationBuilder.InsertData(
                table: "PostTag",
                columns: new[] { "PostsId", "TagsId" },
                values: new object[,]
                {
                    { 25, 15 },
                    { 25, 16 },
                    { 26, 4 },
                    { 26, 5 },
                    { 26, 17 },
                    { 27, 1 },
                    { 27, 2 },
                    { 27, 16 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "PostTag",
                keyColumns: new[] { "PostsId", "TagsId" },
                keyValues: new object[] { 25, 15 });

            migrationBuilder.DeleteData(
                table: "PostTag",
                keyColumns: new[] { "PostsId", "TagsId" },
                keyValues: new object[] { 25, 16 });

            migrationBuilder.DeleteData(
                table: "PostTag",
                keyColumns: new[] { "PostsId", "TagsId" },
                keyValues: new object[] { 26, 4 });

            migrationBuilder.DeleteData(
                table: "PostTag",
                keyColumns: new[] { "PostsId", "TagsId" },
                keyValues: new object[] { 26, 5 });

            migrationBuilder.DeleteData(
                table: "PostTag",
                keyColumns: new[] { "PostsId", "TagsId" },
                keyValues: new object[] { 26, 17 });

            migrationBuilder.DeleteData(
                table: "PostTag",
                keyColumns: new[] { "PostsId", "TagsId" },
                keyValues: new object[] { 27, 1 });

            migrationBuilder.DeleteData(
                table: "PostTag",
                keyColumns: new[] { "PostsId", "TagsId" },
                keyValues: new object[] { 27, 2 });

            migrationBuilder.DeleteData(
                table: "PostTag",
                keyColumns: new[] { "PostsId", "TagsId" },
                keyValues: new object[] { 27, 16 });

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Tags",
                keyColumn: "Id",
                keyValue: 17);
        }
    }
}
