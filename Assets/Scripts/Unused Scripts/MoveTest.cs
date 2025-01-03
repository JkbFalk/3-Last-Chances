using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MoveTest : MonoBehaviour
{
    public Vector2 orgPos;
    int updateFrames = 0;
    int fixedUpdateFrames = 0;
    public float distance = 0;
    public bool moving = false;
    public bool lateUpdate = false;
    public bool fixedUpdate = false;
    public bool fixedDelta = false;
    public bool timeDelta = false;
    public float deltaTimeDistance = 0;
    public float deltaTimeDistanceTotal = 0;
    public float fixedTimeDistance = 0;
    public float fixedTimeDistanceTotal = 0;

    private void Start()
    {
        orgPos = transform.position;
    }

    void LateUpdate()
    {
        if (moving)
        {
            updateFrames++;
        }
        if (moving && lateUpdate)
        {
            GetComponent<Rigidbody2D>().MovePosition(transform.position + (Vector3.right * 0.1f * (fixedDelta ? Time.fixedDeltaTime : 1) * (timeDelta ? Time.deltaTime : 1)));
            deltaTimeDistance = 0.1f * (fixedDelta ? Time.fixedDeltaTime : 1) * (timeDelta ? Time.deltaTime : 1);
            deltaTimeDistanceTotal += deltaTimeDistance;

        }
        GetComponentInChildren<TextMeshProUGUI>().text = "Frames: " + updateFrames + "\nFixed Frames: " + fixedUpdateFrames + "\nDistance per Frame: " + deltaTimeDistance +"\nDistance per Frame Total: " + deltaTimeDistanceTotal + "\nDistance per Fixed Frame: " + fixedTimeDistance + "\nDistance per Fixed Frame Total: " + fixedTimeDistanceTotal + "\nDistance: " + Vector2.Distance(orgPos, transform.position).ToString();
    }

    void FixedUpdate()
    {
        if(moving)
        {
            fixedUpdateFrames++;
        }
        if (moving && fixedUpdate)
        {
            GetComponent<Rigidbody2D>().MovePosition(transform.position + Vector3.right * Time.fixedDeltaTime);
            fixedTimeDistance = Time.fixedDeltaTime;
            fixedTimeDistanceTotal += fixedTimeDistance;
        }
    }
}
