using UnityEngine;

public class Robot : MonoBehaviour
{
    public float acceleration;
    public float turnSpeed;
    public float gravity;
    public Renderer skin;
    
    private Transform robot;
    private Rigidbody body;
    private Material skinMaterial;
    private bool mode = true;
    private GameObject wall;
    private Vector3 wallNormal;
    private Vector3 wallPoint;

    void Start()
    {
        robot = transform;
        body = rigidbody;
        body.centerOfMass = new Vector3(0, -0.05f, 0);
        skinMaterial = skin.material;
        wallNormal = robot.up;
        wallPoint = robot.position;

        var start = Random.value;

        Invoke("StartAudio", 0.5f + start);
        InvokeRepeating("MoveForward", 0.5f + start, 0.1f);
        InvokeRepeating("AI", 0.5f + start, 0.05f);
        InvokeRepeating("SwitchMode", 2 + start * 2, 2 + start * 2);
    }

    void FixedUpdate()
    {
        Debug.DrawRay(robot.position, -wallNormal, Color.red);
        Debug.DrawRay(robot.position, -robot.up, Color.blue);

        if (robot.up != wallNormal)
        {
            var forward = robot.forward;
            Vector3.OrthoNormalize(ref wallNormal, ref forward);
            body.rotation = Quaternion.LookRotation(forward, wallNormal);
        }
        body.AddForce(-wallNormal * gravity);
    }

    void StartAudio()
    {
        audio.Play();
    }

    void SwitchMode()
    {
        if (mode)
        {
            skinMaterial.color = new Color(1, 0.9f, 0);
            mode = !mode;
        }
        else
        {
            skinMaterial.color = new Color(0, 0.2f, 1);
            mode = !mode;
        }
    }

    void AI()
    {
        if (mode)
        {
            MoveLeft();
        }
        else
        {
            MoveRight();
        }
        
    }

    void MoveForward()
    {
        body.AddForce(robot.forward * acceleration);
    }

    void MoveLeft()
    {
        body.AddTorque(-robot.up * turnSpeed);
    }

    void MoveRight()
    {
        body.AddTorque(robot.up * turnSpeed);
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.tag != "Wall") return;

        //if(collisionInfo.gameObject =)
    }

    void OnCollisionStay(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.tag != "Wall") return;

        var angle = Vector3.Angle(robot.up, wallNormal);
        var newNormal = collisionInfo.contacts[0].normal;
        var newPoint = collisionInfo.contacts[0].point;
        
        foreach (var contact in collisionInfo.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.white);
            if (Vector3.Angle(robot.up, contact.normal) < angle)
            {
                newNormal = contact.normal;
                newPoint = contact.point;
            }
        }

        Debug.DrawRay(newPoint, newNormal, Color.black);
        wall = collisionInfo.gameObject;
        wallNormal = newNormal;
        wallPoint = newPoint;
    }
}
