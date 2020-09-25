using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoading : /* Object,*/ IState
{

    LoadingScreen loadingScreen = null;
    bool shouldLoadScene = false;
    bool loadingSceneFinished = false;
    StackedFSM owner;
    public LevelLoading(StackedFSM owner)
    {
        this.owner = owner;
    }


    public void OnStateEnter()
    {
        // Add OnSceneLoaded event listener
        shouldLoadScene = true; 
        loadingSceneFinished = false;

        SceneManager.sceneLoaded += OnSceneLoaded;
        loadingScreen = UIManager.Instance.loadingScreen;
        loadingScreen.Reset();
        loadingScreen.StartLoadingScreen();

      
    }


    public void OnUpdate()
    {
        // If animation ready to load level
        if (loadingScreen.finAppearAnim && shouldLoadScene)
        {
            shouldLoadScene = false;
            SceneManager.LoadScene(GameStateManager.Instance.sceneIndex);
        }

        if (loadingSceneFinished)
        {
            loadingSceneFinished = false;
            loadingScreen.EndLoadingScreen();
        }

        // When animation finished
        if (loadingScreen.finDisappearAnim)
        {
            // Unbind sceneListener
            SceneManager.sceneLoaded -= OnSceneLoaded;
            owner.SetState(new LevelRunning(owner));
        }
    }


    public void OnStateExit()
    {
        // Destroy animations stuff

        //throw new System.NotImplementedException();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        var lgt = GameObject.FindObjectOfType<LevelGridType>();
        
        if (lgt == null)
        {
            Debug.Log("OnSceneLoaded normal scene: " + scene.name);
            // That is not a playable level so no need to load things
            return;
        }

        Debug.Log("OnSceneLoaded: " + scene.name);
        UIManager.Instance.ResetUIVisuals();

        //// Start animation
        //// Set the black screen to load up things

        // Gather data
        GameStateManager.Instance.GatherData();

        // Set Camera at position
        Debug.Log("INITIALIZE CAMERA");
        CyberonCamera.Instance.Init();

        // Initialise systems
        GridControl.Instance.InitialiseMaps(GameObject.FindObjectOfType<LevelGridType>());
        TurnManager.Instance.Init();
        HackingSystem.Instance.Init(GameStateManager.Instance.AllHackables);
        PlayerController player = GameStateManager.Instance.Player;

      
        // Set-up the doorway blocks
        foreach (Doorway doorway in GameStateManager.Instance.AllDoorways)
        {
            doorway.Occupy();
        }

       
        // Load if something has to be loaded
        if (GameStateManager.Instance.reloadGame)
        {
            SaveSystem.Instance.LoadGame();
            GameStateManager.Instance.reloadGame = false;
        }
        
        // Fix Action buttons
        UIManager.Instance.FixButtons();

        // Set-up the doorway blocks
        foreach (Doorway doorway in GameStateManager.Instance.AllDoorways)
        {
            if (GridControl.Instance.IsInRange(doorway.transform.position, player.transform.position, 1.0f))
            {
                UIManager.Instance.EnableGatherButton();
                break;
            }
        }

        foreach (Terminal termnal in GameStateManager.Instance.AllTerminals)
        {
            if (GridControl.Instance.IsInRange(termnal.transform.position, player.transform.position, 1.0f))
            {
                UIManager.Instance.EnableHackButton();
                break;
            }
        }

        // Occupy tiles on grid map
        foreach (CyberonActor actor in GameStateManager.Instance.AllActors)
        {
            GridControl.Instance.OccupyTile(actor.gameObject.transform.position);
        }

        // Create Detection layers
        foreach (Detector detector in GameStateManager.Instance.AllDetectors)
        {
            if (detector.IsActive)
            {
                detector.ForceRedrawDetectionLayer();
            }
        }
        // lastly bind UI
        player.BindUI();

        UIManager.Instance.ResetUIVisuals();

        CyberonCamera.Instance.ForceFastMove(player.transform);
        // Start Animation reverting
        loadingSceneFinished = true;

        //Show Info panel upon starting of the scene
        GameStateManager.Instance.GetLevelInfo();
    }

}

