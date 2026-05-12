using UnityEngine;
using Unity.AI.Navigation;

public class NavMeshBaker : MonoBehaviour
{
    private NavMeshSurface _surface;

    void Awake()
    {
        _surface = GetComponent<NavMeshSurface>();
        _surface.BuildNavMesh();
    }
}