import { useEffect, useState } from "react";

export type Route =
  | { name: "home" }
  | { name: "post"; slug: string }
  | { name: "new" }
  | { name: "edit"; slug: string };

function parse(hash: string): Route {
  const path = hash.replace(/^#/, "") || "/";
  const parts = path.split("/").filter(Boolean);

  if (parts.length === 0) return { name: "home" };
  if (parts[0] === "new") return { name: "new" };
  if (parts[0] === "post" && parts[1]) return { name: "post", slug: decodeURIComponent(parts[1]) };
  if (parts[0] === "edit" && parts[1]) return { name: "edit", slug: decodeURIComponent(parts[1]) };
  return { name: "home" };
}

export function navigate(to: string): void {
  window.location.hash = to;
}

export function useRoute(): Route {
  const [route, setRoute] = useState<Route>(() => parse(window.location.hash));

  useEffect(() => {
    const onChange = () => setRoute(parse(window.location.hash));
    window.addEventListener("hashchange", onChange);
    return () => window.removeEventListener("hashchange", onChange);
  }, []);

  return route;
}
