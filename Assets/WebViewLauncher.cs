using UnityEngine;

public class WebViewLauncher : MonoBehaviour
{
    [Header("URL")]
    [SerializeField] private string url = "https://www.example.com";

    [Header("Options")]
    [SerializeField] private bool transparent = true;
    [SerializeField] private bool enableZoom = true;

    [Header("Margins (px)")]
    [SerializeField] private int left = 0;
    [SerializeField] private int top = 0;
    [SerializeField] private int right = 0;
    [SerializeField] private int bottom = 0;

    private WebViewObject webView;

    private void Start()
    {
        webView = GetComponent<WebViewObject>();
        if (webView == null)
        {
            webView = gameObject.AddComponent<WebViewObject>();
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        webView.Init(
            cb: (msg) => Debug.Log($"[WebView][JS] {msg}"),
            err: (msg) => Debug.LogError($"[WebView][Error] {msg}"),
            httpErr: (msg) => Debug.LogError($"[WebView][HTTP] {msg}"),
            ld: (msg) =>
            {
                // Ensure it becomes visible after the first page finishes loading.
                webView.SetVisibility(true);
            },
            transparent: transparent,
            zoom: enableZoom
        );

        webView.SetMargins(left, top, right, bottom, false);
        webView.SetVisibility(true);
        if (!string.IsNullOrEmpty(url))
        {
            webView.LoadURL(url);
        }
#else
        Debug.LogWarning("Unity WebView runs on device. Build and run on Android/iOS to see it.");
#endif
    }
}


