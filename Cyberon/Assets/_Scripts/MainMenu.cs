using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MainMenu : SSystem<MainMenu>
{

    [SerializeField] private Button continueBtn = null;
    [SerializeField] private Button startNewGameBtn = null;
    [SerializeField] private Button quitButton = null;


    public void Awake()
    {
        AssignContinueBtn(() => GameStateManager.Instance.ReloadGame());
        AssignStartNewGameBtn(() => GameStateManager.Instance.StartNewGame());
        AssignQuitBtn(() => GameStateManager.Instance.EndGame());
    }

    public void AssignContinueBtn(UnityAction call)
    {
         continueBtn.onClick.AddListener(call);
    }

    public void AssignStartNewGameBtn(UnityAction call)
    {
         startNewGameBtn.onClick.AddListener(call);
    }

    public void AssignQuitBtn(UnityAction call)
    {
         quitButton.onClick.AddListener(call);
    }
}
