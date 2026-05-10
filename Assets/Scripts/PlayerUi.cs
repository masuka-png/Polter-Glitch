using UnityEngine;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private GameObject attackUI;
    [SerializeField] private CharacterController characterController;

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
}