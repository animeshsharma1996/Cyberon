using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(TurnActor), typeof(CharacterMovement))]
public class Patroller : MonoBehaviour
{
    [SerializeField] List<Vector3> patrolPoints = new List<Vector3>();
    public int currentPatrolPoint = 0;

    CharacterMovement characterMovement = null;
    TurnActor turnActor = null;
    void Start()
    {
        if (patrolPoints.Count == 0)
            Debug.LogError("Patroller for " + gameObject.ToString() + " has no patroll points!");
    }

    void Awake()
    {
        characterMovement = GetComponent<CharacterMovement>();
        turnActor = GetComponent<TurnActor>();
    }
    public void Patrol()
    {
        SetPatrolPoint();
        List<Node> roadToFollow = GridControl.Instance.
            GetPathToFollow(transform.position, patrolPoints[currentPatrolPoint]);
        if (roadToFollow == null)
        {
            Debug.Log(this.gameObject.name + ": No road to follow.");
            turnActor.EndTurn();
            return;
        }

        turnActor.IsInAction = true;
        characterMovement.MoveCharacter(roadToFollow);
        GridControl.Instance.FreeTile(transform.position);
    }


    public void SetPatrolPoint()
    {
        // If patrol point reached switch to next patrol point
        if (HasReachedTarget(patrolPoints[currentPatrolPoint]))
        {
            currentPatrolPoint = (currentPatrolPoint + 1) % patrolPoints.Count;
        }
    }

    private bool HasReachedTarget(Vector3 targetPos)
    {
        return targetPos.x == transform.position.x && targetPos.z == transform.position.z;
    }
}
