import type { Configuration } from "@azure/msal-browser";

const tenantId = import.meta.env.VITE_AAD_TENANT_ID as string;
const clientId = import.meta.env.VITE_AAD_CLIENT_ID as string;

export const apiScope = import.meta.env.VITE_API_SCOPE as string;
export const apiBaseUrl = import.meta.env.VITE_API_BASE_URL as string;

export const msalConfig: Configuration = {
  auth: {
    clientId,
    authority: `https://login.microsoftonline.com/${tenantId}`,
    redirectUri: window.location.origin,
  },
  cache: {
    cacheLocation: "localStorage",
  },
};

// Scopes requested when signing in / acquiring an API token.
export const loginRequest = {
  scopes: [apiScope],
};

export const SUPER_ADMIN_ROLE = "SuperAdmin";
