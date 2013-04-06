using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Seeker))]
public class RobotAI : MonoBehaviour
{
    public float movementSpeed = 0.1f;
    public float turningSpeed = 0.1f;
    public float gravity = 0.1f;
    public float nextWaypointDistance = 0.5f;
    public Transform target;
    public LayerMask wallsLayer;

    private Transform tr;
    private CharacterController controller;
    private Seeker seeker;
    private Vector3 targetDirection;
    private Path path;
    private int currentWaypoint;
    private RaycastHit hit;
    private GameObject wall;
    private Vector3 wallNormal;
    private Vector3 wallPoint;
    private float wallDistance;

    void Awake()
    {
        tr = transform;
        controller = GetComponent<CharacterController>();
        seeker = GetComponent<Seeker>();
    }

    void Start()
    {
        if(target != null) RecalculatePath();
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
        else
        {
            Debug.Log("Path error");
        }
    }

    void Update()
    {
        //Debug.DrawRay(wallPoint, wallNormal, Color.black);
    }

    void FixedUpdate()
    {
        if (path == null) return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            Debug.Log("End of path reached");
            path = null;
            return;
        }

        // ���� ����� � ������ � ��� ����� �����, ����� ����
        if (Physics.Raycast(tr.position, -tr.up, out hit, 100, wallsLayer.value))
        {
            wallPoint = hit.point;
            wallNormal = hit.normal;
            wallDistance = hit.distance;

            if (wallNormal != tr.up && wallNormal != Vector3.zero)
            {
                RotateTowards(wallNormal, tr.forward);
            }
            if (wallDistance > (0.06f + gravity * Time.fixedDeltaTime))
            {
                controller.Move(-tr.up * gravity * Time.fixedDeltaTime);
            }
        }
        else
        {
            controller.Move(tr.up * gravity * Time.fixedDeltaTime);
        }

        Debug.DrawRay(path.vectorPath[currentWaypoint], Vector3.up, Color.magenta);
        // �������������� � ����
        targetDirection = (path.vectorPath[currentWaypoint] - tr.position).normalized;
        if (targetDirection != tr.forward && targetDirection != Vector3.zero)
        {
            RotateTowards(targetDirection, tr.up);
        }

        // ���� �����
        Debug.DrawRay(tr.position, tr.forward, Color.blue);
        Debug.DrawRay(wallPoint, wallNormal, Color.black);
        if(Input.GetKeyDown("e"))
            controller.Move(tr.forward * movementSpeed * Time.fixedDeltaTime);

        if (Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }

    /// <summary>
    /// ���������� ������ direction �� ���������, ������������� reference.
    /// ������ ������� �� �������� � �����������.
    /// </summary>
    /// <param name="direction">����������� ��������.</param>
    /// <param name="reference">������, ������������� ��������� �������������.</param>
    void RotateTowards(Vector3 direction, Vector3 reference)
    {
        Quaternion currentRotation = tr.rotation;
        Vector3.OrthoNormalize(ref reference, ref direction);
        Quaternion newRotation = Quaternion.LookRotation(direction, reference);
        tr.rotation = Quaternion.Slerp(currentRotation, newRotation, turningSpeed * Time.fixedDeltaTime);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        RecalculatePath();
    }

    void RecalculatePath()
    {
        seeker.StartPath(tr.position, target.position, OnPathComplete);
    }
}
