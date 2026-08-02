using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Blog.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedMorePosts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Posts",
                columns: new[] { "Id", "Content", "CreatedAt", "Published", "Slug", "Title", "UpdatedAt" },
                values: new object[,]
                {
                    { 5, "# Understanding Async and Await in C#\n\nThis is a placeholder post about understanding async and await in c#. Real content coming soon — for now it exists to demonstrate pagination.", new DateTime(2026, 1, 5, 12, 0, 0, 0, DateTimeKind.Utc), true, "understanding-async-and-await-in-c", "Understanding Async and Await in C#", new DateTime(2026, 1, 5, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 6, "# A Practical Tour of LINQ\n\nThis is a placeholder post about a practical tour of linq. Real content coming soon — for now it exists to demonstrate pagination.", new DateTime(2026, 1, 6, 12, 0, 0, 0, DateTimeKind.Utc), true, "a-practical-tour-of-linq", "A Practical Tour of LINQ", new DateTime(2026, 1, 6, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, "# Dependency Injection Explained\n\nThis is a placeholder post about dependency injection explained. Real content coming soon — for now it exists to demonstrate pagination.", new DateTime(2026, 1, 7, 12, 0, 0, 0, DateTimeKind.Utc), true, "dependency-injection-explained", "Dependency Injection Explained", new DateTime(2026, 1, 7, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, "# Entity Framework Core Migrations Tips\n\nThis is a placeholder post about entity framework core migrations tips. Real content coming soon — for now it exists to demonstrate pagination.", new DateTime(2026, 1, 8, 12, 0, 0, 0, DateTimeKind.Utc), true, "entity-framework-core-migrations-tips", "Entity Framework Core Migrations Tips", new DateTime(2026, 1, 8, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 9, "# Building Minimal APIs in ASP.NET Core\n\nThis is a placeholder post about building minimal apis in asp.net core. Real content coming soon — for now it exists to demonstrate pagination.", new DateTime(2026, 1, 9, 12, 0, 0, 0, DateTimeKind.Utc), true, "building-minimal-apis-in-aspnet-core", "Building Minimal APIs in ASP.NET Core", new DateTime(2026, 1, 9, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 10, "# React Hooks You Should Know\n\nThis is a placeholder post about react hooks you should know. Real content coming soon — for now it exists to demonstrate pagination.", new DateTime(2026, 1, 10, 12, 0, 0, 0, DateTimeKind.Utc), true, "react-hooks-you-should-know", "React Hooks You Should Know", new DateTime(2026, 1, 10, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 11, "# TypeScript Generics Made Simple\n\nThis is a placeholder post about typescript generics made simple. Real content coming soon — for now it exists to demonstrate pagination.", new DateTime(2026, 1, 11, 12, 0, 0, 0, DateTimeKind.Utc), true, "typescript-generics-made-simple", "TypeScript Generics Made Simple", new DateTime(2026, 1, 11, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 12, "# State Management Without a Library\n\nThis is a placeholder post about state management without a library. Real content coming soon — for now it exists to demonstrate pagination.", new DateTime(2026, 1, 12, 12, 0, 0, 0, DateTimeKind.Utc), true, "state-management-without-a-library", "State Management Without a Library", new DateTime(2026, 1, 12, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 13, "# Styling with Modern CSS\n\nThis is a placeholder post about styling with modern css. Real content coming soon — for now it exists to demonstrate pagination.", new DateTime(2026, 1, 13, 12, 0, 0, 0, DateTimeKind.Utc), true, "styling-with-modern-css", "Styling with Modern CSS", new DateTime(2026, 1, 13, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 14, "# Debugging Like a Pro\n\nThis is a placeholder post about debugging like a pro. Real content coming soon — for now it exists to demonstrate pagination.", new DateTime(2026, 1, 14, 12, 0, 0, 0, DateTimeKind.Utc), true, "debugging-like-a-pro", "Debugging Like a Pro", new DateTime(2026, 1, 14, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 15, "# Writing Better Git Commit Messages\n\nThis is a placeholder post about writing better git commit messages. Real content coming soon — for now it exists to demonstrate pagination.", new DateTime(2026, 1, 15, 12, 0, 0, 0, DateTimeKind.Utc), true, "writing-better-git-commit-messages", "Writing Better Git Commit Messages", new DateTime(2026, 1, 15, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 16, "# An Introduction to Docker\n\nThis is a placeholder post about an introduction to docker. Real content coming soon — for now it exists to demonstrate pagination.", new DateTime(2026, 1, 16, 12, 0, 0, 0, DateTimeKind.Utc), true, "an-introduction-to-docker", "An Introduction to Docker", new DateTime(2026, 1, 16, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 17, "# Deploying to Azure App Service\n\nThis is a placeholder post about deploying to azure app service. Real content coming soon — for now it exists to demonstrate pagination.", new DateTime(2026, 1, 17, 12, 0, 0, 0, DateTimeKind.Utc), true, "deploying-to-azure-app-service", "Deploying to Azure App Service", new DateTime(2026, 1, 17, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 18, "# Caching Strategies for Web Apps\n\nThis is a placeholder post about caching strategies for web apps. Real content coming soon — for now it exists to demonstrate pagination.", new DateTime(2026, 1, 18, 12, 0, 0, 0, DateTimeKind.Utc), true, "caching-strategies-for-web-apps", "Caching Strategies for Web Apps", new DateTime(2026, 1, 18, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 19, "# Securing Your REST API\n\nThis is a placeholder post about securing your rest api. Real content coming soon — for now it exists to demonstrate pagination.", new DateTime(2026, 1, 19, 12, 0, 0, 0, DateTimeKind.Utc), true, "securing-your-rest-api", "Securing Your REST API", new DateTime(2026, 1, 19, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 20, "# Unit Testing Fundamentals\n\nThis is a placeholder post about unit testing fundamentals. Real content coming soon — for now it exists to demonstrate pagination.", new DateTime(2026, 1, 20, 12, 0, 0, 0, DateTimeKind.Utc), true, "unit-testing-fundamentals", "Unit Testing Fundamentals", new DateTime(2026, 1, 20, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 21, "# Working with JSON in .NET\n\nThis is a placeholder post about working with json in .net. Real content coming soon — for now it exists to demonstrate pagination.", new DateTime(2026, 1, 21, 12, 0, 0, 0, DateTimeKind.Utc), true, "working-with-json-in-net", "Working with JSON in .NET", new DateTime(2026, 1, 21, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 22, "# Optimizing Frontend Performance\n\nThis is a placeholder post about optimizing frontend performance. Real content coming soon — for now it exists to demonstrate pagination.", new DateTime(2026, 1, 22, 12, 0, 0, 0, DateTimeKind.Utc), true, "optimizing-frontend-performance", "Optimizing Frontend Performance", new DateTime(2026, 1, 22, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 23, "# Clean Code Principles\n\nThis is a placeholder post about clean code principles. Real content coming soon — for now it exists to demonstrate pagination.", new DateTime(2026, 1, 23, 12, 0, 0, 0, DateTimeKind.Utc), true, "clean-code-principles", "Clean Code Principles", new DateTime(2026, 1, 23, 12, 0, 0, 0, DateTimeKind.Utc) },
                    { 24, "# My Favorite Developer Tools\n\nThis is a placeholder post about my favorite developer tools. Real content coming soon — for now it exists to demonstrate pagination.", new DateTime(2026, 1, 24, 12, 0, 0, 0, DateTimeKind.Utc), true, "my-favorite-developer-tools", "My Favorite Developer Tools", new DateTime(2026, 1, 24, 12, 0, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 24);
        }
    }
}
