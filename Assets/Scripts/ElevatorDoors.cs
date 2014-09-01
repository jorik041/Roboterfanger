using UnityEngine;

public class ElevatorDoors : MonoBehaviour
{
    public Transform leftDoor;
    public Transform rightDoor;
    public Vector3 leftClosed;
    public Vector3 leftOpen;
    public Vector3 rightClosed;
    public Vector3 rightOpen;
    public float closeSpeed = 10;
    public float openSpeed = 10;
    public bool closed;
    public bool open;
    [HideInInspector]
    public bool tryOpen;
    [HideInInspector]
    public bool tryClose;

    void Start()
    {
        if(open) Open();
        if(closed) Close();
    }

	void Update ()
    {
        if ((leftDoor.position - leftOpen).sqrMagnitude < 0.1f && (rightDoor.position - rightOpen).sqrMagnitude < 0.1f)
	    {
	        open = true;
	    }
        else
        {
            open = false;
        }
        if ((leftDoor.position - leftClosed).sqrMagnitude < 0.1f && (rightDoor.position - rightClosed).sqrMagnitude < 0.1f)
        {
            closed = true;
        }
        else
        {
            closed = false;
        }
	}
	
	void FixedUpdate ()
    {
	    if (tryClose)
	    {
            leftDoor.rigidbody.MovePosition(Vector3.Slerp(leftDoor.position, leftClosed, Time.deltaTime * closeSpeed));
            rightDoor.rigidbody.MovePosition(Vector3.Slerp(rightDoor.position, rightClosed, Time.deltaTime * closeSpeed));
	    }
        if (tryOpen)
        {
            leftDoor.rigidbody.MovePosition(Vector3.Slerp(leftDoor.position, leftOpen, Time.deltaTime * openSpeed));
            rightDoor.rigidbody.MovePosition(Vector3.Slerp(rightDoor.position, rightOpen, Time.deltaTime * openSpeed));
        }
	}

    public void Open()
    {
        tryOpen = true;
        tryClose = false;
    }

    public void Close()
    {
        tryOpen = false;
        tryClose = true;
        closed = false;
    }
}
