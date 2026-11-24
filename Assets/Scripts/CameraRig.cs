using UnityEngine;
using System;
using Unity.Cinemachine;

public class CameraRig : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] float alignSpeed = 10f;

    [SerializeField] CinemachineCamera cineCam;
    [SerializeField] CinemachineOrbitalFollow cineFollow;
    [SerializeField] CinemachineDeoccluder cineOcclude;

    bool isEjecting;

    [SerializeField] Transform monsterLookAt;
    [SerializeField] Transform playerLookAt;

    private void Start()
    {
        GameManager.instance.mietteEjected += AttachCamToMonster;
        GameManager.instance.cancelEject += ReattachCamToPlayer;
    }
    void LateUpdate()
    {
        if (!isEjecting)
        {
            transform.position = player.position;

            Vector3 up = -Physics.gravity.normalized;
            Quaternion target = Quaternion.FromToRotation(transform.up, up) * transform.rotation;
            transform.rotation = Quaternion.Slerp(transform.rotation, target, alignSpeed * Time.deltaTime);
        }
    }

    void AttachCamToMonster()
    {
        isEjecting = true;
        cineFollow.enabled = false;
        cineOcclude.enabled = false;

        cineCam.LookAt = monsterLookAt;

    }

    private void OnDisable()
    {
        if (isEjecting)
        {
            // Camera Rig object only gets disabled at runtime by player falling.

            // Assume that player must have fallen out of the map while ejecting miette, so reset values.
            ReattachCamToPlayer();
        }
    }

    void ReattachCamToPlayer()
    {
        cineCam.LookAt = playerLookAt;

        cineFollow.enabled = true;
        cineOcclude.enabled = true;
        isEjecting = false;
    }
}
