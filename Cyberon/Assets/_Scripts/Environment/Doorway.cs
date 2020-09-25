using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doorway : MonoBehaviour, IHackable
{
    private Vector3[] blockPoints = new Vector3[4]; 
    private bool isUnlocked = false;

    [SerializeField]
    private HackDescriptor hackInfo = null;

    [SerializeField]
    private Material locked = null;
    [SerializeField]
    private Material unlocked = null;

    [SerializeField]
    private MeshRenderer Shield = null;
    [SerializeField]
    private MeshRenderer Shield2 = null;

    [SerializeField]
    private MeshRenderer Side1 = null;
    [SerializeField]
    private MeshRenderer Side2 = null;

    public void Occupy()
    {
        // Assign block points
        Vector3 pos = gameObject.transform.position;
        blockPoints[0] = pos + new Vector3(0.5f, 0.0f, 0.5f);
        blockPoints[1] = pos + new Vector3(-0.5f, 0.0f, 0.5f);
        blockPoints[2] = pos + new Vector3(0.5f, 0.0f, -0.5f);
        blockPoints[3] = pos + new Vector3(-0.5f, 0.0f, -0.5f);

        foreach (Vector3 blockPoint in blockPoints)
        {
            GridControl.Instance.OccupyTile(blockPoint);
            GridControl.Instance.ObstructTile(blockPoint);
        }
    }
    public GameObject GetGameObject()
    {
        return this.gameObject;
    }

    public HackDescriptor GetHackInfo()
    {
        return hackInfo;
    }

    public void Hack()
    {
        Debug.Log("Hacking doorway" + isUnlocked + " -> " + !isUnlocked);
        isUnlocked = !isUnlocked;

        if (isUnlocked) {
            Shield.enabled = false;
            Shield2.enabled = false;
            Side1.material = unlocked;
            Side2.material = unlocked;

            foreach (Vector3 blockPoint in blockPoints)
            {
                GridControl.Instance.FreeTile(blockPoint);
                GridControl.Instance.UnObstructTile(blockPoint);
            }
        } else
        {
            Shield.enabled = true;
            Shield2.enabled = true;
            Side1.material = locked;
            Side2.material = locked;

            foreach (Vector3 blockPoint in blockPoints)
            {
                GridControl.Instance.OccupyTile(blockPoint);
                GridControl.Instance.ObstructTile(blockPoint);
            }
        }
    }

    public bool IsHacked()
    {
        return isUnlocked;
    }
}
