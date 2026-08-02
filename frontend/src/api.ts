import {
  type IPublicClientApplication,
  type AccountInfo,
} from "@azure/msal-browser";
import { apiBaseUrl, loginRequest, SUPER_ADMIN_ROLE } from "./authConfig";

export interface PostSummary {
  id: number;
  title: string;
  slug: string;
  createdAt: string;
  updatedAt: string;
  published: boolean;
}

export interface Post extends PostSummary {
  content: string;
}

export interface PostInput {
  title: string;
  slug?: string;
  content: string;
  published: boolean;
}

// Returns true when the signed-in account holds the SuperAdmin app role.
export function isSuperAdmin(account: AccountInfo | null): boolean {
  if (!account) return false;
  const roles = (account.idTokenClaims as { roles?: string[] } | undefined)?.roles ?? [];
  return roles.includes(SUPER_ADMIN_ROLE);
}

async function authHeader(
  msal: IPublicClientApplication,
  account: AccountInfo | null
): Promise<Record<string, string>> {
  if (!account) return {};
  try {
    const result = await msal.acquireTokenSilent({ ...loginRequest, account });
    return { Authorization: `Bearer ${result.accessToken}` };
  } catch {
    return {};
  }
}

async function handle<T>(res: Response): Promise<T> {
  if (!res.ok) {
    const text = await res.text();
    throw new Error(text || `Request failed with status ${res.status}`);
  }
  if (res.status === 204) return undefined as T;
  return (await res.json()) as T;
}

export function createApi(
  msal: IPublicClientApplication,
  getAccount: () => AccountInfo | null
) {
  const base = apiBaseUrl.replace(/\/$/, "");

  return {
    async listPosts(includeDrafts = false): Promise<PostSummary[]> {
      const headers = await authHeader(msal, getAccount());
      const res = await fetch(
        `${base}/api/posts?includeDrafts=${includeDrafts}`,
        { headers }
      );
      return handle<PostSummary[]>(res);
    },

    async getPost(slug: string): Promise<Post> {
      const headers = await authHeader(msal, getAccount());
      const res = await fetch(`${base}/api/posts/${encodeURIComponent(slug)}`, {
        headers,
      });
      return handle<Post>(res);
    },

    async createPost(input: PostInput): Promise<Post> {
      const headers = await authHeader(msal, getAccount());
      const res = await fetch(`${base}/api/posts`, {
        method: "POST",
        headers: { ...headers, "Content-Type": "application/json" },
        body: JSON.stringify(input),
      });
      return handle<Post>(res);
    },

    async updatePost(id: number, input: PostInput): Promise<Post> {
      const headers = await authHeader(msal, getAccount());
      const res = await fetch(`${base}/api/posts/${id}`, {
        method: "PUT",
        headers: { ...headers, "Content-Type": "application/json" },
        body: JSON.stringify(input),
      });
      return handle<Post>(res);
    },

    async deletePost(id: number): Promise<void> {
      const headers = await authHeader(msal, getAccount());
      const res = await fetch(`${base}/api/posts/${id}`, {
        method: "DELETE",
        headers,
      });
      return handle<void>(res);
    },
  };
}

export type Api = ReturnType<typeof createApi>;
