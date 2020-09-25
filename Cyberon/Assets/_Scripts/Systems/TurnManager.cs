using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TurnManager : SSystem<TurnManager>
{

    public int TurnCount = 0;
    public int actorIndex = -1;
    
    
    // Turns for actors
    [SerializeField] List<TurnActor> ActorsWithTurn = new List<TurnActor>();
    TurnActor currentActor = null;
    int playersCount = 0;
    int enemiesCount = 0;

    public void OnUpdate()
    {
        currentActor.OnTurnExecute();
    }

    // Update is called once per frame
    public void OnLateUpdate()
    {
        if (actorIndex >= 0)
        {
            if (!currentActor.IsActorsTurn)
            {
                currentActor.OnTurnEnd();
                NextTurn();
            }
        }
    }

    public void Init()
    {
        var actors = FindObjectsOfType<TurnActor>();
        Assert.AreNotEqual(actors.Length, 0, " Possible actors: " + actors.Length);

        // Add players to queue
        foreach (var actor in actors)
        {
            if (actor.Allegiance == Allegiance.Player)
            {
                ActorsWithTurn.Add(actor);
                playersCount++;
            }
        }

        // Add enemies to queue
        foreach (var actor in actors)
        {
            if (actor.Allegiance == Allegiance.Enemy)
            {
                ActorsWithTurn.Add(actor);
                enemiesCount++;
            }
        }
        
        if(ActorsWithTurn.Count == 0)
        {
            Debug.LogError("No actors in actor queue");
        }
    }


    public void NextTurn()
    {
        actorIndex = (actorIndex + 1) % ActorsWithTurn.Count;
        currentActor = ActorsWithTurn[actorIndex];
        UIManager.Instance.SetActiveTurnMsg(currentActor.Allegiance);
        currentActor.StartTurn();
        currentActor.OnTurnStart();
    }

    public void ContinueTurn()
    {
        currentActor = ActorsWithTurn[actorIndex];
        UIManager.Instance.SetActiveTurnMsg(currentActor.Allegiance);
        currentActor.IsActorsTurn = true;
        currentActor.IsInAction = false;
        CyberonCamera.Instance.SetTarget(currentActor.gameObject);
        CyberonCamera.Instance.Follow();
        currentActor.OnTurnStart();
    }

    public void SwitchActorTurn(TurnActor actor, Allegiance allegiance)
    {
        // Use to switch an actor's turn, for example when turret gets hacked to be ally!
    }

    public void RemoveActorTurn(TurnActor actor)
    {
        // Use when disabling an actor
    }

    public void AddActorTurn(TurnActor actor)
    {
        // Use when a new actor appears in the scene or enabling an actor
    }

}
