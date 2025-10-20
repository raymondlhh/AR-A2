# GameObjectVisibilityManager Setup Guide

## Overview
This system allows you to show/hide specific game objects when game category buttons are clicked. When a button is clicked, it will show only the corresponding game object and hide all others.

## Setup Instructions

### 1. Add GameObjectVisibilityManager to your scene
1. Create an empty GameObject in your scene
2. Name it "GameObjectVisibilityManager"
3. Add the `GameObjectVisibilityManager` script to this GameObject

### 2. Configure Game Object Groups
In the GameObjectVisibilityManager inspector, you need to set up the groups:

#### Group Names (exactly as shown):
- **MerlinCurse** - for the Merlin's Curse game object
- **TheDebuggers** - for The Debuggers game object  
- **CyberRun** - for the CyberRun game object

#### For each group:
1. Set the `Group Name` field to one of the names above
2. Drag the corresponding game object from the hierarchy into the `Game Objects` list
3. Repeat for all three groups

### 3. Configure ButtonClick Scripts
For each game category button (MerlinsCurseButton, TheDebuggersButton, CyberRunButton):

1. Select the button GameObject
2. In the ButtonClick component, set:
   - `Category` = Game
   - `World Canvas Game Label` = The game name (e.g., "Merlin's Curse", "The Debuggers", "CyberRun")
   - `Game Object Group Name` = The corresponding group name (e.g., "MerlinCurse", "TheDebuggers", "CyberRun")

### 4. Example Configuration

#### MerlinCurse Button:
- Category: Game
- World Canvas Game Label: "Merlin's Curse"
- Game Object Group Name: "MerlinCurse"

#### TheDebuggers Button:
- Category: Game  
- World Canvas Game Label: "The Debuggers"
- Game Object Group Name: "TheDebuggers"

#### CyberRun Button:
- Category: Game
- World Canvas Game Label: "CyberRun" 
- Game Object Group Name: "CyberRun"

## How It Works

1. When a game category button is clicked, the ButtonClick script calls `GameObjectVisibilityManager.ShowGroup()`
2. The visibility manager hides all groups first, then shows only the specified group
3. This ensures only one game object is visible at a time
4. The WorldCanvasManager also handles visibility as a backup system

## Troubleshooting

- Make sure the GameObjectVisibilityManager is in your scene
- Verify group names match exactly (case-sensitive)
- Check that game objects are properly assigned to their groups
- Ensure ButtonClick scripts have the correct group names assigned
- Check the console for any warning messages

## Adding New Games

To add a new game:
1. Create a new group in GameObjectVisibilityManager with the game's name
2. Add the game object to that group
3. Configure the button with the same group name
4. Update the `GetGroupNameFromLabel()` method in WorldCanvasManager if needed
