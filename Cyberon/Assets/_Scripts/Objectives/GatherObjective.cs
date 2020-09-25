using UnityEngine;

public class GatherObjective : Objective
{
    [SerializeField] private ResourceSilo target = null;

    void Awake()
    {
        if (target == null) Debug.LogError("No target for: " + gameObject.name.ToString());
    }

    public override int AddScore(int value)
    {
        return (value + 100);
    }

    public override bool isComplete()
    {
        return target.isDepleted;
    }
}
