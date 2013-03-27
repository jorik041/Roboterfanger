using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform robot;
    public Transform robot1;
    public GUIText display;
    public float robotHeight = 0.1f;

    private RaycastHit hit;
    private Transform cam;
    private int robotCount = 0;

    void Start()
    {
        cam = Camera.main.transform;
    }

    void Update()
    {
        if (Physics.Raycast(cam.position, cam.forward, out hit))
        {
            if (Input.GetMouseButton(0))
            {
                if (hit.transform.tag != "Robot")
                {
                    var normal = hit.normal;
                    var forward = cam.up;
                    Vector3.OrthoNormalize(ref normal, ref forward);
                    Instantiate(robot, hit.point + normal*robotHeight/2, Quaternion.LookRotation(forward, normal));
                    robotCount++;
                }
            }
            Debug.DrawRay(hit.point, hit.normal, Color.green);
        }
        display.text = robotCount.ToString();
    }
}
