using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CyberonActor : MonoBehaviour
{

    public bool isDead = false;
    public bool isHacked = false;
    [SerializeField]
    public float range;

    [SerializeField]
    protected Allegiance allegiance;
    public Allegiance Allegiance { get => allegiance; set => allegiance = value; }

    public abstract void OnUpdate();


}
