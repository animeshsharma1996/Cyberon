using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent (typeof(Detector),typeof(Shooter))]
public class Turret : CyberonActor , IHackable
{
    [SerializeField]
    private HackDescriptor HackInfo = null;
    private bool isDisabled = false;

    [SerializeField]
    private GameObject forceField = null;
    private Detector detector = null;
    private Animator turretAC = null;
    private Shooter shooter = null;
    private Animator animator = null;


    [Header ("Attach Structure Meshes")]
    [SerializeField]
    private GameObject headPivot = null;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        turretAC = GetComponentInChildren<Animator>();
        Assert.AreNotEqual(turretAC, null, "Detectable could not find Animator");
        detector = GetComponent<Detector>();
        shooter = GetComponent<Shooter>();
    }

    override
    public void OnUpdate()
    {
        if (isDisabled) return;

        if (detector.IsActive && detector.Detect())
        {
            GameStateManager.Instance.ChangePhase();

            Vector3 lookAtPos = detector.DetectedObject.transform.position;
            lookAtPos.y = 1.0f;
            headPivot.transform.LookAt(lookAtPos);
            turretAC.enabled = false;
            turretAC.SetTrigger("Shoot");

            StartCoroutine(DelayShooting());
        }

        if (shooter.ShouldShoot)
            shooter.FireAt(detector.DetectedObject);
    }

    private IEnumerator DelayShooting()
    {
        yield return new WaitForSeconds(0.25f);
        shooter.ShouldShoot = true;
    }

    #region IHackable
    public void Hack()
    {
        isDisabled = !isDisabled;
        isDead = isDisabled;
        detector.ShouldDetect(!isDisabled);
        forceField.SetActive(!isDisabled);
        animator.SetBool("IsDisabled", isDisabled);
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
