using UnityEngine;

public class GravityDirection : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float rayDistance = 100f;
    
    private readonly Vector3[] directions =
    {
        Vector3.up,
        Vector3.down,
        Vector3.right,
        Vector3.left,
        Vector3.forward,
        Vector3.back
    };

    public bool GetGravityDirection(out Vector3 gravityDirection)
    {
        gravityDirection = Vector3.down;

        if (playerCamera == null)
        {
            Debug.LogError("Player Camera is not assigned!");
            return false;
        }

        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        Debug.DrawRay(
            ray.origin,
            ray.direction * rayDistance,
            Color.red,
            2f
        );

        int playerLayer = LayerMask.NameToLayer("Player");
        
        if (playerLayer == -1)
        {
            Debug.LogError(
                "The 'Player' layer does not exist. Create it in " +
                "Unity's Layer settings and assign it to your Player."
            );
            return false;
        }

        int layerMask = ~(1 << playerLayer);

        if (!Physics.Raycast(
            ray,
            out RaycastHit hit,
            rayDistance,
            layerMask,
            QueryTriggerInteraction.Ignore))
        {
            Debug.LogWarning("Raycast missed all objects.");
            return false;
        }

        Debug.Log("Ray hit: " + hit.collider.name);
        Debug.Log("HIT OBJECT: " + hit.collider.name);
        Debug.Log("HIT POINT: " + hit.point);
        Debug.Log("HIT DISTANCE: " + hit.distance);

        Vector3 normal = hit.normal.normalized;

        float bestDot = float.NegativeInfinity;
        Vector3 bestDirection = Vector3.down;

        foreach (Vector3 direction in directions)
        {
            float dot = Vector3.Dot(normal, direction);

            if (dot > bestDot)
            {
                bestDot = dot;
                bestDirection = direction;
            }
        }

        gravityDirection = -bestDirection;

        Debug.Log("Selected gravity direction: " + gravityDirection);

        return true;
    }
}
