using UnityEngine;
using System.Collections.Generic;

public class EnemyResetter : MonoBehaviour
{
    public static List<EnemyResetter> AllEnemies = new List<EnemyResetter>();

    private Vector3 _startPosition;
    private Quaternion _startRotation;

    private void Awake()
    {
        _startPosition = transform.position;
        _startRotation = transform.rotation;
        AllEnemies.Add(this);
    }

    private void OnDestroy()
    {
        AllEnemies.Remove(this);
    }

    public void ResetToStart()
    {
        transform.position = _startPosition;
        transform.rotation = _startRotation;
    }
}