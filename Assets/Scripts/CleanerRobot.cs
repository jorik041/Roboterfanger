using UnityEngine;
using Pathfinding;

public class CleanerRobot : MonoBehaviour
{
    public Transform target;
    public float acceleration = 1000;
    public float turnSpeed = 1;
    public float nextWaypointDistance = 2;
    public float targetDistance = 0.5f;
    public Transform taskIndicator;
    public Transform squadIndicator;
    public Color selectColor;
    public Color idleColor;
    public Color followColor;
    public int wallLayerInt = 10;
    public int obstacleLayerInt = 8;
    public int robotLayerInt = 11;
    public bool evade = true;
    [HideInInspector]
    public Path path;

    private Seeker seeker;
    private Transform tr;
    private Rigidbody rb;
    private Vector3 direction;
    private Vector3 away;
    private Vector3 evasiveManeuver;
    private int currentWaypoint;
    private int collisions;
    private Material taskMaterial;
    private Material squadMaterial;

    void Start()
    {
        seeker = GetComponent<Seeker>();
        taskMaterial = taskIndicator.renderer.material;
        squadMaterial = squadIndicator.renderer.material;
        tr = transform;
        rb = rigidbody;
        seeker.pathCallback += OnPathComplete;

        if (target != null)
        {
            SetTarget(target);
        }

        InvokeRepeating("UpdateEvasiveManeuver", Random.value, 0.3f);
    }

    void Update()
    {
        Debug.DrawRay(tr.position, direction, Color.magenta);
        Debug.DrawRay(tr.position, direction + evasiveManeuver, Color.blue);
        Debug.DrawRay(tr.position, evasiveManeuver, Color.cyan);
    }

    void FixedUpdate()
    {
        if (path == null)
        {
            if (evasiveManeuver != Vector3.zero && evade)
            {
                rb.AddForce(evasiveManeuver.normalized*acceleration*Time.deltaTime);
                taskMaterial.color = followColor;
            }
            else
            {
                taskMaterial.color = idleColor;
            }
            return;
        }
        if ((path.vectorPath[path.vectorPath.Count - 1] - tr.position).sqrMagnitude > targetDistance * targetDistance)
        {
            direction = path.vectorPath[currentWaypoint] - tr.position;
            direction.y = 0;
            direction = direction.normalized;
            if (Vector3.Angle(direction, -evasiveManeuver) > 5 && evade)
            {
                rb.AddForce((direction + evasiveManeuver).normalized*acceleration*Time.deltaTime);
            }
            else
            {
                rb.AddForce(direction * acceleration * Time.deltaTime);
            }

            if (Vector3.Distance(tr.position, path.vectorPath[currentWaypoint]) < nextWaypointDistance && currentWaypoint < path.vectorPath.Count-1)
            {
                currentWaypoint++;
            }
        }
        else
        {
            taskMaterial.color = idleColor;
            path = null;
            direction = Vector3.zero;
        }
    }

    void UpdateEvasiveManeuver()
    {
        if (collisions > 0)
        {
            evasiveManeuver = away;
        }
        else
        {
            evasiveManeuver = Vector3.zero;
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
            taskMaterial.color = followColor;
        }
        else
        {
            Debug.Log("Path error");
        }
    }

    void OnDisable()
    {
        seeker.pathCallback -= OnPathComplete;
    }

    public void SetTarget(Vector3 targetPoint)
    {
        seeker.StartPath(tr.position, targetPoint);
    }

    public void SetTarget(Transform targetTransform)
    {
        target = targetTransform;
        SetTarget(target.position);
    }

    public void Select()
    {
        squadMaterial.color = selectColor;
    }

    public void Deselect()
    {
        squadMaterial.color = Color.white;
    }

    void OnCollisionEnter(Collision collision)
    {
        var layer = collision.gameObject.layer;
        if (layer == wallLayerInt || layer == obstacleLayerInt || layer == robotLayerInt)
        {
            collisions++;
        }
    }

    void OnCollisionStay(Collision collision)
    {
        var layer = collision.gameObject.layer;
        if (layer == wallLayerInt || layer == obstacleLayerInt || layer == robotLayerInt)
        {
            away = Vector3.zero;
            for (var i = 0; i < collision.contacts.Length; i++)
            {
                Debug.DrawRay(tr.position, tr.position - collision.contacts[i].point);
                away += tr.position - collision.contacts[i].point;
            }
            away += Random.onUnitSphere;
            away.y = 0;
            away = away.normalized;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        var layer = collision.gameObject.layer;
        if (layer == wallLayerInt || layer == obstacleLayerInt || layer == robotLayerInt)
        {
            collisions--;
        }
    }
}