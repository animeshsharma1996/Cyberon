using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#region _________ENUMERATORS__________
public enum Allegiance
{
    Player,
    Enemy
}

public enum GamePhase
{
    Stealth,
    Hunt
}
#endregion


public class GameStateManager : SSystem<GameStateManager>
{

    public int CameraSensitivity = 100;
    public bool isInit = false;
    private int maximumScenes = 0;
    public int sceneIndex = -1;
    public bool reloadGame = false;
    public int score = 0;

    private  GamePhase phase = GamePhase.Stealth;

    public List<ResourceSilo> AllResourceSilos { get; private set; }
    public  GamePhase Phase { get => phase; set => phase = value; }

    [SerializeField] public List<Material> circuitMaterials;
    [SerializeField] public Material wallMaterial;
    [ColorUsage(true,true)] public Color wallsHuntColor;
    [ColorUsage(true, true)] public Color wallsStealthColor;

    [SerializeField] public List<Material> wallsBorderMaterials;
    [ColorUsage(true, true)] public Color wallsBorderStealthColor;
    [ColorUsage(true, true)] public Color wallsBorderHuntColor;

    [SerializeField] private PlayerStats playerStats;
    public PlayerStats PlayerStats { get => playerStats; set => playerStats = value; }
    public PlayerController Player { get; private set; }

    public List<CyberonActor> AllActors { get; private set; }
    public List<Hunter> AllHunters { get; private set; }
    public List<Turret> AllTurrets { get; private set; }
    public List<Terminal> AllTerminals { get; private set; }
    public List<Detector> AllDetectors { get; private set; }
    public List<IHackable> AllHackables { get; private set; }
    public List<TurnActor> AllTurnActors { get; private set; }
    public List<Detectable> AllDetectables { get; private set; }
    public List<Patroller> AllPatrollers { get; private set; }
    public List<Doorway> AllDoorways { get; private set; }
    public List<Shooter> AllShooters { get; private set; }
    public ObjectivesQueue objectivesQueue;
    private StackedFSM gameStateFSM = null;

    public bool inMenu = false;
    public bool inCredits = false;

    void Awake()
    {
        isInit = true;
        playerStats.aScore = score = PlayerPrefs.GetInt("Score");
        maximumScenes = SceneManager.sceneCountInBuildSettings;
        Debug.Log("SceneCount:" + maximumScenes);
        DontDestroyOnLoad(gameObject);

        // Create State machine
        gameStateFSM = new StackedFSM();
        UIManager.Instance.Init();
        
    }

    void Update()
    {
        if (UIManager.Instance.IsCoverOpened())
        {
            if (Input.anyKey)
                UIManager.Instance.OpenMainMenu();
        }

        gameStateFSM.OnUpdate();
        if (phase == GamePhase.Stealth)
        {
            UIManager.Instance.StopDynamicMusic();
        }

        
    }


    public void GatherData()
    {
        // Set Level state
        phase = GamePhase.Stealth;

        // Get Items data
        AllResourceSilos = new List<ResourceSilo>(FindObjectsOfType<ResourceSilo>());
        AllTerminals = new List<Terminal>(FindObjectsOfType<Terminal>());
        AllHackables = FindInterfaces.Find<IHackable>();
        AllActors = new List<CyberonActor>(FindObjectsOfType<CyberonActor>());
        AllTurrets = new List<Turret>(FindObjectsOfType<Turret>());
        AllTurnActors = new List<TurnActor>(FindObjectsOfType <TurnActor>());
        AllDetectors = new List<Detector>(FindObjectsOfType<Detector>());
        AllDetectables = new List<Detectable>(FindObjectsOfType<Detectable>());
        AllHunters = new List<Hunter>(FindObjectsOfType<Hunter>());
        AllPatrollers = new List<Patroller>(FindObjectsOfType<Patroller>());
        AllDoorways = new List<Doorway>(FindObjectsOfType<Doorway>());
        AllShooters = new List<Shooter>(FindObjectsOfType<Shooter>());
        objectivesQueue = new ObjectivesQueue();
        Player = FindObjectOfType<PlayerController>(); 

        //// Nulify player stats for now      --------- Get Player Stats from the Game Save Master
        playerStats.aEnergy = 0;
        playerStats.aActionPoints = 0;
        playerStats.aScore = score;

        CyberonCamera.Instance.ChangeToStealth();
        ChangeWallsToStealth();
    }

    public void StartNewGame()
    {
        UIManager.Instance.StopDynamicMusic();
        score = 0;
        sceneIndex = 1;

        // Clear player data

        gameStateFSM.SetState(new LevelLoading(gameStateFSM));
    }

    public void BackToMainMenu()
    {
        UIManager.Instance.StopDynamicMusic();
        gameStateFSM.Clear();
        sceneIndex = 0;
        SceneManager.LoadScene(0);
    }

    public void SaveGame()
    {
        SaveSystem.Instance.SaveGame(true);
    }

    public void ReloadGame()
    {
        UIManager.Instance.StopDynamicMusic();
        if (!PlayerPrefs.HasKey("SceneIndex"))
        {
            Debug.LogError("No scene to Reload");
            return;
        }

        sceneIndex = PlayerPrefs.GetInt("SceneIndex");

        reloadGame = true;

        gameStateFSM.SetState(new LevelLoading(gameStateFSM));
    }

    public void RestartLevel()
    {
        UIManager.Instance.StopDynamicMusic();
        sceneIndex = SceneManager.GetActiveScene().buildIndex;

        gameStateFSM.SetState(new LevelLoading(gameStateFSM));
    }

    public void LoadNextLevel()
    {
        UIManager.Instance.StopDynamicMusic();
        sceneIndex++;
        sceneIndex = sceneIndex % maximumScenes;

        if (sceneIndex == 0)
        {
            SceneManager.sceneLoaded += OnLoadingCredits;
            SceneManager.LoadScene(0);

            // Goes to main menu
            return;
        }
        gameStateFSM.SetState(new LevelLoading(gameStateFSM));
    }

    public void OnLoadingCredits(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnLoadingCredits;

        UIManager.Instance.OpenCredits();
    }

    public void ChangePhase()
    {
        if (Phase == GamePhase.Hunt) return;

        StartCoroutine(StartDelayedDynamicMusic());
        UIManager.Instance.PlayVoiceSound("detectionVoice");
        phase = GamePhase.Hunt;
        GridControl.Instance.ChangeToHuntPalette();
        CyberonCamera.Instance.ChangeToHunt();
        ChangeWallsToHunt();
        foreach (var detector in AllDetectors)
        {
            if (detector.IsActive)
            {
                detector.ForceRedrawDetectionLayer();
            }
        }

        foreach (var hunter in AllHunters) 
        {
            hunter.StartHunt(Player.gameObject);
        }
        // Tell each hunter that they can hunt
    }

    private IEnumerator StartDelayedDynamicMusic()
    {
        yield return new WaitForSeconds(5.0f);
        if (Phase == GamePhase.Hunt)
            UIManager.Instance.StartDynamicMusic();
    }

    public void EndGame()
    {
        Application.Quit();
    }

    private void ChangeWallsToHunt()
    {
        foreach (var cMat in circuitMaterials)
        {
            cMat.SetColor("_EmissionColor", wallsHuntColor);
        }
        wallMaterial.color = wallsHuntColor;
        foreach (var wallsBorderMaterial in wallsBorderMaterials)
        {
            wallsBorderMaterial.color = wallsBorderHuntColor;
        }
    }

    private void ChangeWallsToStealth()
    {
        foreach (var cMat in circuitMaterials)
        {
            cMat.SetColor("_EmissionColor", wallsStealthColor);

        }

        wallMaterial.color = wallsStealthColor;
        foreach (var wallsBorderMaterial in wallsBorderMaterials)
        {
            wallsBorderMaterial.color = wallsBorderStealthColor;
        }
    }

    public void SetScore(int value)
    {
        if (value < 0)
        { score = 0; }

        score = value;
        UIManager.Instance.SetScoreText(score);
    }

    private IEnumerator ShowLoseScreen()
    {
        yield return new WaitForSeconds(3.0f);

        UIManager.Instance.InGameBlurMask.SetActive(true);
        UIManager.Instance.LoseScreen.SetActive(true);
        Cursor.visible = true;
    }

    private IEnumerator ShowWinScreen()
    {
        yield return new WaitForSeconds(1.0f);

        UIManager.Instance.InGameBlurMask.SetActive(true);
        UIManager.Instance.WinScreen.SetActive(true);
        Cursor.visible = true;
    }

    public void ActivateStateLose()
    {
        StartCoroutine(ShowLoseScreen());
    }

    public void ActivateWinState()
    {
        StartCoroutine(ShowWinScreen());
    }

    public void GetLevelInfo()
    {
        int drones = FindObjectsOfType<DroneController>().Length;
        int hunters = FindObjectsOfType<HunterController>().Length; 
        int turrets = FindObjectsOfType<Turret>().Length; 

        UIManager.Instance.SetInfoScene(drones,hunters,turrets);
    }

}

