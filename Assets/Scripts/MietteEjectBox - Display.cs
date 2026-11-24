using UnityEngine;
using System.Collections;

public class MietteEjectBoxDisplay : MonoBehaviour
{
    [SerializeField] Transform camObj;
    Vector3 camStartPos;
    [SerializeField] Transform camMovePoint;
    private float elapsedTime;
    [SerializeField] float moveDuration;
    bool camIsMoving = false;

    [SerializeField] AnimationCurve moveCurve;

    [SerializeField] GameObject text;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (camIsMoving)
        {
            elapsedTime += Time.deltaTime;
            float percentageComplete = elapsedTime / moveDuration;
            camObj.position = Vector3.Lerp(camStartPos, camMovePoint.position, moveCurve.Evaluate(percentageComplete));
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            // Start level complete sequence
            camStartPos = camObj.position;
            GameManager.instance.Eject();
            camIsMoving = true;

            text.SetActive(true);
        }
    }
    /*
    IEnumerator MoveCamera()
    {

    }
    */
}
