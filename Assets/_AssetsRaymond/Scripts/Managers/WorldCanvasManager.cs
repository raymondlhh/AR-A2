using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vuplex.WebView;

public class WorldCanvasManager : MonoBehaviour
{
    [System.Serializable]
    public class SocialMediaEntry
    {
        public string label;
        public Texture2D texture;
    }

    [System.Serializable]
    public class GameEntry
    {
        public string label;
        public Texture2D texture;
        public string url;
    }

    [Header("Social Media (Label, Texture)")]
    [SerializeField] private List<SocialMediaEntry> socialMedia = new List<SocialMediaEntry>();

    [Header("Game Development (Label, Texture, Link)")] 
    [SerializeField] private List<GameEntry> games = new List<GameEntry>();

    [Header("Canvas WebViews (Left / Middle / Right)")]
    [SerializeField] private CanvasWebViewPrefab leftCanvasWebViewPrefab;
    [SerializeField] private CanvasWebViewPrefab middleCanvasWebViewPrefab;
    [SerializeField] private CanvasWebViewPrefab rightCanvasWebViewPrefab;

    public static WorldCanvasManager Instance { get; private set; }

    private readonly Dictionary<string, string> labelToUrl = new Dictionary<string, string>();
    private readonly Dictionary<string, Sprite> labelToSprite = new Dictionary<string, Sprite>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        InitializeDefaultsIfEmpty();
        BuildLookup();
    }

    private void InitializeDefaultsIfEmpty()
    {
        if (socialMedia.Count == 0)
        {
            socialMedia.Add(new SocialMediaEntry { label = "Itch.io", texture = null });
            socialMedia.Add(new SocialMediaEntry { label = "LinkedIn", texture = null });
            socialMedia.Add(new SocialMediaEntry { label = "YouTube", texture = null });
        }

        if (games.Count == 0)
        {
            games.Add(new GameEntry { label = "Merlin's Curse", texture = null, url = "https://raymondlhh.itch.io/merlins-curse" });
            games.Add(new GameEntry { label = "The Debuggers", texture = null, url = "https://raymondlhh.itch.io/the-debuggers" });
            games.Add(new GameEntry { label = "CyberRun", texture = null, url = "https://raymondlhh.itch.io/cyber-run" });
        }
    }

    private void BuildLookup()
    {
        labelToUrl.Clear();
        for (int i = 0; i < games.Count; i++)
        {
            GameEntry entry = games[i];
            if (string.IsNullOrEmpty(entry.label)) { continue; }
            if (!labelToUrl.ContainsKey(entry.label))
            {
                labelToUrl.Add(entry.label, entry.url);
            }
            if (!labelToSprite.ContainsKey(entry.label) && entry.texture != null)
            {
                // Convert Texture2D to Sprite for UI Image usage. Cache to avoid recreating.
                labelToSprite[entry.label] = Sprite.Create(
                    entry.texture,
                    new Rect(0, 0, entry.texture.width, entry.texture.height),
                    new Vector2(0.5f, 0.5f)
                );
            }
        }
    }

    public IReadOnlyList<SocialMediaEntry> GetSocialMedia()
    {
        return socialMedia;
    }

    public IReadOnlyList<GameEntry> GetGames()
    {
        return games;
    }

    public bool TryGetUrl(string label, out string url)
    {
        if (string.IsNullOrEmpty(label))
        {
            url = string.Empty;
            return false;
        }

        return labelToUrl.TryGetValue(label, out url) && !string.IsNullOrEmpty(url);
    }

    public void OpenUrl(string label)
    {
        if (!TryGetUrl(label, out string url))
        {
            Debug.LogWarning($"No URL mapped for label: {label}");
            return;
        }

        Application.OpenURL(url);
    }

    /// <summary>
    /// Updates the middle web view's preview image and loads the mapped URL
    /// for the specified game label (e.g., "CyberRun").
    /// </summary>
    public void ShowGameOnMiddle(string label)
    {
        if (string.IsNullOrEmpty(label))
        {
            Debug.LogWarning("ShowGameOnMiddle called with empty label");
            return;
        }

        // Update middle preview Image if present.
        if (middleCanvasWebViewPrefab != null)
        {
            var image = middleCanvasWebViewPrefab.GetComponent<Image>();
            if (image != null)
            {
                if (!labelToSprite.TryGetValue(label, out var sprite))
                {
                    // Attempt to build sprite cache on demand in case it wasn't built yet
                    BuildLookup();
                    labelToSprite.TryGetValue(label, out sprite);
                }
                if (sprite != null)
                {
                    image.sprite = sprite;
                    image.preserveAspect = true;
                }
                else
                {
                    Debug.LogWarning($"No sprite/texture found for label: {label}");
                }
            }

            // Load URL into the Vuplex web view. If not initialized yet, defer until ready.
            if (TryGetUrl(label, out var url))
            {
                if (middleCanvasWebViewPrefab.WebView != null)
                {
                    middleCanvasWebViewPrefab.WebView.LoadUrl(url);
                }
                else
                {
                    // Defer until the prefab finishes initializing.
                    void Handler(object s, System.EventArgs e)
                    {
                        middleCanvasWebViewPrefab.Initialized -= Handler;
                        middleCanvasWebViewPrefab.WebView.LoadUrl(url);
                    }
                    middleCanvasWebViewPrefab.Initialized += Handler;
                }
            }
            else
            {
                Debug.LogWarning($"No URL mapped for label: {label}");
            }
        }
        else
        {
            Debug.LogWarning("Middle CanvasWebViewPrefab reference not assigned in WorldCanvasManager.");
        }
    }
}
