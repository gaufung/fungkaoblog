import { useEffect, useState } from "react";
import MDEditor from "@uiw/react-md-editor";
import type { Api } from "../api";
import { navigate } from "../router";

interface Props {
  api: Api;
  // When editing an existing post, its slug; undefined when creating.
  slug?: string;
}

export default function Editor({ api, slug }: Props) {
  const [id, setId] = useState<number | null>(null);
  const [title, setTitle] = useState("");
  const [content, setContent] = useState<string>("# New post\n\nWrite in **markdown**…");
  const [published, setPublished] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!slug) return;
    api
      .getPost(slug)
      .then((p) => {
        setId(p.id);
        setTitle(p.title);
        setContent(p.content);
        setPublished(p.published);
      })
      .catch((e) => setError(e.message));
  }, [slug]);

  const save = async () => {
    if (!title.trim()) {
      setError("Title is required.");
      return;
    }
    setSaving(true);
    setError(null);
    try {
      const input = { title, content, published };
      const saved =
        id != null
          ? await api.updatePost(id, input)
          : await api.createPost(input);
      navigate(`/post/${saved.slug}`);
    } catch (e) {
      setError((e as Error).message);
    } finally {
      setSaving(false);
    }
  };

  return (
    <div>
      <div className="page-head">
        <h1>{id != null ? "Edit post" : "New post"}</h1>
      </div>

      {error && <p className="error">{error}</p>}

      <label className="field">
        <span>Title</span>
        <input
          value={title}
          onChange={(e) => setTitle(e.target.value)}
          placeholder="Post title"
        />
      </label>

      <label className="field checkbox">
        <input
          type="checkbox"
          checked={published}
          onChange={(e) => setPublished(e.target.checked)}
        />
        <span>Published</span>
      </label>

      <div className="field" data-color-mode="light">
        <span>Content</span>
        <MDEditor
          value={content}
          onChange={(v) => setContent(v ?? "")}
          height={420}
        />
      </div>

      <div className="actions">
        <button onClick={save} disabled={saving}>
          {saving ? "Saving…" : "Save"}
        </button>
        <button className="secondary" onClick={() => navigate("/")}>
          Cancel
        </button>
      </div>
    </div>
  );
}
