using MLAgents;
using UnityEngine;

public class LidAcademy : Academy
{
    public static LidAcademy Instance { get; private set; }

    public LidAgent agent;
    public HoleLogic hole;
    public LayerMask lidLayer;

    public float range;


    public override void InitializeAcademy()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        range = Vector3.Distance(agent.transform.position, hole.transform.position) * 2f;
        Debug.Log($"Range: {range}");

        hole.MoveToRandomPosition(range);
    }

    public override void AcademyReset() => base.AcademyReset();
    public override void AcademyStep() => base.AcademyStep();
}
