using UnityEngine;

public class ElevatorButton : MonoBehaviour
{
    public ElevatorDoors doors;
    public bool open = true;
    public bool twoWay = true;
    public Color on;
    public Color off;
	
	void Update ()
    {
	    if (open && doors.tryOpen)
	    {
            renderer.material.color = on;
	    }
        if (open && doors.open)
        {
            renderer.material.color = off;
            if (twoWay) open = false;
        }
        if (!open && doors.tryClose)
        {
            renderer.material.color = on;
        }
        if (!open && doors.closed)
        {
            renderer.material.color = off;
            if (twoWay) open = true;
        }
	}

    public void Push()
    {
        if (open)
        {
            doors.Open();
        }
        else
        {
            doors.Close();
        }
    }
}
