import { useEffect, useState } from "react";
import MDEditor from "@uiw/react-md-editor";
import type { Api, Post } from "../api";

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

  if (error) return <p className="error">{error}</p>;
  if (!post) return <p>Loading…</p>;

  return (
    <article>
      <div className="page-head">
        <h1>{post.title}</h1>
      </div>
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
      <div data-color-mode="light">
        <MDEditor.Markdown source={post.content} />
      </div>
    </article>
  );
}
