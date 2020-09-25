using UnityEngine;

public class HackDroneObjective : Objective
{
    [SerializeField] private DroneController target = null;

    public override int AddScore(int value)
    {
        return (value - 100);
    }

    void Awake()
    {
        if (target == null) Debug.LogError("No target for: " + gameObject.name.ToString());
    }

    public override bool isComplete()
    {
        return target.IsHacked();
    }
}
