import { useEffect, useState } from "react";

export type Route =
  | { name: "home"; page: number }
  | { name: "tag"; slug: string; page: number }
  | { name: "post"; slug: string };

function pageFrom(parts: string[], start: number): number {
  if (parts[start] === "page" && parts[start + 1]) {
    const page = Number.parseInt(parts[start + 1], 10);
    return Number.isNaN(page) || page < 1 ? 1 : page;
  }
  return 1;
}

function parse(hash: string): Route {
  const path = hash.replace(/^#/, "") || "/";
  const parts = path.split("/").filter(Boolean);

  if (parts.length === 0) return { name: "home", page: 1 };
  if (parts[0] === "page" && parts[1]) return { name: "home", page: pageFrom(parts, 0) };
  if (parts[0] === "tag" && parts[1]) {
    return { name: "tag", slug: decodeURIComponent(parts[1]), page: pageFrom(parts, 2) };
  }
  if (parts[0] === "post" && parts[1]) return { name: "post", slug: decodeURIComponent(parts[1]) };
  return { name: "home", page: 1 };
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
