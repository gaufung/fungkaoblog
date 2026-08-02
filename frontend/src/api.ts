// The backend serves this SPA, so the API lives on the same origin by default.
// An explicit VITE_API_BASE_URL still overrides it (e.g. for `vite dev`).
const apiBaseUrl = (import.meta.env.VITE_API_BASE_URL as string | undefined) ?? "";

export interface Tag {
  name: string;
  slug: string;
}

export interface PostSummary {
  id: number;
  title: string;
  slug: string;
  createdAt: string;
  updatedAt: string;
  published: boolean;
  tags: Tag[];
}

export interface Post extends PostSummary {
  content: string;
}

export interface Paged<T> {
  items: T[];
  total: number;
  page: number;
  pageSize: number;
}

async function handle<T>(res: Response): Promise<T> {
  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || `Request failed with status ${res.status}`);
  }
  if (res.status === 204) return undefined as T;
  return (await res.json()) as T;
}

export function createApi() {
  const base = apiBaseUrl.replace(/\/$/, "");

  return {
    async listPosts(page = 1, tag?: string): Promise<Paged<PostSummary>> {
      const params = new URLSearchParams({ page: String(page) });
      if (tag) params.set("tag", tag);
      const res = await fetch(`${base}/api/posts?${params.toString()}`);
      return handle<Paged<PostSummary>>(res);
    },

    async getPost(slug: string): Promise<Post> {
      const res = await fetch(`${base}/api/posts/${encodeURIComponent(slug)}`);
      return handle<Post>(res);
    },
  };
}

export type Api = ReturnType<typeof createApi>;
