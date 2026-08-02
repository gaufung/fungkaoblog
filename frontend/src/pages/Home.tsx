import { useEffect, useState } from "react";
import type { Api, PostSummary } from "../api";
import { navigate } from "../router";

interface Props {
  api: Api;
  isAdmin: boolean;
}

export default function Home({ api, isAdmin }: Props) {
  const [posts, setPosts] = useState<PostSummary[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  const load = () => {
    setLoading(true);
    api
      .listPosts(isAdmin)
      .then(setPosts)
      .catch((e) => setError(e.message))
      .finally(() => setLoading(false));
  };

  useEffect(load, [isAdmin]);

  const onDelete = async (id: number) => {
    if (!confirm("Delete this post?")) return;
    try {
      await api.deletePost(id);
      load();
    } catch (e) {
      alert((e as Error).message);
    }
  };

  if (loading) return <p>Loading…</p>;
  if (error) return <p className="error">{error}</p>;

  return (
    <div>
      <div className="page-head">
        <h1>Posts</h1>
        {isAdmin && (
          <button onClick={() => navigate("/new")}>+ New post</button>
        )}
      </div>

      {posts.length === 0 && <p>No posts yet.</p>}

      <ul className="post-list">
        {posts.map((p) => (
          <li key={p.id}>
            <a href={`#/post/${p.slug}`} className="post-title">
              {p.title}
            </a>
            {!p.published && <span className="badge">draft</span>}
            <div className="meta">
              {new Date(p.createdAt).toLocaleDateString()}
              {isAdmin && (
                <>
                  {" · "}
                  <a href={`#/edit/${p.slug}`}>edit</a>
                  {" · "}
                  <button className="link" onClick={() => onDelete(p.id)}>
                    delete
                  </button>
                </>
              )}
            </div>
          </li>
        ))}
      </ul>
    </div>
  );
}
