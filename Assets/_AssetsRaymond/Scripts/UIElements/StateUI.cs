using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateUI : MonoBehaviour
{
    [Header("State UI Configuration")]
    [SerializeField] private string stateName;
    [SerializeField] private bool isInitiallyVisible = false;
    
    [Header("Animation Settings")]
    [SerializeField] private bool enableFadeAnimation = true;
    [SerializeField] private float fadeInDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.3f;
    
    [Header("Audio")]
    [SerializeField] private AudioClip showSound;
    [SerializeField] private AudioClip hideSound;
    
    private CanvasGroup canvasGroup;
    private AudioSource audioSource;
    private bool isVisible = false;
    
    // Start is called before the first frame update
    void Start()
    {
        InitializeStateUI();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    /// <summary>
    /// Initialize the StateUI component
    /// </summary>
    private void InitializeStateUI()
    {
        // Get or add CanvasGroup for fade animations
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        // Get or add AudioSource for sound effects
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && (showSound != null || hideSound != null))
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        
        // Extract state name from GameObject name if not set
        if (string.IsNullOrEmpty(stateName))
        {
            stateName = gameObject.name.Replace("StatusUI_", "");
        }
        
        // Set initial visibility
        SetVisibility(isInitiallyVisible, false);
    }
    
    /// <summary>
    /// Show the status UI with optional animation
    /// </summary>
    public void Show(bool animated = true)
    {
        if (isVisible) return;
        
        isVisible = true;
        gameObject.SetActive(true);
        
        if (animated && enableFadeAnimation)
        {
            StartCoroutine(FadeIn());
        }
        else
        {
            canvasGroup.alpha = 1f;
        }
        
        // Play show sound
        PlaySound(showSound);
        
        Debug.Log($"StateUI: Showing {stateName}");
    }
    
    /// <summary>
    /// Hide the status UI with optional animation
    /// </summary>
    public void Hide(bool animated = true)
    {
        if (!isVisible) return;
        
        isVisible = false;
        
        if (animated && enableFadeAnimation)
        {
            StartCoroutine(FadeOut());
        }
        else
        {
            gameObject.SetActive(false);
        }
        
        // Play hide sound
        PlaySound(hideSound);
        
        Debug.Log($"StateUI: Hiding {stateName}");
    }
    
    /// <summary>
    /// Toggle the visibility of the status UI
    /// </summary>
    public void Toggle(bool animated = true)
    {
        if (isVisible)
        {
            Hide(animated);
        }
        else
        {
            Show(animated);
        }
    }
    
    /// <summary>
    /// Set visibility directly without animation
    /// </summary>
    public void SetVisibility(bool visible, bool animated = true)
    {
        if (visible)
        {
            Show(animated);
        }
        else
        {
            Hide(animated);
        }
    }
    
    /// <summary>
    /// Fade in animation coroutine
    /// </summary>
    private IEnumerator FadeIn()
    {
        canvasGroup.alpha = 0f;
        float elapsedTime = 0f;
        
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
            yield return null;
        }
        
        canvasGroup.alpha = 1f;
    }
    
    /// <summary>
    /// Fade out animation coroutine
    /// </summary>
    private IEnumerator FadeOut()
    {
        float startAlpha = canvasGroup.alpha;
        float elapsedTime = 0f;
        
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeOutDuration);
            yield return null;
        }
        
        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }
    
    /// <summary>
    /// Play a sound effect if audio source and clip are available
    /// </summary>
    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
    
    /// <summary>
    /// Get the state name
    /// </summary>
    public string GetStateName()
    {
        return stateName;
    }
    
    /// <summary>
    /// Check if the status UI is currently visible
    /// </summary>
    public bool IsVisible()
    {
        return isVisible;
    }
    
    /// <summary>
    /// Set the state name
    /// </summary>
    public void SetStateName(string newStateName)
    {
        stateName = newStateName;
    }
}
