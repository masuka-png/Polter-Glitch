using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class WinSequence : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private Camera _cinematicCamera;

    [Header("Timing")]
    [SerializeField] private float _cinematicDuration = 4f;
    [SerializeField] private float _fadeDuration = 1.5f;

    [Header("Win Screen")]
    [SerializeField] private string _winText = "You Win";
    [SerializeField] private float _textFadeDelay = 0.5f;
    [SerializeField] private TMP_FontAsset _winFont;
    [SerializeField] private Color _winTextColor = Color.white;
    [SerializeField] private bool _bold = false;
    [SerializeField] private bool _italic = false;

    private Canvas _canvas;
    private Image _fadeImage;
    private TextMeshProUGUI _winLabel;

    private void Start()
    {
        BuildWinUI();

        if (_cinematicCamera != null)
            _cinematicCamera.gameObject.SetActive(false);
    }

    private void BuildWinUI()
    {
        GameObject canvasObj = new GameObject("WinCanvas");
        _canvas = canvasObj.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _canvas.sortingOrder = 99;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // Black fade panel
        GameObject panelObj = new GameObject("FadePanel");
        panelObj.transform.SetParent(canvasObj.transform, false);
        _fadeImage = panelObj.AddComponent<Image>();
        _fadeImage.color = new Color(0, 0, 0, 0);
        RectTransform rt = _fadeImage.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        // Win text
        GameObject textObj = new GameObject("WinText");
        textObj.transform.SetParent(canvasObj.transform, false);
        _winLabel = textObj.AddComponent<TextMeshProUGUI>();
        _winLabel.text = _winText;
        _winLabel.fontSize = 72;
        _winLabel.alignment = TextAlignmentOptions.Center;
        _winLabel.color = new Color(_winTextColor.r, _winTextColor.g, _winTextColor.b, 0);

        // Apply bold and italic
        if (_bold && _italic)
            _winLabel.fontStyle = FontStyles.Bold | FontStyles.Italic;
        else if (_bold)
            _winLabel.fontStyle = FontStyles.Bold;
        else if (_italic)
            _winLabel.fontStyle = FontStyles.Italic;
        else
            _winLabel.fontStyle = FontStyles.Normal;

        RectTransform textRt = _winLabel.rectTransform;
        textRt.anchorMin = new Vector2(0, 0.4f);
        textRt.anchorMax = new Vector2(1, 0.6f);
        textRt.offsetMin = Vector2.zero;
        textRt.offsetMax = Vector2.zero;

        if (_winFont != null)
            _winLabel.font = _winFont;

        // Disable until win sequence triggers
        canvasObj.SetActive(false);
    }

    public void TriggerWinSequence()
    {
        StartCoroutine(WinSequenceRoutine());
    }

    private IEnumerator WinSequenceRoutine()
    {
        _canvas.gameObject.SetActive(true);

        if (_playerCamera != null) _playerCamera.gameObject.SetActive(false);
        if (_cinematicCamera != null) _cinematicCamera.gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        yield return new WaitForSeconds(_cinematicDuration);

        // Fade to black
        float elapsed = 0f;
        while (elapsed < _fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / _fadeDuration);
            _fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }

        _fadeImage.color = new Color(0, 0, 0, 1);

        yield return new WaitForSeconds(_textFadeDelay);

        // Fade in text
        elapsed = 0f;
        while (elapsed < _fadeDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / _fadeDuration);
            _winLabel.color = new Color(_winTextColor.r, _winTextColor.g, _winTextColor.b, alpha);
            yield return null;
        }

        _winLabel.color = new Color(_winTextColor.r, _winTextColor.g, _winTextColor.b, 1);
    }
}