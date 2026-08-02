import { useEffect, useState } from "react";
import type { Api, PostSummary } from "../api";
import { navigate } from "../router";

interface Props {
  api: Api;
  page: number;
  // When set, only posts carrying this tag slug are shown.
  tag?: string;
}

const PAGE_SIZE = 8;

export default function Home({ api, page, tag }: Props) {
  const [posts, setPosts] = useState<PostSummary[]>([]);
  const [total, setTotal] = useState(0);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    setLoading(true);
    setError(null);
    api
      .listPosts(page, tag)
      .then((result) => {
        setPosts(result.items);
        setTotal(result.total);
      })
      .catch((e) => setError(e.message))
      .finally(() => setLoading(false));
  }, [api, page, tag]);

  const basePath = tag ? `/tag/${tag}` : "";
  const goToPage = (p: number) =>
    navigate(p <= 1 ? basePath || "/" : `${basePath}/page/${p}`);

  if (loading) return <p>Loading…</p>;
  if (error) return <p className="error">{error}</p>;

  const totalPages = Math.max(1, Math.ceil(total / PAGE_SIZE));

  // The display name for a tag slug is taken from the loaded posts.
  const tagName = tag
    ? posts.find((p) => p.tags.some((t) => t.slug === tag))?.tags.find(
        (t) => t.slug === tag
      )?.name ?? tag
    : null;

  return (
    <div>
      <div className="page-head">
        <h1>{tagName ? `Posts tagged “${tagName}”` : "Posts"}</h1>
      </div>

      {tag && (
        <p className="meta">
          <a href="#/">← All posts</a>
        </p>
      )}

      {posts.length === 0 && <p>No posts yet.</p>}

      <ul className="post-list">
        {posts.map((p) => (
          <li key={p.id}>
            <a href={`#/post/${p.slug}`} className="post-title">
              {p.title}
            </a>
            <div className="meta">
              {new Date(p.createdAt).toLocaleDateString()}
            </div>
            {p.tags.length > 0 && (
              <div className="tags">
                {p.tags.map((t) => (
                  <a key={t.slug} className="tag" href={`#/tag/${t.slug}`}>
                    {t.name}
                  </a>
                ))}
              </div>
            )}
          </li>
        ))}
      </ul>

      {totalPages > 1 && (
        <nav className="pagination" aria-label="Pagination">
          <button
            className="secondary"
            onClick={() => goToPage(page - 1)}
            disabled={page <= 1}
          >
            ← Prev
          </button>

          {Array.from({ length: totalPages }, (_, i) => i + 1).map((p) => (
            <button
              key={p}
              className={p === page ? "page current" : "page"}
              aria-current={p === page ? "page" : undefined}
              onClick={() => goToPage(p)}
            >
              {p}
            </button>
          ))}

          <button
            className="secondary"
            onClick={() => goToPage(page + 1)}
            disabled={page >= totalPages}
          >
            Next →
          </button>
        </nav>
      )}
    </div>
  );
}
