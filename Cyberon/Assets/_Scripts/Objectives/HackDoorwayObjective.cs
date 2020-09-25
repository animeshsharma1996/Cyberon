using UnityEngine;

public class HackDoorwayObjective : Objective
{
    [SerializeField] private Doorway target = null;

    public override int AddScore(int value)
    {
        return (value + 800);
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
