using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using Random = UnityEngine.Random;
using System.Dynamic;

public class UIManager : SSystem<UIManager>
{
    public enum Menues
    {
        GameMenu = 1,
        SettingsMenu = 2,
        TipsMenu = 4
    }
    [SerializeField] private GameObject ContinueButton = null;

    [SerializeField] private GameObject UISimpleTxtPrefab = null;
    [SerializeField] private TMP_Text activeTurnText = null;

    [Header("Panels")]
    [SerializeField] private GameObject objectivesPanel = null;
    [SerializeField] private Animator objectivesAnimator = null;

    [SerializeField] private GameObject statusBar = null;
    [SerializeField] private Animator statusBarAnimator = null;

    [SerializeField] private GameObject playerActionBar = null;
    [SerializeField] private Animator actionBarAnimator = null;

    [SerializeField] private GameObject hackUI = null;
    [SerializeField] private Animator hackUIAnimator = null;


    [SerializeField] private GameObject MainMenuPanel = null;
    [SerializeField] private GameObject CoverPanel = null;
    [SerializeField] private GameObject credits = null;
    [SerializeField] private GameObject MainMenu = null;

    [SerializeField] public GameObject InGameBlurMask = null;
    [SerializeField] public GameObject LoseScreen = null;
    [SerializeField] public GameObject WinScreen = null;
    [SerializeField] private GameObject InGameMenuPanel = null;
    [SerializeField] private GameObject InGameSettingsPanel = null;
    [SerializeField] private GameObject InGameInfoPanel = null;


    [Header("Save dependent")]
    [SerializeField] private GameObject LoseLoadBtn = null;
    [SerializeField] private GameObject InGameLoadBtn = null;
    [SerializeField] private GameObject StatusBarLoadBtn = null;
    [SerializeField] private GameObject MainMenuContinueBtn = null;

    [Header("Tooltips")]
    [SerializeField] public CursorFollowText cursorFollowText = null;
    [SerializeField] private GameObject UIFloatTextPrefab = null;
    [SerializeField] public GameObject HackFloatTextPrefab = null;

    [Header("End of Level UI")]
    [SerializeField] public Button restartBtn = null;
    [SerializeField] public Button reloadBtn = null;
    [SerializeField] public Button winRestartBtn = null;

    [Header("Upper Action Bar")]
    [SerializeField] private TMP_Text scoreText = null;

    [Header("Player Lower Action Bar")]
    [SerializeField] private TMP_Text energyCount = null;
    [SerializeField] private TMP_Text speedCountMax = null;
    [SerializeField] private TMP_Text speedCount = null;
    private int lastSpeedCount = 0;
    private int lastEnergyCount = 0;
    [SerializeField] private Button hackButton = null;
    [SerializeField] private Button endTurnButton = null;
    [SerializeField] private Button gatherButton = null;


    [Header("Hacking UI Settings")]
    [SerializeField] private Button closeHackButton = null;
    [SerializeField] private Button initiateHackButton = null;
    [SerializeField] private Image terminalView = null;
    [SerializeField] private Toggle hackableTogglePrefab = null;
    [SerializeField] private TMP_Text hackNameTxt = null;
    [SerializeField] private TMP_Text hackDescTxt = null;
    [SerializeField] private TMP_Text hackCostTxt = null;

    [Header("In Game Menu Settings")]
    [SerializeField] private AudioSource InGameMusic = null;
    [SerializeField] private AudioSource dynamicMusic = null;    
    [SerializeField] private AudioSource uiSound = null;
    [SerializeField] private Button quickBarSoundBtn = null;
    [SerializeField] private Sprite soundOnIcon = null;
    [SerializeField] private Sprite soundOffIcon = null;
    [SerializeField] private Slider volumeSlider = null; 
    [SerializeField] private Slider sensitivitySlider = null;

    [Header("Voice Sounds")]
    [SerializeField] private AudioSource voiceAudioSource = null;
    [SerializeField] private AudioClip[] detectionVoice = null;
    [SerializeField] private AudioClip[] huntPhaseVoice = null;
    [SerializeField] private AudioClip[] hackingVoice = null;
    [SerializeField] private AudioClip[] dieVoice = null;    
    [SerializeField] private AudioClip hackErrorSound = null;

    [Header("Objectives Panel")]
    [SerializeField] private TMP_Text objectivesTitle = null;
    [SerializeField] private TMP_Text objectivesDescription = null;
    [SerializeField] private TMP_Text objectivesHint = null;
    [SerializeField] private TMP_Text droneInfoText = null;    
    [SerializeField] private TMP_Text hunterInfoText = null;
    [SerializeField] private TMP_Text turretInfoText = null;

    public TMP_Text HackCostTxt { get => hackCostTxt; set => hackCostTxt = value; }
    public TMP_Text HackDescTxt { get => hackDescTxt; set => hackDescTxt = value; }
    public TMP_Text HackNameTxt { get => hackNameTxt; set => hackNameTxt = value; }
    public Button InitiateHackButton { get => initiateHackButton; set => initiateHackButton = value; }

    [Header("Misc")]
    public LoadingScreen loadingScreen = null;
    public GameObject SaveGameTextPos = null;


    // Menu open functionality
    private int menuesOpened;

    public void ResetUIVisuals()
    {
        // Set what is seen in main menu;
        activeTurnText.enabled = false;

        //if (objectivesPanel.activeInHierarchy)
        //{
            //objectivesAnimator.SetBool("Display", false);
            objectivesPanel.SetActive(false);
        //}
        //if (statusBar.activeInHierarchy)
        //{
            //statusBarAnimator.SetBool("Display", false);
            statusBar.SetActive(false);
        //}

        //if (playerActionBar.activeInHierarchy)
        //{
            //actionBarAnimator.SetBool("Display", false);
            playerActionBar.SetActive(false);
        //}

        //if (hackUI.activeInHierarchy)
        //{
            //hackUIAnimator.SetBool("Display", false);
            hackUI.SetActive(false);
        //}


        MainMenuPanel.SetActive(false);
        credits.SetActive(false);
        MainMenu.SetActive(false);

        InGameBlurMask.SetActive(false);
        LoseScreen.SetActive(false);
        WinScreen.SetActive(false);
        InGameMenuPanel.SetActive(false);
        InGameSettingsPanel.SetActive(false);
        InGameInfoPanel.SetActive(false);
    }


    public void OpenMainMenu()
    {
        // Check for continue button:
        if (!SaveSystem.Instance.HasSavedGame()){
            // Hide continue Button
            ContinueButton.SetActive(false);
        } else
        {
            // Show Continue Button
            ContinueButton.SetActive(true);
        }
        ResetUIVisuals();
        MainMenuPanel.SetActive(true);
        CoverPanel.SetActive(false);
        MainMenu.SetActive(true);
    }

    public bool IsCoverOpened() { return CoverPanel.activeSelf; }

    public void OpenCredits()
    {
        ResetUIVisuals();
        MainMenuPanel.SetActive(true);
        credits.SetActive(true);
    }

    public void Init()
    {
        // Collect the Loading Screen
        InGameBlurMask.SetActive(false);
        // Sound Settings 
        if (!PlayerPrefs.HasKey("VolumeSize"))
        {
            PlayerPrefs.SetFloat("VolumeSize", 1.0f);
            quickBarSoundBtn.GetComponent<Image>().sprite = soundOnIcon;
            AudioListener.volume = volumeSlider.value = 1f;
        }
        else
        {
            float volume = PlayerPrefs.GetFloat("VolumeSize");
            if (volume != 0f) // Allow sound, unmute
            {
                Debug.Log("Sound on");
                quickBarSoundBtn.GetComponent<Image>().sprite = soundOnIcon;
                AudioListener.volume = volumeSlider.value = volume;
            }
            else // Mute sound
            {
                Debug.Log("Sound off");
                quickBarSoundBtn.GetComponent<Image>().sprite = soundOffIcon;
                AudioListener.volume = volumeSlider.value = 0f;
            }
        }
        
        // Senstivity Settings
        if (!PlayerPrefs.HasKey("CameraSensitivity"))
        {
            Debug.Log("No Speed Size key in Prefs");
            int speed = 300;
            PlayerPrefs.SetInt("CameraSensitivity", speed);
            sensitivitySlider.value = speed;
            GameStateManager.Instance.CameraSensitivity = speed;
        } else
        {
            int speed = PlayerPrefs.GetInt("CameraSensitivity");
            sensitivitySlider.value = speed;
            GameStateManager.Instance.CameraSensitivity = speed;
        }


        // Level Settings
        if (!PlayerPrefs.HasKey("SceneIndex"))
        {
            LoseLoadBtn.SetActive(false);
            InGameLoadBtn.SetActive(false);
            StatusBarLoadBtn.SetActive(false);
            MainMenuContinueBtn.SetActive(false);
        } else
        {
            LoseLoadBtn.SetActive(true);
            InGameLoadBtn.SetActive(true);
            StatusBarLoadBtn.SetActive(true);
            MainMenuContinueBtn.SetActive(true);
        }

        DontDestroyOnLoad(this);
    }

    public void StartNewGame()
    {
        GameStateManager.Instance.StartNewGame();
    }
    public void ActivateLoads()
    {
        LoseLoadBtn.SetActive(true);
        InGameLoadBtn.SetActive(true);
        StatusBarLoadBtn.SetActive(true);
        MainMenuContinueBtn.SetActive(true);
    }

    public void Quit()
    {
        GameStateManager.Instance.EndGame();
    }

    public bool IsMenuOpened()
    {
        if (menuesOpened == 0) return false;
        return true;
    }

    public void InGameMenuToggle()
    {
        if (InGameMenuPanel.gameObject.activeSelf)
        {
            // toggle off
            menuesOpened = menuesOpened & (~(int)Menues.GameMenu);
            InGameMenuPanel.SetActive(false);
            return;
        }

        // toggle on
        menuesOpened = menuesOpened | (int)Menues.GameMenu;
        InGameMenuPanel.SetActive(true);

    }

    public void CloseAllOpenMenus()
    {
        InGameMenuPanel.gameObject.SetActive(false);
        InGameInfoPanel.gameObject.SetActive(false);
        InGameSettingsPanel.gameObject.SetActive(false);
        menuesOpened = 0;
    }

    public void SettingsMenuToggle()
    {

        if (InGameSettingsPanel.gameObject.activeSelf)
        {
            // toggle off
            menuesOpened = menuesOpened & (~(int)Menues.SettingsMenu);
            InGameSettingsPanel.SetActive(false);
            return;
        }

        // toggle on
        menuesOpened = menuesOpened | (int)Menues.SettingsMenu;
        InGameSettingsPanel.SetActive(true);

    }

    public void InfoMenuToggle()
    {

        if (InGameInfoPanel.gameObject.activeSelf)
        {
            // toggle off
            menuesOpened = menuesOpened & (~(int)Menues.TipsMenu);
            InGameInfoPanel.SetActive(false);
            return;
        }

        // toggle on
        menuesOpened = menuesOpened | (int)Menues.TipsMenu;
        InGameInfoPanel.SetActive(true);
    }

    

    public void SetActiveTurnMsg(Allegiance allegiance)
    {
        if (allegiance == Allegiance.Player)
        {
            activeTurnText.enabled = false;
            return;
        }
        activeTurnText.text = "Enemy's turn!";
        activeTurnText.enabled = true;
    }
   

    public void BackToMainMenu()
    {
        GameStateManager.Instance.BackToMainMenu();
        ResetUIVisuals();
        OpenMainMenu();
    }

    #region Level
    public void RestartLevel()
    {
        GameStateManager.Instance.RestartLevel();
    }

    public void ReloadGame()
    {
        GameStateManager.Instance.ReloadGame();
    }

    public void NextLevel()
    {
        GameStateManager.Instance.LoadNextLevel();
    }

    public void ContinueToNextLevel()
    {
        GameStateManager.Instance.LoadNextLevel();
    }

    public void SaveGame()
    {
        GameStateManager.Instance.SaveGame();
    }
    #endregion

    #region Sounds
    public void ToggleSound()
    {
        float listerVolume = AudioListener.volume;
        if (listerVolume == 0)
        {
            Debug.Log("Sound on");
            // If muted -> unmute
            PlayerPrefs.SetFloat("VolumeSize", 1f);
            AudioListener.volume = volumeSlider.value = 1f;
            quickBarSoundBtn.GetComponent<Image>().sprite = soundOnIcon;
            return;
        }
        
        Debug.Log("Sound off");
        // else (unmute -> mute)
        PlayerPrefs.SetFloat("VolumeSize", 0f);
        AudioListener.volume = volumeSlider.value = 0f;
        quickBarSoundBtn.GetComponent<Image>().sprite = soundOffIcon;
    }

    public void StartDynamicMusic()
    {
        dynamicMusic.Play();
    }

    public void StopDynamicMusic()
    {
        dynamicMusic.Stop();
    }

    public void VolumeSliderChange()
    {
        float volume = volumeSlider.value;
        AudioListener.volume = volumeSlider.value;
        PlayerPrefs.SetFloat("VolumeSize", volume);

        if (volume == 0)
            quickBarSoundBtn.GetComponent<Image>().sprite = soundOffIcon;
        else
            quickBarSoundBtn.GetComponent<Image>().sprite = soundOnIcon;
    }

    public void PlayVoiceSound(String name)
    {
        AudioClip audioClip = null;
        int random = Random.Range(0, 3);
        switch (name)
        {
            case "dieVoice": audioClip = dieVoice[random];
                break;
            case "detectionVoice" : audioClip = detectionVoice[random];
                break;
            case "hackingDoorwayVoice" : audioClip = hackingVoice[0];
                break;
            case "hackingEnemiesVoice" : audioClip = hackingVoice[1];
                break;
            case "huntingVoice": audioClip = huntPhaseVoice[random];
                break;
            case "errorSound": audioClip = hackErrorSound;
                break;
        }
        voiceAudioSource.clip = audioClip;
        voiceAudioSource.Play();
    }

    public void PlayUIBtnSound()
    {
        uiSound.Play();
    }
    #endregion

    public void SenstivitySliderChange()
    {
        int speed = (int) sensitivitySlider.value;
        GameStateManager.Instance.CameraSensitivity = speed;

        PlayerPrefs.SetInt("CameraSensitivity", speed);
    }

    public void SetScoreText(int value)
    {
        scoreText.text = value.ToString();
    }


    #region UI Panels
    public void EnablePlayerUI()
    {

        objectivesPanel.SetActive(true);
        objectivesAnimator.SetBool("Display", true);

        statusBar.SetActive(true);
        statusBarAnimator.SetBool("Display", true);

        actionBarAnimator.SetBool("Display", true);
        playerActionBar.SetActive(true);
    }

    public void DisablePlayerUI()
    {
        // objectivesPanel.SetActive(false)
        objectivesAnimator.SetBool("Display", false);

        //statusBar.SetActive(false);
        statusBarAnimator.SetBool("Display", false);

        actionBarAnimator.SetBool("Display", false);
        //playerActionBar.SetActive(false);
    }

    public void ObjectivesReappear()
    {
        objectivesPanel.SetActive(false);
        objectivesPanel.SetActive(true);
    }

    public void EnableHackUI()
    {
        hackUI.SetActive(true);
        hackUIAnimator.SetBool("Display", true);
    }

    public void DisableHackUI()
    {
        hackUIAnimator.SetBool("Display", false);
        StartCoroutine(SetHackUIActiveFalse());
    }

    private IEnumerator SetHackUIActiveFalse()
    {
        yield return new WaitForSeconds(0.8f);
        hackUI.SetActive(false);
    }
    #endregion

    #region Resources

    public void SetEnergy()
    {
        int energy = GameStateManager.Instance.PlayerStats.aEnergy;
        energyCount.text = energy.ToString();
        if (energy - lastEnergyCount == 0) return;

        if (actionBarAnimator.GetBool("Display") == false) return;
        GameObject floatText = Instantiate(UIFloatTextPrefab, energyCount.transform.position, Quaternion.identity, this.transform);
        if (energy - lastEnergyCount > 0)
            floatText.GetComponentInChildren<TMP_Text>().text = "+" + (energy - lastEnergyCount).ToString();
        else  
            floatText.GetComponentInChildren<TMP_Text>().text = (energy - lastEnergyCount).ToString();
        lastEnergyCount = energy;
    }

    public void SetMaxSpeed(int maxspeed)
    {
        speedCountMax.text = maxspeed.ToString();
    }

    public void SetSpeed(int speed)
    {
        speedCount.text = speed.ToString();
        if (speed - lastSpeedCount <= 0) return;
        if (actionBarAnimator.GetBool("Display") == false) return;
        GameObject floatText = Instantiate(UIFloatTextPrefab, speedCount.transform.position, Quaternion.identity, this.transform);
            floatText.GetComponentInChildren<TMP_Text>().text = "+" + (speed - lastSpeedCount).ToString();
        lastSpeedCount = speed;
    }
    #endregion

    #region Player Action Sets

    public void EnableHackButton()
    {
        hackButton.gameObject.SetActive(true);
    }

    public void DisableHackButton()
    {
        hackButton.gameObject.SetActive(false);

    }

    public void EnableGatherButton()
    {
        gatherButton.gameObject.SetActive(true);
    }

    public void DisableGatherButton()
    {
        gatherButton.gameObject.SetActive(false);
    }

    public void SetHackBtn(UnityAction call)
    {
        hackButton.onClick.AddListener(call);
    }

    public void SetEndTurnBtn(UnityAction call)
    {
        endTurnButton.onClick.AddListener(call);
    }
    public void SetGatherBtn(UnityAction call)
    {
        gatherButton.onClick.AddListener(call);
    }

    public void SetCloseHackBtn(UnityAction call)
    {
        closeHackButton.onClick.AddListener(call);
    }

    public void SetInitiateHackBtn(UnityAction call)
    {
        initiateHackButton.onClick.AddListener(call);
    }
    #endregion

    #region Hacking System
    public Toggle CreateHackableButton()
    {
        Toggle toggle = Instantiate(hackableTogglePrefab, terminalView.rectTransform);
        toggle.group = terminalView.GetComponent<ToggleGroup>();

        return toggle;
    }

    public Image GetTerminalViewImage()
    {
        return terminalView;
    }

    public void ClearTerminalButtons()
    {
        var toggles = terminalView.gameObject.GetComponentsInChildren<Toggle>();
        foreach (var child in toggles)
        {
            Destroy(child.gameObject);
        }
    }

    public void DisplayMessageInHS(string msg, Color color)
    {
        Vector3 pos = HackCostTxt.transform.position;
        GameObject floatText = Instantiate(HackFloatTextPrefab, pos , Quaternion.identity, this.transform);
        var tmptext = floatText.GetComponentInChildren<TMP_Text>();
        tmptext.text = msg;
        tmptext.color = color;
    }

    #endregion


    #region Objectives panel

    public void SetObjectivesTitle(string txt)
    {
        objectivesTitle.text = txt;
    }

    public void SetObjectivesDescription(string txt)
    {
        objectivesDescription.text = txt;
    }

    public void SetObjectivesHint(string txt)
    {
        objectivesHint.text = txt;
    }

    #endregion


    public void DisplaySaveTxt()
    {

        GameObject floatText = Instantiate(UISimpleTxtPrefab, SaveGameTextPos.transform);
        floatText.transform.localPosition = new Vector3 (0.0f, 0.0f, 10.0f);
        floatText.GetComponentInChildren<TMP_Text>().text = "Saving Game";
        StartCoroutine(DisplaySaveTxtDestroy(floatText));
        
    }

    IEnumerator DisplaySaveTxtDestroy(GameObject savetxt)
    {
        yield return new WaitForSecondsRealtime(1.0f);
        Destroy(savetxt);

        yield break;
    }

    public void FixButtons()
    {
        hackButton.gameObject.SetActive(false);
        gatherButton.gameObject.SetActive(false);
        scoreText.text = GameStateManager.Instance.score.ToString();
    }

    public void SetInfoScene(int drones, int hunters,int turrets)
    {
        droneInfoText.text  =  drones == 1 ? drones + " Drone" : drones + " Drones"; 
        hunterInfoText.text =  hunters == 1 ? hunters + " Hunter" : hunters + " Hunters";
        turretInfoText.text = turrets == 1 ? turrets + " Turret" : turrets + " Turrets";
    }
}
   
