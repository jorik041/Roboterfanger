using UnityEngine;

public class Destination : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Mastermind mastermind = FindObjectOfType(typeof (Mastermind)) as Mastermind;
        mastermind.KillRobot(other.transform);
	}
}
