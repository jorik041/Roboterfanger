using UnityEngine;
using Pathfinding;

[RequireComponent(typeof(Seeker))]
[RequireComponent(typeof(Rigidbody))]
public class SpaceRobot : MonoBehaviour
{
    public float movementSpeed = 0.1f;
    public float turningSpeed = 0.1f;
    public float gravity = 0.1f;
    public float nextWaypointDistance = 0.5f;
    public Renderer skin;
    public Transform target;
    
    private Transform tr;
    private Rigidbody rb;
    private Seeker seeker;
    private Path path;
    private int currentWaypoint;
    private Vector3 targetDirection;
    private Material skinMaterial;
    private bool colorMode = true;
    private GameObject wall;
    private Vector3 wallNormal;
    private Vector3 wallPoint;

    void Awake()
    {
        tr = transform;
        rb = rigidbody;
        seeker = GetComponent<Seeker>();
        rb.centerOfMass = new Vector3(0, -0.05f, 0);
        skinMaterial = skin.material;
        wallNormal = tr.up;
        wallPoint = tr.position;
    }

    void Start()
    {
        if (target != null) RecalculatePath();
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
            audio.Play();
        }
        else
        {
            Debug.Log("Path error");
        }
    }

    void FixedUpdate()
    {
        StickToWall();
        FollowTarget();      
    }

    void StickToWall()
    {
        if (wallNormal != tr.up && wallNormal != Vector3.zero)
        {
            RotateTowards(tr.forward, wallNormal);
        }

        if (wall != null) rb.AddForce(-wallNormal * gravity);
        else rb.AddForce(-tr.up * gravity);
    }

    void FollowTarget()
    {
        if (path == null) return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            RemoveTarget();
            return;
        }

        //Debug.DrawRay(path.vectorPath[currentWaypoint], Vector3.up, Color.magenta);
        targetDirection = (path.vectorPath[currentWaypoint] - tr.position).normalized;
        if (targetDirection != tr.forward && targetDirection != Vector3.zero)
        {
            RotateTowards(targetDirection, tr.up);
        }

        MoveForward();

        if (Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }

    /// <summary>
    /// Проецирует вектор direction на плоскость, ортогональную reference.
    /// Делает поворот от проекции к направлению.
    /// </summary>
    /// <param name="direction">Направление поворота.</param>
    /// <param name="reference">Вектор, ортогональный плоскости проецирования.</param>
    void RotateTowards(Vector3 direction, Vector3 reference)
    {
        Quaternion currentRotation = tr.rotation;
        Vector3.OrthoNormalize(ref reference, ref direction);
        Quaternion newRotation = Quaternion.LookRotation(direction, reference);
        rb.MoveRotation(Quaternion.Slerp(currentRotation, newRotation, turningSpeed));
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

    public void RemoveTarget()
    {
        target = null;
        path = null;
    }

    void MoveForward() 
    {
        rb.AddForce(tr.forward * movementSpeed);
    }

    void TurnLeft()
    {
        rb.AddTorque(-tr.up * turningSpeed);
    }

    void TurnRight()
    {
        rb.AddTorque(tr.up * turningSpeed);
    }

    private void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.tag != "Wall") return;
        if (wall == null) wall = collisionInfo.gameObject;
    }

    void OnCollisionStay(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.tag != "Wall") return;
        if (wall == null) wall = collisionInfo.gameObject;
        
        wallNormal = collisionInfo.contacts[0].normal;
        wallPoint = collisionInfo.contacts[0].point;
    }

    void OnCollisionExit(Collision collisionInfo)
    {
        if (collisionInfo.gameObject == wall) wall = null;
    }
}
