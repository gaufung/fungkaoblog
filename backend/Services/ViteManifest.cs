using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blog.Api.Services;

// Reads the manifest produced by `vite build` (build.manifest = true) so a
// Razor view can reference the content-hashed JS/CSS filenames of the built
// SPA. See https://vite.dev/guide/backend-integration.
public class ViteManifest
{
    private const string EntryKey = "index.html";

    private readonly IWebHostEnvironment _env;
    private readonly ILogger<ViteManifest> _logger;

    private Dictionary<string, ManifestChunk>? _cache;
    private DateTime _cacheStamp;

    public ViteManifest(IWebHostEnvironment env, ILogger<ViteManifest> logger)
    {
        _env = env;
        _logger = logger;
    }

    private string ManifestPath =>
        Path.Combine(_env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot"),
            ".vite", "manifest.json");

    // The JS bundle (as a root-relative URL) for the SPA entry point.
    public string? EntryScript()
    {
        var manifest = Load();
        return manifest is not null && manifest.TryGetValue(EntryKey, out var chunk)
            ? "/" + chunk.File
            : null;
    }

    // All stylesheet URLs (root-relative) the SPA entry depends on, including
    // those pulled in by imported chunks.
    public IReadOnlyList<string> EntryStyles()
    {
        var manifest = Load();
        if (manifest is null || !manifest.TryGetValue(EntryKey, out var entry))
        {
            return Array.Empty<string>();
        }

        var styles = new List<string>();
        var seen = new HashSet<string>();
        Collect(manifest, entry, styles, seen);
        return styles;
    }

    private static void Collect(
        Dictionary<string, ManifestChunk> manifest,
        ManifestChunk chunk,
        List<string> styles,
        HashSet<string> seen)
    {
        foreach (var css in chunk.Css)
        {
            var url = "/" + css;
            if (seen.Add(url))
            {
                styles.Add(url);
            }
        }

        foreach (var import in chunk.Imports)
        {
            if (manifest.TryGetValue(import, out var imported))
            {
                Collect(manifest, imported, styles, seen);
            }
        }
    }

    private Dictionary<string, ManifestChunk>? Load()
    {
        var path = ManifestPath;
        if (!File.Exists(path))
        {
            _logger.LogWarning(
                "Vite manifest not found at {Path}. Run `npm run build` in ./frontend.", path);
            return null;
        }

        // Cache the parsed manifest, but re-read in Development so a fresh
        // `npm run build` is picked up without restarting the server.
        var stamp = File.GetLastWriteTimeUtc(path);
        if (_cache is not null)
        {
            if (!_env.IsDevelopment())
            {
                return _cache;
            }
            if (stamp == _cacheStamp)
            {
                return _cache;
            }
        }

        var json = File.ReadAllText(path);
        _cache = JsonSerializer.Deserialize<Dictionary<string, ManifestChunk>>(json);
        _cacheStamp = stamp;
        return _cache;
    }

    private sealed class ManifestChunk
    {
        [JsonPropertyName("file")]
        public string File { get; set; } = string.Empty;

        [JsonPropertyName("css")]
        public List<string> Css { get; set; } = new();

        [JsonPropertyName("imports")]
        public List<string> Imports { get; set; } = new();
    }
}
