using UnityEngine;
using UnityEngine.UI;
using EasyPeasyFirstPersonController;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private GameObject attackUI;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private PlatformManager platformManager;
    [SerializeField] private GraphicRaycaster hackUIRaycaster;

    private FirstPersonController _fpc;
    private bool _isUIActive;

    private void Awake()
    {
        _fpc = GetComponent<FirstPersonController>();
    }

    public void ShowAttackUI()
    {
        if (_isUIActive) return;

        _isUIActive = true;

        if (attackUI != null)
            attackUI.SetActive(true);

        if (characterController != null)
            characterController.enabled = false;

        if (hackUIRaycaster != null)
            hackUIRaycaster.enabled = false;

        if (_fpc != null)
            _fpc.EnterUIMode();
    }

    public void HideAttackUI()
    {
        if (!_isUIActive) return;

        _isUIActive = false;

        if (attackUI != null)
            attackUI.SetActive(false);

        if (characterController != null)
            characterController.enabled = true;

        if (hackUIRaycaster != null)
            hackUIRaycaster.enabled = true;

        if (_fpc != null)
            _fpc.ExitUIMode();
    }

    public void OnContinuePressed()
    {
        HideAttackUI();

        if (platformManager != null)
            platformManager.RespawnPlayer();
    }

    public void OnMainMenuPressed()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}