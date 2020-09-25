using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(TurnActor), typeof(CharacterMovement))]
public class Hunter : MonoBehaviour
{
    [SerializeField]
    private GameObject commDetection = null;

    public bool isHunting = false;
    public GameObject huntTarget = null;

    private TurnActor actor = null;
    private CharacterMovement charMovement = null;

    void Awake()
    {
        actor = GetComponent<TurnActor>();
        charMovement = GetComponent<CharacterMovement>();
        commDetection.SetActive(false);
    }

    public void StartHunt(GameObject target)
    {
        commDetection.SetActive(true);
        
        isHunting = true;
        huntTarget = target;
    }



    public bool Hunt()
    {
        Node myNode = GridControl.Instance.GetMapNode(gameObject.transform.position);
        Node myTargetNode = GridControl.Instance.GetMapNode(huntTarget.transform.position);

        // If target is already in range and hunter can kill it
        if (CanReachFrom(myNode, myTargetNode))
        {
            Debug.Log("Can Reach");
            return true;
        }

        //// Analize the tiles around the hunt target
        //// ------------------------------------------------------------
        //List<Node> surroundingNodes = new List<Node>();
        //var node = GridControl.Instance.GetMapNode(huntTarget.transform.position + new Vector3(1.0f, 0.0f, 0.0f));
        //if (node != null && !node.isOccupied) surroundingNodes.Add(node);
        //node = GridControl.Instance.GetMapNode(huntTarget.transform.position + new Vector3(-1.0f, 0.0f, 0.0f));
        //if (node != null && !node.isOccupied) surroundingNodes.Add(node);
        //node = GridControl.Instance.GetMapNode(huntTarget.transform.position + new Vector3(0.0f, 0.0f, 1.0f));
        //if (node != null && !node.isOccupied) surroundingNodes.Add(node);
        //node = GridControl.Instance.GetMapNode(huntTarget.transform.position + new Vector3(0.0f, 0.0f, -1.0f));
        //if (node != null && !node.isOccupied) surroundingNodes.Add(node);

        //// If no path exists skip turn
        //// ------------------------------------------------------------
        //if (surroundingNodes.Count == 0)
        //{
        //    //Debug.LogWarning("No path exists");
        //    actor.EndTurn();
        //    return;
        //}

        //// Find shortest Hunt path
        //// ------------------------------------------------------------
        //int minRange = 10000; // some big number 
        //List<Node> shortestHuntPath = new List<Node>();

        //foreach (Node n in surroundingNodes)
        //{
        //    var path = GridControl.Instance.GetPathToFollow(myNode.WorldPos, n.WorldPos);
        //    if (path != null)
        //        if (path.Count < minRange)
        //        {
        //            minRange = path.Count;
        //            shortestHuntPath = path;
        //        }
        //}

        List<Node> shortestHuntPath = GridControl.Instance.GetPathToFollow(myNode.WorldPos, huntTarget.transform.position);

        // If no road was found skip turn
        if (shortestHuntPath == null)
        {
            //actor.EndTurn();
            return false;
        }

        if (shortestHuntPath.Count == 0)
        {
            //actor.EndTurn();
            return false;
        }

        // If we cannot reach into a position that we shoot the target in the amount of action points
        // ------------------------------------------------------------
        if (shortestHuntPath.Count >= actor.ActionPoints + actor.range)
        {
            Move(shortestHuntPath);
            return true;
        }


        // Find the first tile from which you can shoot the target
        // -------------------------------------------------------
        int pointsToMove = 0;
        int tileToReach = shortestHuntPath.Count;

        Debug.Log(shortestHuntPath.Count);
        do
        {
            // Simulate step
            tileToReach--;
            pointsToMove++;
            Debug.Log("tile to reach: " + tileToReach);
            // Check if he can shoot from the newly stepped tile
            if (CanReachFrom(shortestHuntPath[tileToReach], myTargetNode))
                break;


        } while (tileToReach > 0 && pointsToMove < actor.ActionPoints);


        // if used all action points to move
        if (pointsToMove == actor.ActionPoints)
        {
            Move(shortestHuntPath);
            return true;
        }

        if (tileToReach == -1) Debug.LogError("Hunter cannot shoot from any of the tiles around the player!");


        // Remove all tiles except the first "points to move" ones
        // (i.e. path will lead to a tile where it can kill the player from)
        while (shortestHuntPath.Count > pointsToMove)
        {
            shortestHuntPath.RemoveAt(0);
        }

        // Move to that tile
        Move(shortestHuntPath);
        return true;
    }


    private bool CanReachFrom(Node start, Node target)
    {

        List<Node> pathDistance = GridControl.Instance.GetPathToFollow(start.WorldPos, target.WorldPos);

        if (pathDistance == null) return false;

        if (pathDistance.Count >= actor.range) return false;
       // return false;
        //if (!GridControl.Instance.IsInRange(start.WorldPos, target.WorldPos, actor.range))
        //    return false;


        // Hardcoded offsets so that raycasting is not done below the map
        Vector3 targetPos = target.WorldPos + new Vector3(0.0f, 0.25f, 0.0f);
        Vector3 startPos = start.WorldPos + new Vector3(0.0f, 0.25f, 0.0f);


        Vector3 direction = targetPos - startPos;
        RaycastHit hit;
        Ray ray = new Ray(startPos, direction);

        if (Physics.Raycast(ray, out hit, actor.range))
        {
            if (hit.transform.gameObject == huntTarget)
                return true;
        }

        return false;
    }

    private void Move(List<Node> path)
    {
        actor.IsInAction = true;
        charMovement.MoveCharacter(path);
        GridControl.Instance.FreeTile(transform.position);
    }
}
