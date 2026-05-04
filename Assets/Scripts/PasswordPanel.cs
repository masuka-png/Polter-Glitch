using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

/// <summary>
/// UI Panel for the password prompt dialog
/// Automatically creates UI elements if not assigned
/// </summary>
public class PasswordPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TMP_InputField _passwordInput;
    [SerializeField] private TextMeshProUGUI _feedbackText;
    [SerializeField] private Button _submitButton;
    [SerializeField] private Button _cancelButton;
    
    private string _correctPassword;
    private Action<bool> _onComplete;
    private bool _isComplete = false;
    
    private void Start()
    {
        // Auto-create UI elements if they're not assigned
        if (_titleText == null)
            CreateTitleText();
        
        if (_passwordInput == null)
            CreatePasswordInput();
        
        if (_feedbackText == null)
            CreateFeedbackText();
        
        if (_submitButton == null)
            CreateSubmitButton();
        
        if (_cancelButton == null)
            CreateCancelButton();
        
        // Hook up button listeners
        if (_submitButton != null)
            _submitButton.onClick.AddListener(OnSubmit);
        
        if (_cancelButton != null)
            _cancelButton.onClick.AddListener(OnCancel);
    }
    
    public void Initialize(string keyboardName, string correctPassword, Action<bool> onComplete)
    {
        _correctPassword = correctPassword;
        _onComplete = onComplete;
        
        if (_titleText != null)
            _titleText.text = keyboardName;
        
        if (_passwordInput != null)
        {
            _passwordInput.text = "";
            _passwordInput.ActivateInputField();
        }
        
        if (_feedbackText != null)
            _feedbackText.text = "Enter password:";
    }
    
    private void OnSubmit()
    {
        if (_isComplete)
            return;
        
        if (_passwordInput == null)
            return;
        
        bool correct = _passwordInput.text == _correctPassword;
        
        if (correct)
        {
            if (_feedbackText != null)
                _feedbackText.text = "Access granted!";
            _isComplete = true;
            _onComplete?.Invoke(true);
            Destroy(gameObject, 1f);
        }
        else
        {
            if (_feedbackText != null)
                _feedbackText.text = "Access denied! Try again.";
            if (_passwordInput != null)
                _passwordInput.text = "";
        }
    }
    
    private void OnCancel()
    {
        if (_isComplete)
            return;
        
        _isComplete = true;
        _onComplete?.Invoke(false);
        Destroy(gameObject);
    }
    
    // Auto-create UI elements if not found
    private void CreateTitleText()
    {
        GameObject textObj = new GameObject("TitleText");
        textObj.transform.SetParent(transform, false);
        
        RectTransform rectTransform = textObj.AddComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0, 100);
        rectTransform.sizeDelta = new Vector2(600, 100);
        
        _titleText = textObj.AddComponent<TextMeshProUGUI>();
        _titleText.text = "Security Terminal";
        _titleText.fontSize = 50;
        _titleText.alignment = TextAlignmentOptions.Center;
    }
    
    private void CreatePasswordInput()
    {
        GameObject inputObj = new GameObject("PasswordInput");
        inputObj.transform.SetParent(transform, false);
        
        RectTransform rectTransform = inputObj.AddComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(500, 60);
        
        Image background = inputObj.AddComponent<Image>();
        background.color = new Color(0.2f, 0.2f, 0.2f);
        
        _passwordInput = inputObj.AddComponent<TMP_InputField>();
        _passwordInput.contentType = TMP_InputField.ContentType.Password;
        _passwordInput.inputType = TMP_InputField.InputType.Password;
        _passwordInput.textComponent = CreateInputText(inputObj);
        _passwordInput.textViewport = rectTransform;
        _passwordInput.selectionColor = new Color(0.5f, 0.5f, 1f, 0.5f);
    }
    
    private TextMeshProUGUI CreateInputText(GameObject parent)
    {
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(parent.transform, false);
        
        RectTransform rectTransform = textObj.AddComponent<RectTransform>();
        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.sizeDelta = Vector2.zero;
        
        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = "";
        text.fontSize = 36;
        text.color = Color.white;
        
        return text;
    }
    
    private void CreateFeedbackText()
    {
        GameObject textObj = new GameObject("FeedbackText");
        textObj.transform.SetParent(transform, false);
        
        RectTransform rectTransform = textObj.AddComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0, -50);
        rectTransform.sizeDelta = new Vector2(600, 80);
        
        _feedbackText = textObj.AddComponent<TextMeshProUGUI>();
        _feedbackText.text = "Enter password:";
        _feedbackText.fontSize = 32;
        _feedbackText.alignment = TextAlignmentOptions.Center;
        _feedbackText.color = Color.yellow;
    }
    
    private void CreateSubmitButton()
    {
        GameObject buttonObj = new GameObject("SubmitButton");
        buttonObj.transform.SetParent(transform, false);
        
        RectTransform rectTransform = buttonObj.AddComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(-150, -120);
        rectTransform.sizeDelta = new Vector2(200, 60);
        
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.8f, 0.2f);
        
        _submitButton = buttonObj.AddComponent<Button>();
        _submitButton.targetGraphic = buttonImage;
        
        // Add button text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchoredPosition = Vector2.zero;
        textRect.sizeDelta = Vector2.zero;
        
        TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = "SUBMIT";
        buttonText.fontSize = 28;
        buttonText.alignment = TextAlignmentOptions.Center;
    }
    
    private void CreateCancelButton()
    {
        GameObject buttonObj = new GameObject("CancelButton");
        buttonObj.transform.SetParent(transform, false);
        
        RectTransform rectTransform = buttonObj.AddComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(150, -120);
        rectTransform.sizeDelta = new Vector2(200, 60);
        
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.8f, 0.2f, 0.2f);
        
        _cancelButton = buttonObj.AddComponent<Button>();
        _cancelButton.targetGraphic = buttonImage;
        
        // Add button text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchoredPosition = Vector2.zero;
        textRect.sizeDelta = Vector2.zero;
        
        TextMeshProUGUI buttonText = textObj.AddComponent<TextMeshProUGUI>();
        buttonText.text = "CANCEL";
        buttonText.fontSize = 28;
        buttonText.alignment = TextAlignmentOptions.Center;
    }
}
