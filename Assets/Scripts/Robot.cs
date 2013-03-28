using UnityEngine;

public class Robot : MonoBehaviour
{
    public float acceleration;
    public float turnSpeed;
    public float gravity;
    public LayerMask wallLayer;
    public Renderer skin;
    
    private Transform robot;
    private Rigidbody body;
    private Material skinMaterial;
    private bool mode = true;
    private Vector3 wallNormal;

    void Start()
    {
        robot = transform;
        body = rigidbody;
        body.centerOfMass = new Vector3(0, -0.05f, 0);
        skinMaterial = skin.material;
        wallNormal = robot.up;

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

    void OnCollisionStay(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.tag != "Wall") return;

        if (collisionInfo.contacts.Length > 0)
        {
            foreach (var contact in collisionInfo.contacts)
            {
                Debug.DrawRay(contact.point, contact.normal, Color.white);
            }

            wallNormal = collisionInfo.contacts[0].normal;
            Debug.DrawRay(collisionInfo.contacts[0].point, collisionInfo.contacts[0].normal, Color.black);
        }
    }
}
