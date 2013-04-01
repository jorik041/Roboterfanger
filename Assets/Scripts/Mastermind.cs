using UnityEngine;
using System.Collections.Generic;

public class Mastermind : MonoBehaviour
{

    public Transform player;
    public Transform robotPrefab;
    public Transform destinationPrefab;
    public GUIText display;
    public LayerMask wallsLayer;
    public LayerMask robotsLayer;
    public float robotHeight = 0.14f;

    private Transform cam;
    private List<Transform> robots = new List<Transform>();
    private RaycastHit hit;
    private Transform destination;
    private Vector3 destinationPoint;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        cam = Camera.main.transform;
    }

    void Update()
    {
        if (Physics.Raycast(cam.position, cam.forward, out hit, 100, wallsLayer.value) &&
            !Physics.Raycast(cam.position, cam.forward, 100, robotsLayer.value))
        {
            if (Input.GetMouseButton(0))
            {
                var normal = hit.normal;
                var forward = cam.up;
                Vector3.OrthoNormalize(ref normal, ref forward);
                var robot =
                    Instantiate(robotPrefab, hit.point + normal*robotHeight/2, Quaternion.LookRotation(forward, normal))
                    as Transform;
                robots.Add(robot);
                if (destination != null) robot.GetComponent<Robot>().SetDestination(destinationPoint);
                display.text = robots.Count.ToString();
            }
            Debug.DrawRay(hit.point, hit.normal, Color.green);

            if (Input.GetMouseButton(1))
            {
                destinationPoint = hit.point;
                if (destination != null) destination.position = destinationPoint;
                else destination = Instantiate(destinationPrefab, destinationPoint, Quaternion.identity) as Transform;

                foreach (var robot in robots)
                {
                    robot.GetComponent<Robot>().SetDestination(destinationPoint);
                }
            }
            if (Input.GetMouseButtonDown(2))
            {
                foreach (var robot in robots)
                {
                    robot.GetComponent<Robot>().RemoveDestination();
                }
                Destroy(destination.gameObject);
            }
        }
    }

    public void KillRobot(Transform sacrifice)
    {
        robots.Remove(sacrifice);
        Destroy(sacrifice.gameObject);
    }
}
