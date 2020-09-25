using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;

[RequireComponent (typeof(CharacterMovement), typeof(Detector), typeof(Patroller))]
public class DroneController : TurnActor , IHackable
{
    [SerializeField]
    private HackDescriptor HackInfo = null;
    private bool isDisabled = false;

    private Patroller patroller = null; 
    private Detector detector = null;

    public bool IsDisabled { get => isDisabled; set => isDisabled = value; }

    void Awake()
    {
        detector = GetComponent<Detector>();
        patroller = GetComponent<Patroller>();
    }

    #region Actor
    override public void OnUpdate()
    {
        if (!detector.IsActive) return;

        detector.RedrawDetectionLayer();
        if (!(GameStateManager.Instance.Phase == GamePhase.Stealth)) return;

        if (detector.Detect())
        {
            GameStateManager.Instance.ChangePhase();
        }
    }
    #endregion

    #region TurnActor
    override public void OnTurnExecute()
    {
        if (isDisabled)
        {
            EndTurn();
            return;
        }

        if (isInAction) return;

        if (actionPoints == 0)
        {
            EndTurn();
            return;
        }
        patroller.Patrol();
    }
    #endregion

    #region IHackable
    public void Hack()
    {
        isDisabled = !isDisabled;
        isDead = IsDisabled;
        detector.ShouldDetect(!isDisabled);
    }

    public bool IsHacked()
    {
        return isDisabled;
    }

    public GameObject GetGameObject()
    {
        return this.gameObject;
    }

    public HackDescriptor GetHackInfo()
    {
        return HackInfo;
    }
#endregion
}
