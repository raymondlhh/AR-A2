# Location Pin and Status UI System Setup Guide

This guide will help you set up the interactive location pin system that shows/hides corresponding status UIs when touched.

## Overview

The system consists of three main components:
1. **LocationPinManager** - Main controller that manages all interactions
2. **LocationPinTouchDetector** - Individual touch detection for each location pin
3. **StateUI** - Enhanced status UI component with animations and sound

## Setup Instructions

### Step 1: Add the LocationPinManager to your scene

1. Create an empty GameObject in your scene (e.g., "LocationPinManager")
2. Add the `LocationPinManager` script to this GameObject
3. In the inspector, assign the following:
   - **Location Pins Parent**: Drag the "LocationPins" GameObject from your hierarchy
   - **Status UI Parent**: Drag the "WorldCanvas" GameObject from your hierarchy
   - **Enable Debug Logs**: Check this for debugging (optional)

### Step 2: Verify Location Pin Setup

The system will automatically:
- Find all GameObjects with names starting with "LocationPin_" under the LocationPins parent
- Find all GameObjects with names starting with "StatusUI_" under the WorldCanvas parent
- Add touch detection components to each location pin
- Ensure all status UIs are initially hidden

### Step 3: Add StateUI Components (Optional Enhancement)

For enhanced functionality, you can add the `StateUI` script to each StatusUI GameObject:

1. Select each StatusUI GameObject in the WorldCanvas
2. Add the `StateUI` component
3. Configure the following in the inspector:
   - **State Name**: Will be auto-extracted from GameObject name
   - **Is Initially Visible**: Should be false (default)
   - **Enable Fade Animation**: Check for smooth transitions
   - **Fade In Duration**: 0.5 seconds (default)
   - **Fade Out Duration**: 0.3 seconds (default)
   - **Show Sound**: Optional audio clip for when UI appears
   - **Hide Sound**: Optional audio clip for when UI disappears

## How It Works

### Touch Detection
- **Mobile**: Uses Unity's touch input system
- **Editor/PC**: Uses mouse clicks for testing
- **Method**: Raycast from camera through touch/click position

### UI Management
- When a LocationPin is touched, the system:
  1. Hides all currently visible StatusUIs
  2. Shows the corresponding StatusUI (e.g., LocationPin_Johor → StatusUI_Johor)
  3. Plays optional sound effects
  4. Applies fade animations if enabled

### Naming Convention
The system relies on consistent naming:
- LocationPins: `LocationPin_StateName` (e.g., LocationPin_Johor)
- StatusUIs: `StatusUI_StateName` (e.g., StatusUI_Johor)

## API Reference

### LocationPinManager Methods

```csharp
// Show a specific status UI
public void ShowStatusUI(string statusUIName)

// Hide all status UIs
public void HideAllStatusUIs()

// Toggle a specific status UI
public void ToggleStatusUI(string statusUIName)

// Check if a status UI is active
public bool IsStatusUIActive(string statusUIName)

// Get currently active status UI name
public string GetCurrentActiveStatusUI()
```

### StateUI Methods

```csharp
// Show the status UI
public void Show(bool animated = true)

// Hide the status UI
public void Hide(bool animated = true)

// Toggle visibility
public void Toggle(bool animated = true)

// Set visibility directly
public void SetVisibility(bool visible, bool animated = true)

// Check if visible
public bool IsVisible()

// Get state name
public string GetStateName()
```

## Testing

1. **In Editor**: Click on any LocationPin to test the system
2. **On Mobile**: Touch any LocationPin to see the corresponding StatusUI appear
3. **Debug**: Enable debug logs to see system activity in the console

## Troubleshooting

### Common Issues

1. **LocationPins not responding to touch**:
   - Ensure LocationPins have BoxCollider components
   - Check that the camera has a Camera component
   - Verify the LocationPinManager is properly assigned

2. **StatusUIs not showing**:
   - Check naming convention (LocationPin_X → StatusUI_X)
   - Ensure StatusUIs are children of WorldCanvas
   - Verify the StatusUI GameObjects are not disabled

3. **Animations not working**:
   - Ensure StateUI components are added to StatusUI GameObjects
   - Check that CanvasGroup components are present
   - Verify animation settings in the inspector

### Debug Tips

- Enable "Enable Debug Logs" in LocationPinManager for detailed console output
- Use Unity's Scene view to verify raycast hits
- Check the hierarchy to ensure proper parent-child relationships

## Customization

### Adding New States
1. Create new LocationPin prefab with name "LocationPin_NewState"
2. Create new StatusUI prefab with name "StatusUI_NewState"
3. Place them in the appropriate parent objects
4. The system will automatically detect and handle them

### Custom Animations
- Modify the fade durations in StateUI components
- Add custom animation scripts to StatusUI GameObjects
- Override the Show/Hide methods for custom behavior

### Sound Effects
- Assign AudioClip assets to Show Sound and Hide Sound in StateUI components
- The system will automatically play sounds when UIs appear/disappear

## Performance Notes

- The system uses efficient dictionary lookups for O(1) access
- Raycast is only performed on touch/click events
- Animations use coroutines to avoid blocking the main thread
- All StatusUIs are initially hidden to improve performance
