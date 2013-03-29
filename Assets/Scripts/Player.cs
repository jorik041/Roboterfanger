using UnityEngine;

public class Player : MonoBehaviour
{
    public Transform robot;
    public GUIText display;
    public float robotHeight = 0.14f;
    public LayerMask wallLayer;
    public LayerMask robotLayer;

    private RaycastHit hit;
    private Transform cam;
    private int robotCount;
    private AsyncOperation nextScene;
    private bool loading;

    void Start()
    {
        cam = Camera.main.transform;
    }

    void Update()
    {
        if (Physics.Raycast(cam.position, cam.forward, out hit, 100, wallLayer.value) &&
            !Physics.Raycast(cam.position, cam.forward, 100, robotLayer.value))
        {
            if (Input.GetMouseButton(0))
            {
                var normal = hit.normal;
                var forward = cam.up;
                Vector3.OrthoNormalize(ref normal, ref forward);
                Instantiate(robot, hit.point + normal * robotHeight / 2, Quaternion.LookRotation(forward, normal));
                robotCount++;
                display.text = robotCount.ToString();
            }
            Debug.DrawRay(hit.point, hit.normal, Color.green);
        }
        if (Input.GetMouseButtonDown(1) && !loading)
        {
            print("loading");
            nextScene = Application.LoadLevelAdditiveAsync(1);
            loading = true;
        }
        if (loading)
        {
            print(nextScene.progress);
        }
    }
}
