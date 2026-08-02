import { useEffect, useState } from "react";
import MDEditor from "@uiw/react-md-editor";
import type { Api, Post } from "../api";

interface Props {
  api: Api;
  slug: string;
  isAdmin: boolean;
}

export default function PostView({ api, slug, isAdmin }: Props) {
  const [post, setPost] = useState<Post | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    api
      .getPost(slug)
      .then(setPost)
      .catch((e) => setError(e.message));
  }, [slug]);

  if (error) return <p className="error">{error}</p>;
  if (!post) return <p>Loading…</p>;

  return (
    <article>
      <div className="page-head">
        <h1>{post.title}</h1>
        {isAdmin && <a href={`#/edit/${post.slug}`}>edit</a>}
      </div>
      <p className="meta">{new Date(post.createdAt).toLocaleString()}</p>
      <div data-color-mode="light">
        <MDEditor.Markdown source={post.content} />
      </div>
    </article>
  );
}
