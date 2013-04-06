using System.Globalization;
using UnityEngine;
using System.Collections.Generic;

public class Mastermind : MonoBehaviour
{
    public Transform playerPrefab;
    public Transform robotPrefab;
    public Transform destinationPrefab;
    public Transform hudPrefab;
    public LayerMask wallsLayer;
    public LayerMask robotsLayer;

    //[Range(0.1f, 0.2f)]
    public float robotHeight = 0.14f;

    private Transform respawn;
    private Transform player;
    private Transform cam;
    private List<Transform> robots = new List<Transform>();
    private GUIText robotsDisplay;
    private RaycastHit hit;
    private Transform destination;
    private Vector3 destinationPoint;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        respawn = GameObject.FindGameObjectWithTag("Respawn").transform;
        player = Instantiate(playerPrefab, respawn.position, respawn.rotation) as Transform;
        cam = Camera.main.transform;
        var hud = Instantiate(hudPrefab) as Transform;
        robotsDisplay = hud.FindChild("Robots display").GetComponent<GUIText>();
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
                if (destination != null) robot.GetComponent<SpaceRobot>().SetTarget(destination);
            }
            //Debug.DrawRay(hit.point, hit.normal, Color.green);

            if (Input.GetMouseButton(1))
            {
                destinationPoint = hit.point;
                if (destination != null) destination.position = destinationPoint;
                else destination = Instantiate(destinationPrefab, destinationPoint, Quaternion.identity) as Transform;
            }
            if(Input.GetMouseButtonUp(1))
            {
                foreach (var robot in robots)
                {
                    robot.GetComponent<SpaceRobot>().SetTarget(destination);
                }
            }

            if (Input.GetMouseButtonDown(2))
            {
                foreach (var robot in robots)
                {
                    robot.GetComponent<SpaceRobot>().RemoveTarget();
                }
                Destroy(destination.gameObject);
            }
        }

        robotsDisplay.text = robots.Count.ToString();
    }

    public void KillRobot(Transform sacrifice)
    {
        robots.Remove(sacrifice);
        Destroy(sacrifice.gameObject);
    }
}
