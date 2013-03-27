using UnityEngine;

public class Robot : MonoBehaviour
{
    public int acceleration;
    public float mobility;
    public float gravity;
    public LayerMask wallLayer;
    public Renderer skin;
    public Collider sensor;
    
    private Transform robot;
    private Rigidbody body;
    private Material skinMaterial;
    private bool mode = true;
    private Vector3 gravityVector;
    private Quaternion rotation;

    void Start()
    {
        robot = transform;
        body = rigidbody;
        body.centerOfMass = new Vector3(0, -0.05f, 0);
        skinMaterial = skin.material;

        var randomness = Random.value;

        Invoke("StartAudio", 0.5f + randomness);

        InvokeRepeating("MoveForward", 0.5f + randomness, 0.1f);
        //InvokeRepeating("AI", 0.5f + randomness, 0.05f);
        //InvokeRepeating("SwitchMode", 2 + randomness * 2, 2 + randomness * 2);
    }

    void FixedUpdate()
    {
        Debug.DrawRay(robot.position, gravityVector, Color.red);
        Debug.DrawRay(robot.position, -robot.up, Color.blue);
        //body.MoveRotation(Quaternion.FromToRotation(-robot.up, gravityVector));
        //body.MoveRotation(Quaternion.Euler(gravityVector));
        //public Vector3 eulerAngleVelocity = new Vector3(0, 100, 0);
        //var deltaRotation = Quaternion.Euler(eulerAngleVelocity * Time.deltaTime);
        //body.MoveRotation(body.rotation * deltaRotation);
        //Quaternion.Euler(-transform.up)
        
        //var rotation = Quaternion.Slerp(Quaternion.Euler(-robot.up), Quaternion.Euler(gravityVector), 1);
        //if (gravityVector != -robot.up)

        //float step = 0.5f * Time.deltaTime;
        //Vector3 newDir = Vector3.RotateTowards(-robot.up, gravityVector, step, 0.0F);
        //Debug.DrawRay(robot.position, newDir, Color.cyan);

        //rotation = Quaternion.RotateTowards(Quaternion.Euler(-robot.up), Quaternion.Euler(gravityVector), 1);
        //body.MoveRotation(Quaternion.LookRotation(newDir));

        //* Vector3.Angle(gravityVector, -robot.up) 
        body.AddForce(gravityVector * gravity);
        //body.AddForce(-robot.up * gravity);
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
        body.AddTorque(-robot.up * mobility);
    }

    void MoveRight()
    {
        body.AddTorque(robot.up * mobility);
    }

    void OnCollisionStay(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.tag != "Wall") return;

        if (collisionInfo.contacts.Length > 0)
        {
            foreach (ContactPoint contact in collisionInfo.contacts)
            {
                Debug.DrawRay(contact.point, contact.normal, Color.white);
            }
            gravityVector = -collisionInfo.contacts[0].normal;
            Debug.DrawRay(collisionInfo.contacts[0].point, collisionInfo.contacts[0].normal, Color.black);
        }
    }

    void OnCollisionExit(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.tag != "Wall") return;

        if (collisionInfo.contacts.Length > 0)
        {
            //gravityVector = -collisionInfo.contacts[0].normal;
        }

        foreach (ContactPoint contact in collisionInfo.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.black);
        }
    }
}
