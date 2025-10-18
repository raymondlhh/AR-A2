using UnityEngine;

/// <summary>
/// Component to handle touch detection for individual location pins
/// This provides an alternative approach to the raycast method in LocationPinManager
/// </summary>
public class LocationPinTouchDetector : MonoBehaviour
{
    private LocationPinManager manager;
    private string pinName;
    
    /// <summary>
    /// Initialize the touch detector with reference to manager and pin name
    /// </summary>
    public void Initialize(LocationPinManager manager, string pinName)
    {
        this.manager = manager;
        this.pinName = pinName;
    }
    
    /// <summary>
    /// Unity's built-in method for handling mouse/touch input on colliders
    /// This will be called when the object is touched/clicked
    /// </summary>
    void OnMouseDown()
    {
        if (manager != null)
        {
            manager.OnLocationPinTouched(pinName);
        }
    }
    
    /// <summary>
    /// Alternative method for touch input using Unity's EventSystem
    /// This can be used if you want to use UI events instead of OnMouseDown
    /// </summary>
    public void OnTouch()
    {
        if (manager != null)
        {
            manager.OnLocationPinTouched(pinName);
        }
    }
}
