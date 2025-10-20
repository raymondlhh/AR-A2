using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocationPinManager : MonoBehaviour
{
    [Header("Location Pin and Status UI Management")]
    [SerializeField] private Transform locationPinsParent;
    [SerializeField] private Transform statusUIParent;
    
    [Header("Auto-Hide Settings")]
    [SerializeField] private bool enableAutoHide = true;
    [SerializeField] private float autoHideDelay = 10f;
    
    [Header("Audio")]
    [SerializeField] private AudioManager audioManager;
    
    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;
    
    // Dictionary to store the mapping between location pins and their corresponding status UIs
    private Dictionary<string, GameObject> locationPins = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> statusUIs = new Dictionary<string, GameObject>();
    
    // Currently active status UI
    private string currentActiveStatusUI = "";
    
    // Auto-hide timer 
    private float autoHideTimer = 0f;
    private bool isAutoHideActive = false;
    
    void Start()
    {
        InitializeLocationPinsAndStatusUIs();
        InitializeAudioManager();
    }
    
    void Update()
    {
        HandleTouchInput();
        HandleAutoHide();
    }
    
    /// <summary>
    /// Initialize the location pins and status UIs by finding them in the scene
    /// </summary>
    private void InitializeLocationPinsAndStatusUIs()
    {
        // Find the parent objects if not assigned
        if (locationPinsParent == null)
        {
            locationPinsParent = GameObject.Find("LocationPins")?.transform;
        }
        
        if (statusUIParent == null)
        {
            statusUIParent = GameObject.Find("WorldCanvas")?.transform;
        }
        
        if (locationPinsParent == null || statusUIParent == null)
        {
            Debug.LogError("LocationPinManager: Could not find LocationPins or WorldCanvas parent objects!");
            return;
        }
        
        // Populate location pins dictionary
        for (int i = 0; i < locationPinsParent.childCount; i++)
        {
            Transform child = locationPinsParent.GetChild(i);
            string pinName = child.name;
            
            if (pinName.StartsWith("LocationPin_"))
            {
                locationPins[pinName] = child.gameObject;
                
                // Add touch detection component if not present
                if (child.GetComponent<LocationPinTouchDetector>() == null)
                {
                    LocationPinTouchDetector touchDetector = child.gameObject.AddComponent<LocationPinTouchDetector>();
                    touchDetector.Initialize(this, pinName);
                }
                
                if (enableDebugLogs)
                    Debug.Log($"LocationPinManager: Found location pin - {pinName}");
            }
        }
        
        // Populate status UIs dictionary
        for (int i = 0; i < statusUIParent.childCount; i++)
        {
            Transform child = statusUIParent.GetChild(i);
            string uiName = child.name;
            
            if (uiName.StartsWith("StatusUI_"))
            {
                statusUIs[uiName] = child.gameObject;
                
                // Ensure all status UIs are initially hidden
                child.gameObject.SetActive(false);
                
                if (enableDebugLogs)
                    Debug.Log($"LocationPinManager: Found status UI - {uiName}");
            }
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"LocationPinManager: Initialized with {locationPins.Count} location pins and {statusUIs.Count} status UIs");
        }
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
            Debug.LogWarning("LocationPinManager: AudioManager not found! Sound effects will not play.");
        }
        else if (enableDebugLogs)
        {
            Debug.Log("LocationPinManager: AudioManager found and initialized");
        }
    }
    
    /// <summary>
    /// Handle auto-hide timer
    /// </summary>
    private void HandleAutoHide()
    {
        if (!enableAutoHide || !isAutoHideActive)
            return;
        
        // Count down the timer
        autoHideTimer -= Time.deltaTime;
        
        // Check if timer has expired
        if (autoHideTimer <= 0f)
        {
            HideAllStatusUIs();
            isAutoHideActive = false;
            
            if (enableDebugLogs)
                Debug.Log("LocationPinManager: Auto-hide triggered - all status UIs hidden");
        }
    }
    
    /// <summary>
    /// Handle touch input for mobile devices
    /// </summary>
    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                CheckForLocationPinTouch(ray);
            }
        }
        
        // Also handle mouse input for testing in editor
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            CheckForLocationPinTouch(ray);
        }
    }
    
    /// <summary>
    /// Check if the ray hits any location pin and handle the interaction
    /// </summary>
    private void CheckForLocationPinTouch(Ray ray)
    {
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObject = hit.collider.gameObject;
            string objectName = hitObject.name;
            
            if (enableDebugLogs)
                Debug.Log($"LocationPinManager: Raycast hit - {objectName}");
            
            // Check if the hit object is a location pin
            if (objectName.StartsWith("LocationPin_"))
            {
                OnLocationPinTouched(objectName);
            }
        }
    }
    
    /// <summary>
    /// Called when a location pin is touched
    /// </summary>
    public void OnLocationPinTouched(string locationPinName)
    {
        if (enableDebugLogs)
            Debug.Log($"LocationPinManager: Location pin touched - {locationPinName}");
        
        // Play button click sound effect
        PlayButtonClickSound();
        
        // Convert LocationPin name to StatusUI name
        string statusUIName = locationPinName.Replace("LocationPin_", "StatusUI_");
        
        // Show the corresponding status UI
        ShowStatusUI(statusUIName);
        
        // Start auto-hide timer if enabled
        if (enableAutoHide)
        {
            StartAutoHideTimer();
        }
    }
    
    /// <summary>
    /// Start the auto-hide timer
    /// </summary>
    private void StartAutoHideTimer()
    {
        autoHideTimer = autoHideDelay;
        isAutoHideActive = true;
        
        if (enableDebugLogs)
            Debug.Log($"LocationPinManager: Auto-hide timer started - {autoHideDelay} seconds");
    }
    
    /// <summary>
    /// Stop the auto-hide timer
    /// </summary>
    private void StopAutoHideTimer()
    {
        isAutoHideActive = false;
        autoHideTimer = 0f;
        
        if (enableDebugLogs)
            Debug.Log("LocationPinManager: Auto-hide timer stopped");
    }
    
    /// <summary>
    /// Show a specific status UI and hide all others
    /// </summary>
    public void ShowStatusUI(string statusUIName)
    {
        // Hide all status UIs first
        HideAllStatusUIs();
        
        // Show the requested status UI
        if (statusUIs.ContainsKey(statusUIName))
        {
            statusUIs[statusUIName].SetActive(true);
            currentActiveStatusUI = statusUIName;
            
            if (enableDebugLogs)
                Debug.Log($"LocationPinManager: Showing status UI - {statusUIName}");
        }
        else
        {
            Debug.LogWarning($"LocationPinManager: Status UI not found - {statusUIName}");
        }
    }
    
    /// <summary>
    /// Hide all status UIs
    /// </summary>
    public void HideAllStatusUIs()
    {
        foreach (var statusUI in statusUIs.Values)
        {
            statusUI.SetActive(false);
        }
        
        currentActiveStatusUI = "";
        
        // Stop auto-hide timer when hiding all UIs
        StopAutoHideTimer();
        
        if (enableDebugLogs)
            Debug.Log("LocationPinManager: All status UIs hidden");
    }
    
    /// <summary>
    /// Get the currently active status UI name
    /// </summary>
    public string GetCurrentActiveStatusUI()
    {
        return currentActiveStatusUI;
    }
    
    /// <summary>
    /// Check if a specific status UI is currently active
    /// </summary>
    public bool IsStatusUIActive(string statusUIName)
    {
        return currentActiveStatusUI == statusUIName;
    }
    
    /// <summary>
    /// Toggle a specific status UI (show if hidden, hide if shown)
    /// </summary>
    public void ToggleStatusUI(string statusUIName)
    {
        if (IsStatusUIActive(statusUIName))
        {
            HideAllStatusUIs();
        }
        else
        {
            ShowStatusUI(statusUIName);
        }
    }
    
    /// <summary>
    /// Get the remaining auto-hide time
    /// </summary>
    public float GetRemainingAutoHideTime()
    {
        return isAutoHideActive ? autoHideTimer : 0f;
    }
    
    /// <summary>
    /// Check if auto-hide is currently active
    /// </summary>
    public bool IsAutoHideActive()
    {
        return isAutoHideActive;
    }
    
    /// <summary>
    /// Play the button click sound effect
    /// </summary>
    private void PlayButtonClickSound()
    {
        if (audioManager != null)
        {
            audioManager.PlaySFXByName("Button Click");
            
            if (enableDebugLogs)
                Debug.Log("LocationPinManager: Playing button click sound effect");
        }
        else
        {
            Debug.LogWarning("LocationPinManager: Cannot play button click sound - AudioManager not found!");
        }
    }
}
