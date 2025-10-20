using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ButtonClick : MonoBehaviour
{
    public enum ButtonCategory
    {
        None,
        Game,
        Social
    }
    [SerializeField]
    private Camera raycastCamera;

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private string triggerParameterName = "ButtonClick";

	[SerializeField]
	private string worldCanvasGameLabel = ""; // e.g., "CyberRun"

	[SerializeField]
	private ButtonCategory category = ButtonCategory.None;

	[Header("Game Object Visibility")]
	[SerializeField]
	private string gameObjectGroupName = ""; // e.g., "MerlinCurse", "TheDebuggers", "CyberRun"

	[SerializeField]
	private LayerMask hittableLayers = ~0; // All layers by default

	[SerializeField]
	private bool alsoTryMainCamera = true;

	[Header("Audio Settings")]
	[SerializeField]
	private string soundEffectName = "Button Pressed"; // Name of the sound effect to play

	[SerializeField]
	private AudioManager audioManager; // Reference to AudioManager

    // Start is called before the first frame update
    void Start()
    {
        if (raycastCamera == null)
        {
            raycastCamera = Camera.main;
        }

		if (animator == null)
		{
			animator = GetComponent<Animator>();
			if (animator == null)
			{
				animator = GetComponentInChildren<Animator>();
			}
		}

		// Initialize AudioManager reference if not assigned
		if (audioManager == null)
		{
			audioManager = FindObjectOfType<AudioManager>();
		}
    }

	// Update is called once per frame
	void Update()
	{
		// Touch (mobile) — use first touch only like the tutorial pattern
		if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
		{
			TryHandlePointerDown(Input.touches[0].position);
			return;
		}

		// Mouse (editor/desktop)
		if (Input.GetMouseButtonDown(0))
		{
			TryHandlePointerDown(Input.mousePosition);
		}
	}

    private void TryHandlePointerDown(Vector2 screenPosition)
    {
        if (raycastCamera == null)
        {
            return;
        }

		Camera primary = raycastCamera != null ? raycastCamera : Camera.main;
		if (primary == null)
		{
			return;
		}

		Ray ray = primary.ScreenPointToRay(screenPosition);
		if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, hittableLayers, QueryTriggerInteraction.Collide))
        {
            Transform hitTransform = hitInfo.transform;
            if (hitTransform == transform || hitTransform.IsChildOf(transform))
            {
                Debug.Log($"3D Button tapped: {name}");
				OnButtonPressed();
            }
        }
		else if (alsoTryMainCamera && primary != Camera.main && Camera.main != null)
		{
			// Sometimes the assigned camera is a UI camera; try the main camera too
			Ray altRay = Camera.main.ScreenPointToRay(screenPosition);
			if (Physics.Raycast(altRay, out RaycastHit altHit, Mathf.Infinity, hittableLayers, QueryTriggerInteraction.Collide))
			{
				Transform hitTransform = altHit.transform;
				if (hitTransform == transform || hitTransform.IsChildOf(transform))
				{
					Debug.Log($"3D Button tapped (alt cam): {name}");
					OnButtonPressed();
				}
			}
		}
    }

	private void OnButtonPressed()
	{
		// Play sound effect
		PlaySoundEffect();

		if (animator != null && !string.IsNullOrEmpty(triggerParameterName))
		{
			animator.SetTrigger(triggerParameterName);
		}

		if (!string.IsNullOrEmpty(worldCanvasGameLabel) && WorldCanvasManager.Instance != null)
		{
			if (category == ButtonCategory.Game)
			{
				WorldCanvasManager.Instance.ShowGameOnMiddle(worldCanvasGameLabel);
			}
			else if (category == ButtonCategory.Social)
			{
				WorldCanvasManager.Instance.ShowSocialOnRight(worldCanvasGameLabel);
			}
		}

		// Handle game object visibility for game category buttons
		if (category == ButtonCategory.Game && !string.IsNullOrEmpty(gameObjectGroupName) && GameObjectVisibilityManager.Instance != null)
		{
			GameObjectVisibilityManager.Instance.ShowGroup(gameObjectGroupName);
		}
	}

	private void PlaySoundEffect()
	{
		if (string.IsNullOrEmpty(soundEffectName))
			return;

		// Use AudioManager to play sound effect
		if (audioManager != null)
		{
			audioManager.PlaySFXByName(soundEffectName);
		}
		else
		{
			Debug.LogWarning($"ButtonClick: AudioManager not found. Cannot play sound effect '{soundEffectName}'. Please assign an AudioManager or ensure one exists in the scene.");
		}
	}
}
