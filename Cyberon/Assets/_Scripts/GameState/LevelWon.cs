using UnityEngine;
using UnityEngine.SceneManagement;

class LevelWon : IState
{
    StackedFSM owner;
    public LevelWon(StackedFSM owner)
    {
        this.owner = owner;
    }

    public void OnStateEnter()
    {
        //Score = 7*scene Index;
        int score = GameStateManager.Instance.score;
        score += SceneManager.GetActiveScene().buildIndex * 70;
        GameStateManager.Instance.SetScore(score);
        GameStateManager.Instance.ActivateWinState();
    }

    public void OnUpdate()
    {

    }

    public void OnStateExit()
    {
        UIManager.Instance.InGameBlurMask.SetActive(false);
    }
}

