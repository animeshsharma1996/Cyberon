using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent (typeof (CharacterMovement))]
public class PlayerController : TurnActor
{
    public Vector3 WinPos;

    [SerializeField]
    private PlayerStats stats = null;
    [SerializeField]
    private LayerMask mask = 0;
    private CharacterMovement charMovement = null;
    [SerializeField]
    private Animator playerAnimator = null;


    void Start()
    {
        range = 0.0f;
    }
    void Awake()
    {
        range = 0.0f;
        charMovement = GetComponent<CharacterMovement>();
        Assert.AreNotEqual(charMovement, null, "Player Controller did not find Character Movement");

        playerAnimator = GetComponentInChildren<Animator>();
        Assert.AreNotEqual(playerAnimator, null, "Player Controller did not find Player Animator");
    }

    #region Actor
    override public void OnUpdate()
    {
        if (isDead) return;
        if (playerAnimator.GetBool("IsDead") == true) {

            isDead = true;
            return;
        }
    }
    #endregion

    #region TurnActor
    override public void OnTurnStart()
    {
        GridControl.Instance.EnablePlayMap();
    }

    override public void OnTurnExecute()
    {
        if (isInAction) return;
        UIManager.Instance.EnablePlayerUI();

        if (!CyberonCamera.Instance.IsCameraFocused) return;

        if (!CyberonCamera.Instance.IsCameraFreeRoaming)
        {
            CyberonCamera.Instance.FreeRoam();
            GridControl.Instance.OccupyTile(transform.position);
            GridControl.Instance.RefreshPlayMap();
            GridControl.Instance.ClearPath();
            GridControl.Instance.DisplayRange(transform.position, actionPoints, maxActionPoints);
            UIManager.Instance.SetSpeed(actionPoints);

        }


        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out hit, mask))
            return;

        GridControl.Instance.DrawPath(transform.position, hit.point, actionPoints, maxActionPoints);
        HandleControlInput();
    }

    override public void OnTurnEnd()
    {
        GridControl.Instance.DisablePlayMap();
        GridControl.Instance.RefreshPlayMap();
        GridControl.Instance.ClearPath();
        UIManager.Instance.DisablePlayerUI();
    }
    #endregion


    private void HandleControlInput()
    {
        // Player ends turn with Tab (hardcoded for now)
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            EndTurn();
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            // Note: EventSystem.current.IsPointerOverGameObjects returns true if it is not over game object!! :D
            if (EventSystem.current.IsPointerOverGameObject()) return;
            if (actionPoints == 0)
                return;

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(ray, out hit, mask))
                return;

            var roadToFollow = GridControl.Instance.GetPathToFollow(transform.position, hit.point);

            if (roadToFollow == null) return;
            isInAction = true;

            CyberonCamera.Instance.Follow();
            GridControl.Instance.FreeTile(transform.position);
            GridControl.Instance.RefreshPlayMap();
            UIManager.Instance.DisablePlayerUI();
            charMovement.MoveCharacter(roadToFollow);
        }
    }


    public void GatherAction()
    {
        foreach (var silo in GameStateManager.Instance.AllResourceSilos)
        {
            if (GridControl.Instance.IsInRange(this.gameObject.transform.position,
                silo.gameObject.transform.position, silo.AccessDistance))
            {
                silo.Gather();
            }
        }
    }

    public void HackStartAction()
    {
        Debug.Log("Hacks nearby terminal!");
        Terminal terminal = null;

        foreach (Terminal t in GameStateManager.Instance.AllTerminals)
        {
            if (GridControl.Instance.IsInRange(this.gameObject.transform.position,
                t.gameObject.transform.position, t.AccessDistance))
            {
                terminal = t;
                continue;
            }
        }
        if (terminal == null) return;
        isInAction = true;
        UIManager.Instance.EnableHackUI();

        HackingSystem.Instance.InitiateHacking(terminal);                        
    }

    public void HackEndAction()
    {
        UIManager.Instance.DisableHackUI();
        isInAction = false;

        // Hardcode refresh for now!
        GridControl.Instance.RefreshPlayMap();
        GridControl.Instance.DisplayRange(transform.position, actionPoints, maxActionPoints);
    }

    public void BindUI()
    {
        UIManager.Instance.SetMaxSpeed(maxActionPoints);
        UIManager.Instance.SetEnergy();
        UIManager.Instance.SetEndTurnBtn(() => this.EndTurn());
        UIManager.Instance.SetGatherBtn(() => this.GatherAction());
        UIManager.Instance.SetHackBtn(() => this.HackStartAction());
        UIManager.Instance.SetCloseHackBtn(() => this.HackEndAction());
    }
}
