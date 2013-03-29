using UnityEngine;

public class Robot : MonoBehaviour
{
    public float acceleration = 0.3f;
    public float turnSpeed = 0.01f;
    public float gravity = 3;
    //public float maxVelocity = 0.3f;
    public Renderer skin;
    public LayerMask wallsLayer;
    
    private Transform tr;
    private Rigidbody rb;
    private Material skinMaterial;
    private bool colorMode = true;
    private GameObject wall;
    private Vector3 wallNormal;
    private Vector3 wallPoint;
    //private float sqrMaxVelocity;
    private Vector3 destination;
    private Vector3 destinationNormal;
    private Vector3 destinationVector;
    private bool goal;

    void Awake()
    {
        tr = transform;
        rb = rigidbody;
        rb.centerOfMass = new Vector3(0, -0.05f, 0);
        //sqrMaxVelocity = maxVelocity*maxVelocity;
        skinMaterial = skin.material;
        wallNormal = tr.up;
        wallPoint = tr.position;
    }

    void Start()
    {
        var start = Random.value;
        Invoke("StartAudio", 0.5f + start);
        InvokeRepeating("AI", 0.5f + start, 0.1f);
        InvokeRepeating("SwitchColorMode", 2 + start*2, 2 + start*2);
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

        Debug.DrawRay(tr.position, -wallNormal, Color.red);
        Debug.DrawRay(tr.position, -tr.up, Color.blue);
        //var velocity = rb.velocity;
        //if (velocity.sqrMagnitude > sqrMaxVelocity)
        //{
        //    rb.velocity = Vector3.ClampMagnitude(velocity, maxVelocity);
        //}
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 100, wallsLayer.value);
            destination = hit.point;
            destinationNormal = hit.normal;
            if (!goal)
            {
                CancelInvoke("SwitchColorMode");
                skinMaterial.color = new Color(1, 0.1f, 0);
                goal = true;
            }
        }
    }

    void AI()
    {
        if (goal)
        {
            destinationVector = destination - tr.position;
            var up = tr.up;
            Vector3.OrthoNormalize(ref up, ref destinationVector);

            Debug.DrawRay(tr.position, destinationVector, Color.cyan, 0.1f);
            Debug.DrawRay(destination, destinationNormal, Color.magenta, 0.1f);
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
    }

    void StartAudio()
    {
        audio.Play();
    }

    void SwitchColorMode()
    {
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
        //if (collisionInfo.gameObject != wall) return;
        
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
