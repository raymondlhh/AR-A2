using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSFX : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioManager audioManager;
    
    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;
    
    // Start is called before the first frame update
    void Start()
    {
        InitializeAudioManager();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    /// <summary>
    /// Initialize the AudioManager reference
    /// </summary>
    private void InitializeAudioManager()
    {
        // Find AudioManager if not assigned
        if (audioManager == null)
        {
            audioManager = FindObjectOfType<AudioManager>();
        }
        
        if (audioManager == null)
        {
            Debug.LogWarning("AnimationSFX: AudioManager not found! Sound effects will not play.");
        }
        else if (enableDebugLogs)
        {
            Debug.Log("AnimationSFX: AudioManager found and initialized");
        }
    }
    
    /// <summary>
    /// Play the Pop sound effect
    /// </summary>
    public void PlayPopSound()
    {
        if (audioManager != null)
        {
            audioManager.PlaySFXByName("Pop");
            
            if (enableDebugLogs)
                Debug.Log("AnimationSFX: Playing Pop sound effect");
        }
        else
        {
            Debug.LogWarning("AnimationSFX: Cannot play Pop sound - AudioManager not found!");
        }
    }
}
