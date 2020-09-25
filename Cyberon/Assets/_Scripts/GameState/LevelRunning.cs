using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelRunning : IState
{
    StackedFSM owner;
    public LevelRunning(StackedFSM owner) {
        this.owner = owner;
    }

    PlayerController player = null;
    WinSpot winSpot = null;

    public void OnStateEnter()
    {
        
        SaveSystem.Instance.SaveGame(true);
        var lgt = GameObject.FindObjectOfType<LevelGridType>();
        
        if (lgt == null)
        {
            // That is not a playable level so no need to runn things
            return;
        }

        player = GameObject.FindObjectOfType<PlayerController>();
        winSpot = GameObject.FindObjectOfType<WinSpot>();

        if (TurnManager.Instance.actorIndex != -1)
        {
            TurnManager.Instance.ContinueTurn();
            return;
        }
        TurnManager.Instance.NextTurn();
    }


    public void OnStateExit()
    {

    }

    public void OnUpdate()
    {
        var lgt = GameObject.FindObjectOfType<LevelGridType>();
        if (lgt == null)
        {
            // That is not a playable level so no need to runn things
            return;
        }

        // Early Update
        if (player.isDead)
        {
            owner.SetState(new LevelLost(owner));
            return;
        }

        if (GameStateManager.Instance.objectivesQueue.OnUpdate())
        {
            owner.SetState(new LevelWon(owner));
            return;
        }

        // Update
        TurnManager.Instance.OnUpdate();

        foreach (var actor in GameStateManager.Instance.AllActors)
        {
            actor.OnUpdate();
        }


        // Late Update
        TurnManager.Instance.OnLateUpdate();

        HandleInput();
    }

    private void HandleInput()
    {
        if (UIManager.Instance.IsMenuOpened())
            owner.AddState(new LevelPaused(owner));

        if (Input.GetKeyDown(KeyCode.S))
        {
            GameStateManager.Instance.SaveGame();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            GameStateManager.Instance.ReloadGame();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.Instance.InGameMenuToggle();
        }
    }
}
