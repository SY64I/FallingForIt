using UnityEngine;

public class CameraRig : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float alignSpeed = 10f;

    void LateUpdate()
    {
        transform.position = player.position;

        Vector3 up = -Physics.gravity.normalized;
        Quaternion target = Quaternion.FromToRotation(transform.up, up) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, target, alignSpeed * Time.deltaTime);
    }
}
