using System.Collections;
using UnityEngine;

[RequireComponent (typeof(CyberonActor))]
public class Shooter : MonoBehaviour
{
    private bool shootLaser = false;

    [SerializeField] private GameObject laserPrefab = null;
    [SerializeField] private GameObject laserOut = null;
    private GameObject target = null;

    public bool ShouldShoot { get; set; }

    private CyberonActor actor = null;

    void Start()
    {
        ShouldShoot = false;

    }
    void Awake()
    {
        actor = GetComponent<CyberonActor>();
    }

    public void FireAt(GameObject _target)
    {
        if (shootLaser) return;
        Debug.LogWarning("SHOOTING PLAYER!");

        target = _target;
        StartCoroutine(ShootLaser());

        // Should fix this
        RaycastHit hit;
        Vector3 targetPos = target.transform.position;
        targetPos.y = laserOut.transform.position.y;

        Vector3 direction = targetPos - laserOut.transform.position;
        if (Physics.Raycast(laserOut.transform.position, direction, out hit, actor.range))
        {
            Debug.LogWarning("SHOOTING PLAYER!");
            //Kill player;
            target.SendMessage("UponDetection");
            target.GetComponentInChildren<Animator>().SetBool("IsDead", true);
            shootLaser = true;
        }
    }

    private IEnumerator ShootLaser()
    {

        // Instantiate laser
        GameObject laserObj = Instantiate(laserPrefab);
        GameObject laserLine = laserObj.GetComponentInChildren<LineRenderer>().gameObject;

        // Set size
        Vector3 targetPos = target.transform.position;
        targetPos.y = 1.0f;
        float laserSize = Vector3.Distance(targetPos, laserOut.transform.position);
        Vector3 laserScale = Vector3.one;
        laserScale.z = laserSize;
        laserLine.transform.localScale = laserScale;

        // Set position
        laserObj.transform.position = laserOut.transform.position;
        laserObj.transform.LookAt(targetPos);

        // Enable renderer
        laserLine.GetComponent<LineRenderer>().enabled = true;

        yield return new WaitForSeconds(0.5f);

        Destroy(laserObj);
        yield return new WaitForSeconds(1.0f);

        UIManager.Instance.PlayVoiceSound("dieVoice");
    }


    //public bool CanShootAt(GameObject target, Vector3 displacementVector = default(Vector3))
    //{

    //    // Hardcoded offsets so that raycasting is not done below the map
    //    Vector3 laserOutPosition = laserOut.transform.position + displacementVector;
    //    Vector3 targetPos = target.transform.position + new Vector3(0.0f, 1.0f, 0.0f);

    //    Vector3 direction = targetPos - laserOutPosition;
    //    RaycastHit hit;
    //    Ray ray = new Ray(targetPos, direction);

    //    if (Physics.Raycast(ray, out hit, actor.range))
    //    {

    //        if (hit.transform.gameObject == target.gameObject)
    //            return true;
    //    }
    //    return false;

    //}

}
