using System.Collections.Generic;
using UnityEngine;

public class GameObjectVisibilityManager : MonoBehaviour
{
    [System.Serializable]
    public class GameObjectGroup
    {
        public string groupName;
        public List<GameObject> gameObjects = new List<GameObject>();
    }

    [Header("Game Object Groups")]
    [SerializeField] private List<GameObjectGroup> gameObjectGroups = new List<GameObjectGroup>();

    public static GameObjectVisibilityManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Initialize all groups to be hidden by default
        HideAllGroups();
    }

    /// <summary>
    /// Show only the specified group and hide all others
    /// </summary>
    /// <param name="groupName">Name of the group to show</param>
    public void ShowGroup(string groupName)
    {
        if (string.IsNullOrEmpty(groupName))
        {
            Debug.LogWarning("GameObjectVisibilityManager: Group name is null or empty");
            return;
        }

        // Hide all groups first
        HideAllGroups();

        // Find and show the specified group
        GameObjectGroup targetGroup = gameObjectGroups.Find(group => group.groupName == groupName);
        if (targetGroup != null)
        {
            foreach (GameObject obj in targetGroup.gameObjects)
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                }
            }
            Debug.Log($"GameObjectVisibilityManager: Showing group '{groupName}'");
        }
        else
        {
            Debug.LogWarning($"GameObjectVisibilityManager: Group '{groupName}' not found");
        }
    }

    /// <summary>
    /// Hide all groups
    /// </summary>
    public void HideAllGroups()
    {
        foreach (GameObjectGroup group in gameObjectGroups)
        {
            foreach (GameObject obj in group.gameObjects)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// Show all groups
    /// </summary>
    public void ShowAllGroups()
    {
        foreach (GameObjectGroup group in gameObjectGroups)
        {
            foreach (GameObject obj in group.gameObjects)
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                }
            }
        }
    }

    /// <summary>
    /// Add a game object to a specific group
    /// </summary>
    /// <param name="groupName">Name of the group</param>
    /// <param name="gameObject">GameObject to add</param>
    public void AddToGroup(string groupName, GameObject gameObject)
    {
        if (string.IsNullOrEmpty(groupName) || gameObject == null)
        {
            Debug.LogWarning("GameObjectVisibilityManager: Invalid group name or game object");
            return;
        }

        GameObjectGroup group = gameObjectGroups.Find(g => g.groupName == groupName);
        if (group == null)
        {
            group = new GameObjectGroup { groupName = groupName };
            gameObjectGroups.Add(group);
        }

        if (!group.gameObjects.Contains(gameObject))
        {
            group.gameObjects.Add(gameObject);
        }
    }

    /// <summary>
    /// Remove a game object from a specific group
    /// </summary>
    /// <param name="groupName">Name of the group</param>
    /// <param name="gameObject">GameObject to remove</param>
    public void RemoveFromGroup(string groupName, GameObject gameObject)
    {
        if (string.IsNullOrEmpty(groupName) || gameObject == null)
        {
            return;
        }

        GameObjectGroup group = gameObjectGroups.Find(g => g.groupName == groupName);
        if (group != null)
        {
            group.gameObjects.Remove(gameObject);
        }
    }
}
