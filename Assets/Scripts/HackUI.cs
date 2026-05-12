using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HackUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _promptObject;
    [SerializeField] private GameObject _progressBarObject;
    [SerializeField] private Image _progressFill;
    [SerializeField] private TextMeshProUGUI _hackCountText;

    private void Start()
    {
        if (_progressFill != null)
            _progressFill.fillAmount = 0f;

        ShowPrompt(false);
        ShowProgressBar(false);
    }

    public void ShowPrompt(bool show)
    {
        if (_promptObject != null)
            _promptObject.SetActive(show);
    }

    public void ShowProgressBar(bool show)
    {
        if (_progressBarObject != null)
            _progressBarObject.SetActive(show);
    }

    public void SetProgress(float value, int hacksCompleted, int totalHacks)
    {
        if (_progressFill != null)
            _progressFill.fillAmount = value;

        if (_hackCountText != null)
            _hackCountText.text = $"Hack {Mathf.Min(hacksCompleted, totalHacks)} / {totalHacks}";
    }
}