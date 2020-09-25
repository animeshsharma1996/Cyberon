using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;

[RequireComponent(typeof(Patroller), typeof(Detector), typeof(Hunter))]
[RequireComponent(typeof(Shooter))]
public class HunterController : TurnActor
{

    private bool shouldBreakAction = false;
    private CharacterMovement charMovement = null;
    private Patroller patroller = null;
    private Detector detector = null;
    private Hunter hunter = null;
    private Shooter shooter = null;

    void Awake()
    {
        hunter = GetComponent<Hunter>();
        shooter = GetComponent<Shooter>();
        detector = GetComponent<Detector>();
        patroller = GetComponent<Patroller>();
        charMovement = GetComponent<CharacterMovement>();
        Assert.AreNotEqual(charMovement, null, "Drone Controller did not find Character Movement");
    }

    #region Actor
    override
    public void OnUpdate() 
    {
        if (!detector.IsActive) return;


        detector.RedrawDetectionLayer();
  

        if (detector.Detect())
        {
            charMovement.shouldBreakmovement = true;
            GameStateManager.Instance.ChangePhase();

            Vector3 lookAtPos = detector.DetectedObject.transform.position;
            lookAtPos.y = 1.0f;
            gameObject.transform.LookAt(lookAtPos);

            StartCoroutine(DelayShooting());
        }

        if (shooter.ShouldShoot)
            shooter.FireAt(detector.DetectedObject);


    }
    #endregion

    #region TurnActor
    public override void OnTurnStart()
    {
        charMovement.shouldBreakmovement = false;
        base.OnTurnStart();
    }

    override public void OnTurnExecute()
    {
        if (isInAction ) return;

        if (actionPoints == 0)
        {
            EndTurn();
            return;
        }

        if (hunter.isHunting)
        {
            if (hunter.Hunt()) return;
        }

        patroller.Patrol();
    }
    #endregion

    private IEnumerator DelayShooting()
    {
        yield return new WaitForSeconds(0.25f);
        shooter.ShouldShoot = true;
    }
}
