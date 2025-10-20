using UnityEngine;
using Unity.AI.Navigation;

public class BuildNavAtRuntime : MonoBehaviour
{
    void Awake()
    {
        var s = GetComponent<NavMeshSurface>();
        if (s) s.BuildNavMesh();
    }
}
