using UnityEngine;

public class ReachObjective : Objective
{
    [SerializeField]
    private float distance = 1.0f;

    public override int AddScore(int value)
    {
        return (value+500);
    }

    public override bool isComplete()
    {
        return Vector3.Distance(GameStateManager.Instance.Player.transform.position, gameObject.transform.position) <= distance;
    }
}
