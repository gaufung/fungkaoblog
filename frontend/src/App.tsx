import { useMemo } from "react";
import { createApi } from "./api";
import { useRoute, navigate } from "./router";
import Home from "./pages/Home";
import PostView from "./pages/PostView";
import "./App.css";

export default function App() {
  const route = useRoute();

  const api = useMemo(() => createApi(), []);

  const renderRoute = () => {
    switch (route.name) {
      case "post":
        return <PostView api={api} slug={route.slug} />;
      case "tag":
        return <Home api={api} page={route.page} tag={route.slug} />;
      case "home":
      default:
        return <Home api={api} page={route.name === "home" ? route.page : 1} />;
    }
  };

  return (
    <div className="app">
      <header className="topbar">
        <a className="brand" href="#/" onClick={() => navigate("/")}>
          Fung Kao's Blog
        </a>
        <nav className="social" aria-label="Social links">
          <a
            className="social-link"
            href="https://github.com/gaufung"
            target="_blank"
            rel="noopener noreferrer"
            aria-label="GitHub profile"
          >
            <svg viewBox="0 0 16 16" width="24" height="24" aria-hidden="true">
              <path
                fill="currentColor"
                d="M8 0C3.58 0 0 3.58 0 8c0 3.54 2.29 6.53 5.47 7.59.4.07.55-.17.55-.38 0-.19-.01-.82-.01-1.49-2.01.37-2.53-.49-2.69-.94-.09-.23-.48-.94-.82-1.13-.28-.15-.68-.52-.01-.53.63-.01 1.08.58 1.23.82.72 1.21 1.87.87 2.33.66.07-.52.28-.87.51-1.07-1.78-.2-3.64-.89-3.64-3.95 0-.87.31-1.59.82-2.15-.08-.2-.36-1.02.08-2.12 0 0 .67-.21 2.2.82.64-.18 1.32-.27 2-.27.68 0 1.36.09 2 .27 1.53-1.04 2.2-.82 2.2-.82.44 1.1.16 1.92.08 2.12.51.56.82 1.27.82 2.15 0 3.07-1.87 3.75-3.65 3.95.29.25.54.73.54 1.48 0 1.07-.01 1.93-.01 2.2 0 .21.15.46.55.38A8.01 8.01 0 0 0 16 8c0-4.42-3.58-8-8-8z"
              />
            </svg>
          </a>
          <a
            className="social-link"
            href="https://x.com/gaufung"
            target="_blank"
            rel="noopener noreferrer"
            aria-label="X profile"
          >
            <svg viewBox="0 0 24 24" width="22" height="22" aria-hidden="true">
              <path
                fill="currentColor"
                d="M18.244 2.25h3.308l-7.227 8.26 8.502 11.24H16.17l-5.214-6.817L4.99 21.75H1.68l7.73-8.835L1.254 2.25H8.08l4.713 6.231zm-1.161 17.52h1.833L7.084 4.126H5.117z"
              />
            </svg>
          </a>
          <a
            className="social-link"
            href="mailto:fungkao92@gmail.com"
            aria-label="Email Fung Kao"
          >
            <svg viewBox="0 0 24 24" width="23" height="23" aria-hidden="true">
              <path
                fill="currentColor"
                d="M3.75 4.5h16.5A2.75 2.75 0 0 1 23 7.25v9.5a2.75 2.75 0 0 1-2.75 2.75H3.75A2.75 2.75 0 0 1 1 16.75v-9.5A2.75 2.75 0 0 1 3.75 4.5Zm0 2a.75.75 0 0 0-.75.75v.386l9 5.4 9-5.4V7.25a.75.75 0 0 0-.75-.75H3.75ZM21 9.968l-8.485 5.09a1 1 0 0 1-1.03 0L3 9.968v6.782c0 .414.336.75.75.75h16.5a.75.75 0 0 0 .75-.75V9.968Z"
              />
            </svg>
          </a>
          <a
            className="social-link rss-link"
            href="/rss.xml"
            target="_blank"
            rel="noopener noreferrer"
            aria-label="RSS feed"
          >
            <svg viewBox="0 0 24 24" width="22" height="22" aria-hidden="true">
              <path
                fill="currentColor"
                d="M5.5 3.5a1.5 1.5 0 0 0 0 3c6.617 0 12 5.383 12 12a1.5 1.5 0 0 0 3 0c0-8.271-6.729-15-15-15Zm0 6a1.5 1.5 0 0 0 0 3 6 6 0 0 1 6 6 1.5 1.5 0 0 0 3 0 9 9 0 0 0-9-9ZM7.75 18.5A2.25 2.25 0 1 1 3.25 18.5a2.25 2.25 0 0 1 4.5 0Z"
              />
            </svg>
          </a>
        </nav>
      </header>

      <main className="content">{renderRoute()}</main>
    </div>
  );
}
