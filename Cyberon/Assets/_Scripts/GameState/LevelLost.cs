using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class LevelLost : IState
{
    StackedFSM owner;

    public LevelLost(StackedFSM owner)
    {
        this.owner = owner;
    }

    public void OnStateEnter()
    {
        int score = GameStateManager.Instance.score;
        score -= 10;
        GameStateManager.Instance.SetScore(score);

        GameStateManager.Instance.ActivateStateLose();
    }

    public void OnUpdate()
    {

    }



    public void OnStateExit()
    {
        UIManager.Instance.InGameBlurMask.SetActive(false);

    }
}
