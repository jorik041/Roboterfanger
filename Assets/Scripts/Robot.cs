using UnityEngine;

public class Robot : MonoBehaviour
{
    public int acceleration = 50;
    public float mobility = 0.6f;
    public int gravity = 5;
    public LayerMask wallLayer;
    public Renderer skin;
    
    private Transform robot;
    private Rigidbody body;
    private Material skinMaterial;
    private bool mode = true;
    private Vector3 gravityVector;
    private int grounded = 0;

    void Start()
    {
        robot = transform;
        body = rigidbody;
        skinMaterial = skin.material;

        var randomness = Random.value;

        Invoke("StartAudio", 0.5f + randomness);

        InvokeRepeating("MoveForward", 0.5f + randomness, 0.1f);
        InvokeRepeating("AI", 0.5f + randomness, 0.05f);
        InvokeRepeating("SwitchMode", 2 + randomness * 2, 2 + randomness * 2);
    }

    void FixedUpdate()
    {
        body.MoveRotation(body.rotation * Quaternion.FromToRotation(-robot.up, gravityVector));
        body.AddForce(-robot.up * gravity);
    }

    void FindWall()
    {
        var position = robot.position;
        var sphere = Physics.OverlapSphere(position, 0.25f, wallLayer.value);
        var closestWall = Vector3.zero;
        var distanceToWall = 2f;

        foreach (var wall in sphere)
        {
            var closestPoint = wall.ClosestPointOnBounds(position);
            var distance = Vector3.Distance(position, closestPoint);
            if (distance <= distanceToWall)
            { 
                distanceToWall = distance;
                closestWall = closestPoint;
            }
        }

        gravityVector = Vector3.Normalize(position - closestWall);

        body.MoveRotation(body.rotation * Quaternion.FromToRotation(-robot.up, gravityVector));

        //public Vector3 eulerAngleVelocity = new Vector3(0, 100, 0);
        //var deltaRotation = Quaternion.Euler(eulerAngleVelocity * Time.deltaTime);
        //body.MoveRotation(body.rotation * deltaRotation);

        //Vector3.OrthoNormalize(ref normal, ref forward);
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

    void OnCollisionEnter(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.tag != "Wall") return;
        grounded++;
    }

    void OnCollisionStay(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.tag != "Wall") return;

        if (collisionInfo.contacts.Length > 0)
        {
            gravityVector = -collisionInfo.contacts[0].normal;
        }

        foreach (ContactPoint contact in collisionInfo.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.white);
        }
    }

    void OnCollisionExit(Collision collisionInfo)
    {
        if(collisionInfo.gameObject.tag != "Wall") return;

        if (grounded > 0)
        {
            grounded--;
        }
    }
}
