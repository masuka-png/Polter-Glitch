using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InteractionUIManager : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Image _interactionPrompt;
    [SerializeField] private Sprite _doorSprite;
    [SerializeField] private Sprite _keyboardSprite;
    [SerializeField] private Sprite _elevatorDoorSprite;
    [SerializeField] private float _fadeInDuration = 0.2f;
    [SerializeField] private float _fadeOutDuration = 0.2f;
    [SerializeField] private Vector2 _promptOffset = new Vector2(0, -100);
    
    private CanvasGroup _canvasGroup;
    private RectTransform _promptRect;
    private float _fadeTimer = 0f;
    private bool _isFadingIn = false;
    private bool _isFadingOut = false;
    private IInteractable _currentInteractable = null;

    private void Start()
    {
        if (_canvas == null)
        {
            _canvas = FindObjectOfType<Canvas>();
        }

        _canvasGroup = _canvas.GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
        {
            _canvasGroup = _canvas.gameObject.AddComponent<CanvasGroup>();
        }

        _promptRect = _interactionPrompt.GetComponent<RectTransform>();
        
        // Start invisible
        _canvasGroup.alpha = 0f;
        _interactionPrompt.gameObject.SetActive(false);
    }

    /// <summary>
    /// Show UI prompt for a specific interactable type
    /// </summary>
    public void ShowPrompt(IInteractable interactable, InteractableType type)
    {
        if (_currentInteractable == interactable)
            return; // Already showing this one

        _currentInteractable = interactable;
        Sprite spriteToShow = GetSpriteForType(type);

        if (spriteToShow != null)
        {
            _interactionPrompt.sprite = spriteToShow;
            _interactionPrompt.gameObject.SetActive(true);
            FadeIn();
        }
    }

    /// <summary>
    /// Hide the UI prompt
    /// </summary>
    public void HidePrompt()
    {
        if (_currentInteractable == null)
            return; // Already hidden

        _currentInteractable = null;
        FadeOut();
    }

    /// <summary>
    /// Hide prompt for a specific interactable (if it's currently showing)
    /// </summary>
    public void HidePromptIfCurrent(IInteractable interactable)
    {
        if (_currentInteractable == interactable)
        {
            HidePrompt();
        }
    }

    private Sprite GetSpriteForType(InteractableType type)
    {
        return type switch
        {
            InteractableType.Door => _doorSprite,
            InteractableType.Keyboard => _keyboardSprite,
            InteractableType.ClosedElevator => _elevatorDoorSprite,
            _ => null
        };
    }

    private void FadeIn()
    {
        _isFadingOut = false;
        _isFadingIn = true;
        _fadeTimer = 0f;
    }

    private void FadeOut()
    {
        _isFadingIn = false;
        _isFadingOut = true;
        _fadeTimer = 0f;
    }

    private void Update()
    {
        if (_isFadingIn)
        {
            _fadeTimer += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(0f, 1f, _fadeTimer / _fadeInDuration);

            if (_fadeTimer >= _fadeInDuration)
            {
                _canvasGroup.alpha = 1f;
                _isFadingIn = false;
            }
        }
        else if (_isFadingOut)
        {
            _fadeTimer += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(1f, 0f, _fadeTimer / _fadeOutDuration);

            if (_fadeTimer >= _fadeOutDuration)
            {
                _canvasGroup.alpha = 0f;
                _isFadingOut = false;
                _interactionPrompt.gameObject.SetActive(false);
            }
        }
    }

    public IInteractable GetCurrentInteractable => _currentInteractable;
}

public enum InteractableType
{
    Door,
    Keyboard,
    ClosedElevator
}
