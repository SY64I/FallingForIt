using System;
using UnityEngine;
using Unity.AI.Navigation;

public class GravityNavMeshManager : MonoBehaviour
{
    [Header("NavMesh Surfaces")]
    [SerializeField] private NavMeshSurface gravityDownSurface;
    [SerializeField] private NavMeshSurface gravityUpSurface;
    [SerializeField] private NavMeshSurface gravityRightSurface;
    [SerializeField] private NavMeshSurface gravityLeftSurface;
    [SerializeField] private NavMeshSurface gravityForwardSurface;
    [SerializeField] private NavMeshSurface gravityBackSurface;

    public event Action GravityChanging;
    public event Action GravityChanged;

    public bool IsInitialized { get; private set; }

    private NavMeshSurface activeSurface;

    private void Awake()
    {
        // Start with every NavMesh disabled; the active one is enabled on initialization
        SetSurfaceEnabled(gravityDownSurface, false);
        SetSurfaceEnabled(gravityUpSurface, false);
        SetSurfaceEnabled(gravityRightSurface, false);
        SetSurfaceEnabled(gravityLeftSurface, false);
        SetSurfaceEnabled(gravityForwardSurface, false);
        SetSurfaceEnabled(gravityBackSurface, false);
    }

    public void InitializeForGravity(Vector3 gravity)
    {
        NavMeshSurface selectedSurface = GetSurfaceForGravity(gravity);

        if (selectedSurface == null)
        {
            Debug.LogError("No NavMeshSurface assigned for initial gravity!");
            return;
        }

        SetSurfaceEnabled(selectedSurface, true);

        activeSurface = selectedSurface;
        IsInitialized = true;
    }

    public void SetGravityDirection(Vector3 gravity)
    {
        NavMeshSurface selectedSurface = GetSurfaceForGravity(gravity);

        if (selectedSurface == null)
        {
            Debug.LogError("No NavMeshSurface assigned for gravity: " + gravity);
            return;
        }

        if (selectedSurface == activeSurface)
            return;

        // Enemies detach from the old NavMesh before it is removed
        GravityChanging?.Invoke();

        SetSurfaceEnabled(activeSurface, false);
        SetSurfaceEnabled(selectedSurface, true);

        activeSurface = selectedSurface;
        IsInitialized = true;

        GravityChanged?.Invoke();
    }

    private NavMeshSurface GetSurfaceForGravity(Vector3 gravity)
    {
        if (gravity.sqrMagnitude < 0.001f)
            return null;

        Vector3 direction = gravity.normalized;

        float highestDot = Vector3.Dot(direction, Vector3.down);
        NavMeshSurface result = gravityDownSurface;

        void Check(Vector3 axis, NavMeshSurface surface)
        {
            float dot = Vector3.Dot(direction, axis);

            if (dot > highestDot)
            {
                highestDot = dot;
                result = surface;
            }
        }

        Check(Vector3.up, gravityUpSurface);
        Check(Vector3.right, gravityRightSurface);
        Check(Vector3.left, gravityLeftSurface);
        Check(Vector3.forward, gravityForwardSurface);
        Check(Vector3.back, gravityBackSurface);

        return result;
    }

    private void SetSurfaceEnabled(NavMeshSurface surface, bool state)
    {
        if (surface != null)
            surface.enabled = state;
    }

    // Editor helpers

    [ContextMenu("1. Align surface rotations")]
    private void AlignRotations()
    {
        Align(gravityDownSurface, Vector3.down);
        Align(gravityUpSurface, Vector3.up);
        Align(gravityRightSurface, Vector3.right);
        Align(gravityLeftSurface, Vector3.left);
        Align(gravityForwardSurface, Vector3.forward);
        Align(gravityBackSurface, Vector3.back);
    }

    private void Align(NavMeshSurface surface, Vector3 gravityDirection)
    {
        if (surface == null)
            return;

        surface.transform.rotation = Quaternion.FromToRotation(Vector3.up, -gravityDirection);
    }

    [ContextMenu("2. Bake all surfaces")]
    private void BakeAll()
    {
        NavMeshSurface[] surfaces =
        {
            gravityDownSurface,
            gravityUpSurface,
            gravityRightSurface,
            gravityLeftSurface,
            gravityForwardSurface,
            gravityBackSurface
        };

        foreach (NavMeshSurface surface in surfaces)
        {
            if (surface != null)
                surface.BuildNavMesh();
        }
    }
}