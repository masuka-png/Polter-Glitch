using UnityEngine;
using TMPro;
using System;

public class KeyboardUI : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private GameObject _passwordPromptPrefab;
    
    private PasswordPromptPanel _currentPrompt;
    
    private void Start()
    {
        if (_canvas == null)
        {
            _canvas = FindObjectOfType<Canvas>();
        }
        
        if (_passwordPromptPrefab == null)
        {
            Debug.LogWarning("Password prompt prefab not assigned!");
        }
    }
    
    public void ShowPasswordPrompt(string keyboardName, string correctPassword, Action<bool> onComplete)
    {
        if (_passwordPromptPrefab == null)
        {
            Debug.LogError("Password prompt prefab not assigned!");
            onComplete?.Invoke(false);
            return;
        }
        
        GameObject promptObj = Instantiate(_passwordPromptPrefab, _canvas.transform);
        _currentPrompt = promptObj.GetComponent<PasswordPromptPanel>();
        
        if (_currentPrompt != null)
        {
            _currentPrompt.Initialize(keyboardName, correctPassword, onComplete);
        }
    }
}

/// <summary>
/// UI Panel for the password prompt
/// </summary>
public class PasswordPromptPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TMP_InputField _passwordInput;
    [SerializeField] private TextMeshProUGUI _feedbackText;
    [SerializeField] private UnityEngine.UI.Button _submitButton;
    [SerializeField] private UnityEngine.UI.Button _cancelButton;
    
    private string _correctPassword;
    private Action<bool> _onComplete;
    
    private void Start()
    {
        if (_submitButton != null)
            _submitButton.onClick.AddListener(OnSubmit);
        
        if (_cancelButton != null)
            _cancelButton.onClick.AddListener(OnCancel);
        
        if (_passwordInput != null)
        {
            _passwordInput.contentType = TMP_InputField.ContentType.Password;
            _passwordInput.inputType = TMP_InputField.InputType.Password;
            _passwordInput.onEndEdit.AddListener(OnPasswordChanged);
        }
    }
    
    public void Initialize(string keyboardName, string correctPassword, Action<bool> onComplete)
    {
        _correctPassword = correctPassword;
        _onComplete = onComplete;
        
        if (_titleText != null)
            _titleText.text = keyboardName;
        
        if (_passwordInput != null)
            _passwordInput.text = "";
        
        if (_feedbackText != null)
            _feedbackText.text = "Enter password:";
    }
    
    private void OnSubmit()
    {
        if (_passwordInput == null)
            return;
        
        bool correct = _passwordInput.text == _correctPassword;
        
        if (correct)
        {
            if (_feedbackText != null)
                _feedbackText.text = "Access granted!";
            _onComplete?.Invoke(true);
        }
        else
        {
            if (_feedbackText != null)
                _feedbackText.text = "Access denied! Try again.";
            if (_passwordInput != null)
                _passwordInput.text = "";
        }
        
        if (correct)
        {
            Destroy(gameObject, 1f);
        }
    }
    
    private void OnCancel()
    {
        _onComplete?.Invoke(false);
        Destroy(gameObject);
    }
    
    private void OnPasswordChanged(string newPassword)
    {
        // Optional: Validate in real-time
    }
}
