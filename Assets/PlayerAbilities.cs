using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    [SerializeField] private float invisDuration = 3f;
    [SerializeField] private float cooldown = 8f;

    private bool _isInvisible;
    private float _cooldownTimer;
    private float _invisTimer;

    public bool IsInvisible => _isInvisible;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && _cooldownTimer <= 0f)
        {
            _isInvisible = true;
            _invisTimer = invisDuration;
            _cooldownTimer = cooldown;
        }

        if (_isInvisible)
        {
            _invisTimer -= Time.deltaTime;
            if (_invisTimer <= 0f)
                _isInvisible = false;
        }

        if (_cooldownTimer > 0f)
            _cooldownTimer -= Time.deltaTime;
    }
}