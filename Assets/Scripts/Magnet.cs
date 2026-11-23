using UnityEngine;

public class Magnet : MonoBehaviour
{
    [SerializeField] Switch switchAttract;
    [SerializeField] Switch switchRepel;
    [SerializeField] ActivatedMultiLight magnetLight;
    [SerializeField] Rigidbody block;
    [SerializeField] float moveSpeed = 1f;

    // Temporary bool for magnet being on and off
    // bool magnetOn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        magnetLight.attractSwitch = switchAttract;
        magnetLight.repelSwitch = switchRepel;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        /*
        if (switchAttract.activated)
        {
            //block.MovePosition(block.position + transform.up*-1f*moveSpeed);
            block.linearVelocity = transform.up*-1f*moveSpeed;
        }
        else if (switchRepel.activated)
        {
            //block.MovePosition(block.position + transform.up*1f*moveSpeed);
            block.linearVelocity = transform.up*1f*moveSpeed;
        } else
        {
            block.linearVelocity = Vector3.zero;
        }
        */

        Vector3 direction = Vector3.zero;

        if (switchAttract.activated)
        {
            direction = -transform.up;
        }
        else if (switchRepel.activated)
        {
            direction = transform.up;
        }

        if (direction != Vector3.zero)
        {
            float distance = moveSpeed;

            if (block.SweepTest(direction, out RaycastHit hit, distance))
            {
                distance = Mathf.Max(0f, hit.distance - 0.01f);
            }

            block.MovePosition(block.position + direction * distance);
        }
    }
}
