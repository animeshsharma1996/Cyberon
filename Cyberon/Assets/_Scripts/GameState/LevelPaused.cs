using UnityEngine;

public class LevelPaused : IState
{
    StackedFSM owner;

    public LevelPaused(StackedFSM owner){
        this.owner = owner;
    }

    public void OnStateEnter()
    {
        UIManager.Instance.InGameBlurMask.SetActive(true);
        Time.timeScale = 0.0f;
        Cursor.visible = true;

    }

    public void OnStateExit()
    {
        UIManager.Instance.InGameBlurMask.SetActive(false);
        Time.timeScale = 1.0f;

        if(!CyberonCamera.Instance.IsCameraFreeRoaming)
            Cursor.visible = false;
    }

    public void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Close all menues opened
            UIManager.Instance.CloseAllOpenMenus();
        }

        if (!UIManager.Instance.IsMenuOpened())
                owner.ExitState();
  
    }
}