using UnityEngine;

public abstract class TurnActor : CyberonActor
{
    protected bool isInAction = false;
    public bool IsInAction { get => isInAction; set => isInAction = value; }

    protected bool isActorsTurn = false;
    public bool IsActorsTurn { get => isActorsTurn; set => isActorsTurn = value; }

    [SerializeField, Range(1, 10)]
    protected int maxActionPoints = 5;

    protected int actionPoints;
    public int ActionPoints { get => actionPoints; set => actionPoints = value; }

    public void StartTurn()
    {
        isActorsTurn = true;
        isInAction = false;
        actionPoints = maxActionPoints;
        CyberonCamera.Instance.SetTarget(gameObject);
        CyberonCamera.Instance.Follow();
    }

    public void EndTurn()
    {
        isInAction = false;
        isActorsTurn = false;
        GridControl.Instance.OccupyTile(transform.position);

    }

    public virtual void OnTurnStart() { }

    public abstract void OnTurnExecute();

    public virtual void OnTurnEnd() { }

    
}

