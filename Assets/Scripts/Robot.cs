using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Robot : MonoBehaviour
{
    public float acceleration = 0.3f;
    public float turnSpeed = 0.01f;
    public float gravity = 3;
    public Renderer skin;
    
    private Transform tr;
    private Rigidbody rb;
    private Material skinMaterial;
    private bool colorMode = true;
    private GameObject wall;
    private Vector3 wallNormal;
    private Vector3 wallPoint;
    private Vector3 destination;
    private Vector3 destinationVector;
    private bool goal;

    void Awake()
    {
        tr = transform;
        rb = rigidbody;
        rb.centerOfMass = new Vector3(0, -0.05f, 0);
        skinMaterial = skin.material;
        wallNormal = tr.up;
        wallPoint = tr.position;
    }

    void Start()
    {
        var start = Random.value;
        //audio.pitch = 0.5f + start;
        Invoke("StartAudio", start);
        InvokeRepeating("AI", start, 0.1f);
        InvokeRepeating("SwitchColorMode", 2 + start*2, 2 + start*2);
    }

    void Update()
    {
        Debug.DrawRay(tr.position, -wallNormal, Color.red);
        Debug.DrawRay(tr.position, -tr.up, Color.blue);
        if(goal) Debug.DrawRay(tr.position, destinationVector, Color.cyan);
    }

    void FixedUpdate()
    {
        if (tr.up != wallNormal)
        {
            var forward = tr.forward;
            Vector3.OrthoNormalize(ref wallNormal, ref forward);
            //rb.MoveRotation(Quaternion.LookRotation(forward, wallNormal));
            tr.rotation = Quaternion.LookRotation(forward, wallNormal);
        }
        if (wall != null) rb.AddForce(-wallNormal*gravity);
        else rb.AddForce(-tr.up*gravity);
    }

    void AI()
    {
        if (goal)
        {
            destinationVector = destination - tr.position;
            var up = tr.up;
            Vector3.OrthoNormalize(ref up, ref destinationVector);
            
            if (destinationVector != tr.forward)
            {
                tr.rotation = Quaternion.LookRotation(destinationVector, tr.up);
            }
            MoveForward();
        }
        else
        {
            if (colorMode) MoveLeft();
            else MoveRight();
            MoveForward();
        }
        
    }

    public void SetDestination(Vector3 point)
    {
        destination = point;

        if (!goal)
        {
            CancelInvoke("SwitchColorMode");
            skinMaterial.color = new Color(1, 0.1f, 0);
            goal = true;
        }
    }

    public void RemoveDestination()
    {
        goal = false;
        SwitchColorMode();
        var start = Random.value;
        InvokeRepeating("SwitchColorMode", 2 + start * 2, 2 + start * 2);
    }

    void StartAudio()
    {
        audio.Play();
    }

    void SwitchColorMode()
    {
        if (goal)
        {
            CancelInvoke("SwitchColorMode");
            return;
        }
            
        if (colorMode)
        {
            skinMaterial.color = new Color(1, 0.8f, 0);
            colorMode = !colorMode;
        }
        else
        {
            skinMaterial.color = new Color(0, 0.2f, 1);
            colorMode = !colorMode;
        }
    }

    void MoveForward() 
    {
        rb.AddForce(tr.forward * acceleration);
    }

    void MoveLeft()
    {
        rb.AddTorque(-tr.up * turnSpeed);
    }

    void MoveRight()
    {
        rb.AddTorque(tr.up * turnSpeed);
    }

    private void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.tag != "Wall") return;
        if (wall == null) wall = collisionInfo.gameObject;

        //var angle = Vector3.Angle(robot.up, wallNormal);
        //var newNormal = collisionInfo.contacts[0].normal;
        //var newPoint = collisionInfo.contacts[0].point;
        //if (Vector3.Angle(robot.up, contact.normal) < angle)
    }

    void OnCollisionStay(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.tag != "Wall") return;
        if (wall == null) wall = collisionInfo.gameObject;
        
        foreach (var contact in collisionInfo.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.white);
        }
        
        wallNormal = collisionInfo.contacts[0].normal;
        wallPoint = collisionInfo.contacts[0].point;
        Debug.DrawRay(wallPoint, wallNormal, Color.black);
    }

    void OnCollisionExit(Collision collisionInfo)
    {
        if (collisionInfo.gameObject == wall) wall = null;
    }
}
