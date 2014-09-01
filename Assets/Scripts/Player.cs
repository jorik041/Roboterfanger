using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public LayerMask floorLayer;
    public LayerMask robotLayer;
    public LayerMask buttonLayer;
    public GameObject targetPrefab;
    public GUIText robotsCount;

    private Transform cam;
    private Transform tr;
    private RaycastHit hit;
    private Transform target;
    private List<CleanerRobot> robots; 

	void Start ()
	{
        robots = new List<CleanerRobot>();
	    cam = Camera.main.transform;
	    tr = transform;
	}
	
	void Update ()
    {
        if (Input.GetKeyDown("e"))
        {
            var rayHit = new RaycastHit();
            if (Physics.Raycast(cam.position, cam.forward, out rayHit, 2, buttonLayer.value))
            {
                rayHit.transform.GetComponent<ElevatorButton>().Push();
            }
        }

        if (Input.GetMouseButton(0))
        {
            if (robots.Count > 0)
            {
                if (Physics.Raycast(cam.position, cam.forward, out hit, 100, floorLayer.value))
                {
                    if (target == null)
                    {
                        target = (Instantiate(targetPrefab, hit.point, Quaternion.LookRotation(hit.normal, cam.forward)) as GameObject).transform;
                    }
                    else
                    {
                        target.position = hit.point;
                        target.rotation = Quaternion.LookRotation(hit.normal, cam.forward);
                    }
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (robots.Count > 0)
            {
                foreach (var robot in robots)
                {
                    robot.SetTarget(hit.point);
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (Physics.Raycast(cam.position, cam.forward, out hit, 100, robotLayer.value))
            {
                var robot = hit.transform.GetComponent<CleanerRobot>();
                if (robot != null)
                {
                    if (!robots.Contains(robot))
                    {
                        robots.Add(robot);
                        robot.Select();
                    }
                    else
                    {
                        robots.Remove(robot);
                        robot.Deselect();
                    }
                }
            }

            //var colliders = Physics.OverlapSphere(tr.position, 5, robotLayer.value);
            //foreach (var col in colliders)
            //{
            //    var robot = col.GetComponent<CleanerRobot>();
            //    if (robot != null)
            //    {
            //        if (!robots.Contains(robot))
            //        {
            //            robots.Add(robot);
            //        }
            //        robot.Select();
            //    }
            //}
        }

	    robotsCount.text = robots.Count.ToString();
    }
}
