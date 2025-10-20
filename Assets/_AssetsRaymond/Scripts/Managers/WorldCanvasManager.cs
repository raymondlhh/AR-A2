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
    [SerializeField] private Image middleScreenImage;
    [SerializeField] private Image rightScreenImage;

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
        labelToSprite.Clear();
        // Build sprites for social media
        for (int i = 0; i < socialMedia.Count; i++)
        {
            var entry = socialMedia[i];
            if (string.IsNullOrEmpty(entry.label)) { continue; }
            if (!labelToSprite.ContainsKey(entry.label) && entry.texture != null)
            {
                labelToSprite[entry.label] = Sprite.Create(
                    entry.texture,
                    new Rect(0, 0, entry.texture.width, entry.texture.height),
                    new Vector2(0.5f, 0.5f)
                );
            }
        }

        // Build sprites and URL lookup for games
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
    /// Middle shows Game texture; Left loads the game's URL.
    /// </summary>
    public void ShowGameOnMiddle(string label)
    {
        if (string.IsNullOrEmpty(label))
        {
            Debug.LogWarning("ShowGameOnMiddle called with empty label");
            return;
        }

        // Update middle screen image only.
        if (middleScreenImage != null)
        {
            if (!labelToSprite.TryGetValue(label, out var sprite))
            {
                // Attempt to build sprite cache on demand in case it wasn't built yet
                BuildLookup();
                labelToSprite.TryGetValue(label, out sprite);
            }
            if (sprite != null)
            {
                middleScreenImage.sprite = sprite;
                // Keep the original preserveAspect setting to maintain panel size
                // middleScreenImage.preserveAspect = true; // Commented out to prevent size changes
            }
            else
            {
                Debug.LogWarning($"No sprite/texture found for label: {label}");
            }
        }
        else
        {
            Debug.LogWarning("Middle Screen Image reference not assigned in WorldCanvasManager.");
        }

        // Load URL on the LEFT webview only.
        if (leftCanvasWebViewPrefab != null && TryGetUrl(label, out var leftUrl))
        {
            if (leftCanvasWebViewPrefab.WebView != null)
            {
                leftCanvasWebViewPrefab.WebView.LoadUrl(leftUrl);
            }
            else
            {
                void LeftHandler(object s, System.EventArgs e)
                {
                    leftCanvasWebViewPrefab.Initialized -= LeftHandler;
                    leftCanvasWebViewPrefab.WebView.LoadUrl(leftUrl);
                }
                leftCanvasWebViewPrefab.Initialized += LeftHandler;
            }
        }

        // Handle game object visibility based on the game label
        HandleGameObjectVisibility(label);
    }

    /// <summary>
    /// Handle showing/hiding game objects based on the game label
    /// </summary>
    private void HandleGameObjectVisibility(string label)
    {
        if (GameObjectVisibilityManager.Instance == null)
        {
            Debug.LogWarning("GameObjectVisibilityManager not found. Game object visibility will not be controlled.");
            return;
        }

        // Map game labels to group names
        string groupName = GetGroupNameFromLabel(label);
        if (!string.IsNullOrEmpty(groupName))
        {
            GameObjectVisibilityManager.Instance.ShowGroup(groupName);
        }
    }

    /// <summary>
    /// Map game labels to their corresponding group names
    /// </summary>
    private string GetGroupNameFromLabel(string label)
    {
        switch (label.ToLower())
        {
            case "merlin's curse":
            case "merlins curse":
            case "merlin curse":
                return "MerlinCurse";
            case "the debuggers":
            case "debuggers":
                return "TheDebuggers";
            case "cyber run":
            case "cyberrun":
                return "CyberRun";
            default:
                return label; // Return the label as-is if no mapping found
        }
    }

    /// <summary>
    /// Right shows Social Media texture; no URL change here.
    /// </summary>
    public void ShowSocialOnRight(string label)
    {
        if (string.IsNullOrEmpty(label))
        {
            Debug.LogWarning("ShowSocialOnRight called with empty label");
            return;
        }

        if (rightScreenImage == null)
        {
            Debug.LogWarning("Right Screen Image reference not assigned in WorldCanvasManager.");
            return;
        }

        if (!labelToSprite.TryGetValue(label, out var sprite))
        {
            // Ensure cache is up to date; then try again.
            BuildLookup();
            labelToSprite.TryGetValue(label, out sprite);
        }
        if (sprite != null)
        {
            rightScreenImage.sprite = sprite;
            // Keep the original preserveAspect setting to maintain panel size
            // rightScreenImage.preserveAspect = true; // Commented out to prevent size changes
        }
    }
}
