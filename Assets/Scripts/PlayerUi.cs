using UnityEngine;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private GameObject attackUI;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private PlatformManager platformManager;

    private bool _isUIActive;

    public void ShowAttackUI()
    {
        if (_isUIActive) return;

        _isUIActive = true;

        if (attackUI != null)
            attackUI.SetActive(true);

        if (characterController != null)
            characterController.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void HideAttackUI()
    {
        if (!_isUIActive) return;

        _isUIActive = false;

        if (attackUI != null)
            attackUI.SetActive(false);

        if (characterController != null)
            characterController.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Wire this to the Continue button's OnClick in the Inspector
    public void OnContinuePressed()
    {
        HideAttackUI();

        if (platformManager != null)
            platformManager.RespawnPlayer();
    }

    // Wire this to the Main Menu button's OnClick in the Inspector
    public void OnMainMenuPressed()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}