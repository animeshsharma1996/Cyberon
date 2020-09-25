using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HackingSystem : SSystem<HackingSystem>
{

    private const string defaultHackName = "Hack Menu";
    private const string defaultHackDesc = "On the left you can see the reach of this terminal " +
        "and all entities that you can hack. Select an Icon to show the description of " +
        "the hack. To apply hack click Initiate Hack.";
    private const string defaultHackCost = "Cost: ";


    [Header ("Hacking Data")]
    [SerializeField]  private float hackRange = 10.0f;
    public float HackRange { get => hackRange; set => hackRange = value; }
    [Header("Hacking Camera")]
    private new Camera camera = null;

    // Resetable
    private List<IHackable> hackables = new List<IHackable>();
    private Terminal currentTerminal = null;
    private Dictionary<Toggle, IHackable> hackablesInRange = new Dictionary<Toggle, IHackable>();
    private IHackable toBeHacked = null;



    // Start is called before the first frame update
    public void Init(List<IHackable> listOfHackables)
    {
        hackables = listOfHackables;

        // Set-up camera
        camera = GetComponent<Camera>();
        camera.aspect = 1.0f;
        camera.orthographicSize = hackRange + 1.0f;

        // Set Hack UI requirements
        UIManager.Instance.SetInitiateHackBtn(() => this.ExecuteHack());
        UIManager.Instance.SetCloseHackBtn(() => this.ResetAllInfo());
    }


    public void InitiateHacking(Terminal terminal)
    {
        // GameSaveMaster.Instance.SaveGameData();
        ResetAllInfo();

        currentTerminal = terminal;
        Vector3 pos = terminal.gameObject.transform.position;
        camera.transform.position = new Vector3(pos.x, gameObject.transform.position.y, pos.z);

        //camera.targetTexture.ResolveAntiAliasedSurface();
        //camera.targetTexture.Release();
        // For every hackable object in range create a toggle
        foreach (IHackable hackable in hackables)
        {
            if (IsInRange(hackable.GetGameObject()))
            {
                // Create a button for it
                CreateHackButton(hackable);
            }
        }
        ResetDisplayInfo();
    }

    private bool IsInRange(GameObject obj)
    {
        Vector3 vec1 = obj.transform.position;
        Vector3 vec2 = currentTerminal.gameObject.transform.position;
        return Vector3.Distance(vec1, vec2) <= hackRange; 
    }

    private void CreateHackButton(IHackable hackable)
    {
        // Create Button in UI and return it
        Toggle toggle = UIManager.Instance.CreateHackableButton();

        // Add Action Listener
        toggle.onValueChanged.AddListener(delegate { this.SetActiveHack(toggle, hackable); });

        // Set Position on display
        SetHackableButtonPosition(toggle, hackable);

        // Set Data for toggle
        HackDescriptor hackInfo = hackable.GetHackInfo();
        toggle.gameObject.name = hackInfo.aName + ":" + hackable.GetGameObject().name;
        toggle.GetComponentInChildren<Image>().sprite = hackInfo.aSprite;
    }

    public void SetActiveHack(Toggle toggle, IHackable hackable)
    {
        // Get Descriptor
        HackDescriptor hack = hackable.GetHackInfo();
        // Set UI Hack Name
        UIManager.Instance.HackNameTxt.text = hack.aName;
        // Set UI Hack Description
        UIManager.Instance.HackDescTxt.text = hack.aDescription;
        // Set UI Hack Cost
        UIManager.Instance.HackCostTxt.text = "Cost: " + hack.aCost.ToString() + " Energy";

        if (hackable.IsHacked())
        {
            UIManager.Instance.InitiateHackButton.GetComponentInChildren<TMPro.TMP_Text>().text = "Unhack";
        } else UIManager.Instance.InitiateHackButton.GetComponentInChildren<TMPro.TMP_Text>().text = "Hack";

        toBeHacked = hackable;
    }

    private void SetHackableButtonPosition(Toggle toggle, IHackable hackable)
    {
        // obtain direction vector from terminal
        Vector3 direction = hackable.GetGameObject().transform.position - currentTerminal.transform.position;
        // set Y axis to 0
        direction.y = 0.0f;
        // normalize in terms of the terminal view image size
        Image viewImage = UIManager.Instance.GetTerminalViewImage();
        float xSizeUnit = viewImage.rectTransform.sizeDelta.x / ((hackRange + 1.0f) * 2.0f);
        float ySizeUnit = viewImage.rectTransform.sizeDelta.y / ((hackRange + 1.0f) * 2.0f);
        toggle.transform.localPosition = new Vector3(direction.x * xSizeUnit, direction.z * ySizeUnit, -1.0f);

    }

    public void ExecuteHack()
    {
        if (toBeHacked == null)
        {
            UIManager.Instance.DisplayMessageInHS("No hack chosen!", Color.red);
            UIManager.Instance.PlayVoiceSound("errorSound");

            return; 
        }

        if (GameStateManager.Instance.PlayerStats.aEnergy < toBeHacked.GetHackInfo().aCost)
        {
            UIManager.Instance.DisplayMessageInHS("Not enough energy!", Color.red);
            UIManager.Instance.PlayVoiceSound("errorSound");
            return; 

        }

        // Initiate hack effect
        toBeHacked.Hack();
        var hackInfo = toBeHacked.GetHackInfo();


        GameStateManager.Instance.PlayerStats.aEnergy -= hackInfo.aCost;
        UIManager.Instance.SetEnergy();


        // Update UI
        if (toBeHacked.IsHacked())
        {
            UIManager.Instance.InitiateHackButton.GetComponentInChildren<TMPro.TMP_Text>().text = "Unhack";
            UIManager.Instance.DisplayMessageInHS(hackInfo.aTarget + " disabled!", Color.green);
            if (hackInfo.aName == "Hack Doorway")
            {
                UIManager.Instance.PlayVoiceSound("hackingDoorwayVoice");
            }
            else
            {
                UIManager.Instance.PlayVoiceSound("hackingEnemiesVoice");
            }
            return;
        }
        
        // else
        UIManager.Instance.InitiateHackButton.GetComponentInChildren<TMPro.TMP_Text>().text = "Hack";
        UIManager.Instance.DisplayMessageInHS(hackInfo.aTarget + " enabled!", Color.red);

    }

    public void ResetAllInfo()
    {
        currentTerminal = null;
        toBeHacked = null;
        UIManager.Instance.ClearTerminalButtons();
        ResetDisplayInfo();
    }

    public void ResetDisplayInfo()
    {
        toBeHacked = null;
        UIManager.Instance.HackCostTxt.text = defaultHackCost;
        UIManager.Instance.HackDescTxt.text = defaultHackDesc;
        UIManager.Instance.HackNameTxt.text = defaultHackName;
        UIManager.Instance.InitiateHackButton.GetComponentInChildren<TMPro.TMP_Text>().text = "Hack";
    }
}
