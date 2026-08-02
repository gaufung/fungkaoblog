import { useMemo } from "react";
import { useMsal } from "@azure/msal-react";
import { createApi, isSuperAdmin } from "./api";
import { loginRequest } from "./authConfig";
import { useRoute, navigate } from "./router";
import Home from "./pages/Home";
import PostView from "./pages/PostView";
import Editor from "./pages/Editor";
import "./App.css";

export default function App() {
  const { instance, accounts } = useMsal();
  const account = accounts[0] ?? null;
  const admin = isSuperAdmin(account);
  const route = useRoute();

  const api = useMemo(
    () =>
      createApi(
        instance,
        () => instance.getActiveAccount() ?? accounts[0] ?? null
      ),
    [instance, accounts]
  );

  const signIn = () => instance.loginPopup(loginRequest).catch(console.error);
  const signOut = () => instance.logoutPopup().catch(console.error);

  const renderRoute = () => {
    switch (route.name) {
      case "post":
        return <PostView api={api} slug={route.slug} isAdmin={admin} />;
      case "new":
        return admin ? <Editor api={api} /> : <NotAllowed />;
      case "edit":
        return admin ? <Editor api={api} slug={route.slug} /> : <NotAllowed />;
      case "home":
      default:
        return <Home api={api} isAdmin={admin} />;
    }
  };

  return (
    <div className="app">
      <header className="topbar">
        <a className="brand" href="#/" onClick={() => navigate("/")}>
          My Blog
        </a>
        <div className="auth">
          {account ? (
            <>
              <span className="who">
                {account.name ?? account.username}
                {admin && <span className="badge admin">SuperAdmin</span>}
              </span>
              <button className="secondary" onClick={signOut}>
                Sign out
              </button>
            </>
          ) : (
            <button onClick={signIn}>Sign in</button>
          )}
        </div>
      </header>

      <main className="content">{renderRoute()}</main>
    </div>
  );
}

function NotAllowed() {
  return (
    <div>
      <h1>Not allowed</h1>
      <p>You need the SuperAdmin role to edit content.</p>
      <a href="#/">Back to posts</a>
    </div>
  );
}
