import { useEffect, useMemo, useState } from "react";
import MDEditor from "@uiw/react-md-editor";
import type { Api, Post } from "../api";

const CODE_LANGUAGE_ALIASES: Record<string, string> = {
  "c#": "csharp",
  cs: "csharp",
  "c-sharp": "csharp",
  dotnet: "csharp",
  js: "javascript",
  ts: "typescript",
  sh: "bash",
  shell: "bash",
  yml: "yaml",
  md: "markdown",
};

const CODE_LANGUAGE_LABELS: Record<string, string> = {
  csharp: "C#",
  javascript: "JavaScript",
  typescript: "TypeScript",
  bash: "Shell",
  yaml: "YAML",
  markdown: "Markdown",
};

function normalizeCodeFenceLanguages(markdown: string) {
  return markdown.replace(
    /^([ \t]*)(`{3,}|~{3,})[ \t]*([^\s`~]+)([^\r\n]*)$/gm,
    (_, indentation: string, fence: string, language: string, metadata: string) => {
      const normalizedLanguage =
        CODE_LANGUAGE_ALIASES[language.toLowerCase()] ?? language.toLowerCase();
      return `${indentation}${fence}${normalizedLanguage}${metadata}`;
    }
  );
}

interface Props {
  api: Api;
  slug: string;
}

export default function PostView({ api, slug }: Props) {
  const [post, setPost] = useState<Post | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    api
      .getPost(slug)
      .then(setPost)
      .catch((e) => setError(e.message));
  }, [api, slug]);

  const markdown = useMemo(
    () => normalizeCodeFenceLanguages(post?.content ?? ""),
    [post?.content]
  );

  if (error) return <p className="error">{error}</p>;
  if (!post) return <p>Loading…</p>;

  return (
    <article className="post-page">
      <a className="back-link" href="#/">
        ← All posts
      </a>

      <header className="post-header">
        <h1>{post.title}</h1>
        <p className="meta">{new Date(post.createdAt).toLocaleString()}</p>
        {post.tags.length > 0 && (
          <div className="tags">
            {post.tags.map((t) => (
              <a key={t.slug} className="tag" href={`#/tag/${t.slug}`}>
                {t.name}
              </a>
            ))}
          </div>
        )}
      </header>

      <div className="post-content" data-color-mode="light">
        <MDEditor.Markdown
          source={markdown}
          rehypeRewrite={(node) => {
            if (node.type !== "element" || node.tagName !== "pre") return;

            const code = node.children.find(
              (child) => child.type === "element" && child.tagName === "code"
            );
            if (code?.type !== "element") return;

            const classNames = code.properties.className;
            if (!Array.isArray(classNames)) return;

            const languageClass = classNames
              .map(String)
              .find((className) => className.startsWith("language-"));
            if (!languageClass) return;

            const language = languageClass.slice("language-".length);
            node.properties["data-language"] =
              CODE_LANGUAGE_LABELS[language] ?? language.toUpperCase();
          }}
        />
      </div>
    </article>
  );
}
