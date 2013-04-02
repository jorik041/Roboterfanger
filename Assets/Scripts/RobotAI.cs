using UnityEngine;

[RequireComponent(typeof(Seeker))]
public class RobotAI : AIPath
{
    public float sleepVelocity = 0.1F;

    public new void Start()
    {
        base.Start();
    }

    public override Vector3 GetFeetPosition()
    {
        return tr.position;
    }

    protected new void Update()
    {
        if (canMove)
        {
            Vector3 dir = CalculateVelocity(GetFeetPosition());

            if (targetDirection != Vector3.zero)
            {
                RotateTowards(targetDirection);
            }

            if (navController != null)
                navController.SimpleMove(GetFeetPosition(), dir);
            else if (controller != null)
                controller.SimpleMove(dir);
            else
                Debug.LogWarning("No NavmeshController or CharacterController attached to GameObject");
        }
    }
}
