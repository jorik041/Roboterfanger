using UnityEngine;

public class Destination : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Robot")
        {
            Mastermind mastermind = FindObjectOfType(typeof(Mastermind)) as Mastermind;
            mastermind.KillRobot(other.transform);
        }
	}
}
