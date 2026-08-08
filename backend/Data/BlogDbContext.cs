using System.Text.RegularExpressions;
using Blog.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Blog.Api.Data;

public class BlogDbContext : DbContext
{
    public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
    {
    }

    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Tag> Tags => Set<Tag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Post>()
            .HasIndex(p => p.Slug)
            .IsUnique();

        modelBuilder.Entity<Post>()
            .HasIndex(p => p.Title)
            .IsUnique();

        modelBuilder.Entity<Post>()
            .HasIndex(p => p.SourceNumber)
            .IsUnique();

        modelBuilder.Entity<Tag>()
            .HasIndex(t => t.Slug)
            .IsUnique();

        SeedPosts(modelBuilder);
        SeedTags(modelBuilder);

        modelBuilder.Entity<Post>()
            .HasMany(p => p.Tags)
            .WithMany(t => t.Posts)
            .UsingEntity(join => join.HasData(BuildPostTagLinks()));
    }

    // Dummy content so a fresh database isn't empty. Values are static (not
    // DateTime.UtcNow) so migrations stay deterministic.
    private static void SeedPosts(ModelBuilder modelBuilder)
    {
        var seededAt = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        modelBuilder.Entity<Post>().HasData(
            new Post
            {
                Id = 1,
                Title = "Welcome to My Blog",
                Slug = "welcome-to-my-blog",
                Content = "# Welcome\n\nThis is the very first post on this deliberately small blog. " +
                          "It's built with **ASP.NET Core** on the backend and **React + TypeScript** on the front end.\n\n" +
                          "Feel free to look around!",
                Published = true,
                CreatedAt = seededAt,
                UpdatedAt = seededAt
            },
            new Post
            {
                Id = 2,
                Title = "Getting Started with Markdown",
                Slug = "getting-started-with-markdown",
                Content = "# Markdown 101\n\nPosts here are authored in Markdown. A few basics:\n\n" +
                          "- **Bold** and _italic_ text\n" +
                          "- Lists, like this one\n" +
                          "- `inline code` and fenced code blocks\n\n" +
                          "```csharp\nConsole.WriteLine(\"Hello, blog!\");\n```\n",
                Published = true,
                CreatedAt = seededAt.AddDays(1),
                UpdatedAt = seededAt.AddDays(1)
            },
            new Post
            {
                Id = 3,
                Title = "Why I Built This Blog",
                Slug = "why-i-built-this-blog",
                Content = "# Motivation\n\nI wanted a minimal place to write, without a heavyweight CMS. " +
                          "This project pairs a small Web API with Azure SQL storage and Entra ID sign-in " +
                          "so only I can publish, while everyone can read.",
                Published = true,
                CreatedAt = seededAt.AddDays(2),
                UpdatedAt = seededAt.AddDays(2)
            },
            new Post
            {
                Id = 4,
                Title = "Draft: Ideas for Upcoming Posts",
                Slug = "draft-ideas-for-upcoming-posts",
                Content = "# Backlog\n\nA scratchpad of things I might write about:\n\n" +
                          "1. Deploying to Azure\n" +
                          "2. EF Core migrations tips\n" +
                          "3. Securing an API with app roles\n\n" +
                          "_This is an unpublished draft._",
                Published = false,
                CreatedAt = seededAt.AddDays(3),
                UpdatedAt = seededAt.AddDays(3)
            }
        );

        modelBuilder.Entity<Post>().HasData(BuildExtraPosts(seededAt));
        modelBuilder.Entity<Post>().HasData(BuildChinesePosts(seededAt));
    }

    // Posts authored in Chinese, to exercise Unicode handling end to end.
    private static Post[] BuildChinesePosts(DateTime seededAt)
    {
        return new[]
        {
            new Post
            {
                Id = 25,
                Title = "你好，世界：中文博客测试",
                Slug = "chinese-hello-world",
                Content = "# 你好，世界！\n\n" +
                          "这是一篇用于测试**中文字符兼容性**的文章。如果你能正常阅读这段文字，" +
                          "说明数据库、接口和前端都能正确处理 UTF-8 编码。\n\n" +
                          "## 常见标点\n\n" +
                          "逗号，句号。感叹号！问号？分号；冒号：引号「你好」『世界』，还有省略号……\n\n" +
                          "## 列表\n\n" +
                          "- 苹果 🍎\n" +
                          "- 香蕉 🍌\n" +
                          "- 西瓜 🍉\n\n" +
                          "## 代码\n\n" +
                          "```csharp\nConsole.WriteLine(\"你好，世界\");\n```\n\n" +
                          "> 引用：路漫漫其修远兮，吾将上下而求索。\n",
                Published = true,
                CreatedAt = seededAt.AddDays(24),
                UpdatedAt = seededAt.AddDays(24)
            },
            new Post
            {
                Id = 26,
                Title = "前端开发笔记：React 与 TypeScript",
                Slug = "frontend-notes-react-typescript",
                Content = "# 前端开发笔记\n\n" +
                          "在现代前端项目中，**React** 搭配 **TypeScript** 已经成为主流选择。" +
                          "类型系统能在编译期发现许多潜在的错误。\n\n" +
                          "## 一个简单的组件\n\n" +
                          "```tsx\ninterface Props {\n  名称: string;\n}\n\n" +
                          "export function 问候({ 名称 }: Props) {\n" +
                          "  return <p>你好，{名称}！</p>;\n}\n```\n\n" +
                          "## 小结\n\n" +
                          "1. 组件应保持单一职责。\n" +
                          "2. 尽量复用逻辑。\n" +
                          "3. 为公共接口编写类型。\n",
                Published = true,
                CreatedAt = seededAt.AddDays(25),
                UpdatedAt = seededAt.AddDays(25)
            },
            new Post
            {
                Id = 27,
                Title = "深入理解 .NET 与 C# 的异步编程",
                Slug = "dotnet-csharp-async",
                Content = "# 异步编程\n\n" +
                          "在 .NET 中，`async` 和 `await` 让编写非阻塞代码变得简单直观。\n\n" +
                          "| 关键字 | 含义 |\n" +
                          "| --- | --- |\n" +
                          "| `async` | 标记一个异步方法 |\n" +
                          "| `await` | 等待异步操作完成 |\n\n" +
                          "```csharp\npublic async Task<string> 获取数据Async()\n{\n" +
                          "    await Task.Delay(100);\n" +
                          "    return \"完成\";\n}\n```\n\n" +
                          "**注意**：不要在异步方法中使用 `.Result`，否则可能导致死锁。\n",
                Published = true,
                CreatedAt = seededAt.AddDays(26),
                UpdatedAt = seededAt.AddDays(26)
            }
        };
    }

    // Additional published posts so pagination has enough content to page through.
    private static Post[] BuildExtraPosts(DateTime seededAt)
    {
        var titles = new[]
        {
            "Understanding Async and Await in C#",
            "A Practical Tour of LINQ",
            "Dependency Injection Explained",
            "Entity Framework Core Migrations Tips",
            "Building Minimal APIs in ASP.NET Core",
            "React Hooks You Should Know",
            "TypeScript Generics Made Simple",
            "State Management Without a Library",
            "Styling with Modern CSS",
            "Debugging Like a Pro",
            "Writing Better Git Commit Messages",
            "An Introduction to Docker",
            "Deploying to Azure App Service",
            "Caching Strategies for Web Apps",
            "Securing Your REST API",
            "Unit Testing Fundamentals",
            "Working with JSON in .NET",
            "Optimizing Frontend Performance",
            "Clean Code Principles",
            "My Favorite Developer Tools"
        };

        return titles.Select((title, index) =>
        {
            // Ids 1-4 are the explicit posts above; continue from 5. Days 0-3 are
            // taken, so these start at day 4.
            var day = index + 4;
            return new Post
            {
                Id = index + 5,
                Title = title,
                Slug = Slugify(title),
                Content = $"# {title}\n\nThis is a placeholder post about {title.ToLowerInvariant()}. " +
                          "Real content coming soon — for now it exists to demonstrate pagination.",
                Published = true,
                CreatedAt = seededAt.AddDays(day),
                UpdatedAt = seededAt.AddDays(day)
            };
        }).ToArray();
    }

    private static string Slugify(string value)
    {
        value = value.Trim().ToLowerInvariant();
        value = Regex.Replace(value, @"[^a-z0-9\s-]", string.Empty);
        value = Regex.Replace(value, @"[\s-]+", "-").Trim('-');
        return value;
    }

    // (Id, Name, Slug) — names like "C#"/".NET" don't slugify cleanly, so slugs
    // are given explicitly.
    private static readonly (int Id, string Name, string Slug)[] TagDefinitions =
    {
        (1, ".NET", "dotnet"),
        (2, "C#", "csharp"),
        (3, "ASP.NET Core", "aspnet-core"),
        (4, "React", "react"),
        (5, "TypeScript", "typescript"),
        (6, "CSS", "css"),
        (7, "Azure", "azure"),
        (8, "DevOps", "devops"),
        (9, "Testing", "testing"),
        (10, "Performance", "performance"),
        (11, "Best Practices", "best-practices"),
        (12, "Git", "git"),
        (13, "Docker", "docker"),
        (14, "Database", "database"),
        (15, "Tutorial", "tutorial"),
        (16, "教程", "jiaocheng"),
        (17, "前端", "qianduan")
    };

    private static void SeedTags(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tag>().HasData(
            TagDefinitions.Select(t => new Tag { Id = t.Id, Name = t.Name, Slug = t.Slug }).ToArray());
    }

    // Deterministically assign tags to the seeded posts. A fixed RNG seed keeps
    // the generated join rows identical across migration scaffolding and runtime,
    // so the model never drifts.
    private static object[] BuildPostTagLinks()
    {
        var rng = new Random(20260102);
        var links = new List<object>();

        void Link(int postId, params int[] tagIds)
        {
            foreach (var tagId in tagIds)
            {
                links.Add(new { PostsId = postId, TagsId = tagId });
            }
        }

        // Original 24 posts: 1-3 random tags. The pool is fixed at the first 15
        // tags so that adding new tags later never reshuffles these assignments.
        const int randomPostCount = 24;
        const int randomTagPool = 15;
        for (var postId = 1; postId <= randomPostCount; postId++)
        {
            var count = rng.Next(1, 4);
            var chosen = new SortedSet<int>();
            while (chosen.Count < count)
            {
                chosen.Add(rng.Next(1, randomTagPool + 1));
            }

            Link(postId, chosen.ToArray());
        }

        // Chinese posts (25-27): explicit tags, including the Chinese-named tags.
        Link(25, 16, 15);      // 教程, Tutorial
        Link(26, 17, 4, 5);    // 前端, React, TypeScript
        Link(27, 16, 1, 2);    // 教程, .NET, C#

        return links.ToArray();
    }
}
