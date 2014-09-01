using UnityEngine;

public class ElevatorFinish : MonoBehaviour
{

    public ElevatorDoors doors;

    private bool playerIn;
    private int robots;

	void Update()
    {
	    if (playerIn && robots <= 0 && doors.closed)
	    {
	        Application.LoadLevel(Application.loadedLevel + 1);
	    }
	}

    void OnTriggerEnter(Collider other)
    {
	    if (other.tag == "Player")
	    {
	        playerIn = true;
	    }
        if (other.tag == "Robot")
        {
            robots++;
        }
	}

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            playerIn = false;
        }
        if (other.tag == "Robot")
        {
            robots--;
        }
    }
}
