using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent (typeof(CyberonActor))]
public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private Animator animator = null;
    private TurnActor actor = null;

    //Character Stats
    [SerializeField, Range(0.1f, 10.0f)] private float moveSpeed = 1.5f;

    //Moving Data
    private List<Node> nodesToFollow = null;
    private int nodeIndex;
    public bool hasToMove = false;

    public bool shouldBreakmovement = false;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        Assert.AreNotEqual(animator, null, "Character Movement could not find Animator");

        actor = GetComponent<TurnActor>();
        Assert.AreNotEqual(actor, null, "Character Movement could not find Actor");

    }

    // Update is called once per frame
    void Update()
    {
        if (shouldBreakmovement) return;
        if (hasToMove && !actor.isDead) CharacterMoveAlongPath();
    }

    public void MoveCharacter(List<Node> pathOfNodes)
    {
        nodesToFollow = pathOfNodes;
        nodeIndex = nodesToFollow.Count - 1;
        hasToMove = true;
        if(animator != null)
            animator.SetBool("IsWalking", hasToMove = true);
    }

    //Move Player Character to the pressed mouse location
    private void CharacterMoveAlongPath()
    {
        if (nodeIndex < 0 || actor.ActionPoints <= 0)
        {
            hasToMove = false;
            if (animator != null)
                animator.SetBool("IsWalking", hasToMove = false);
            actor.IsInAction = false;
            return;
        }

        Vector3 targetPosition = nodesToFollow[nodeIndex].WorldPos;
        transform.LookAt(targetPosition);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (transform.position == targetPosition)
        {
            nodeIndex--;
            actor.ActionPoints--;
        }

    }
}


