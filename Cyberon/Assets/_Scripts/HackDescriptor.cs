using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "HackDescriptor/Hack")]
public class HackDescriptor : ScriptableObject
{
    public string aName = "New Hack";
    public string aTarget = "Target name";
    public Sprite aSprite;
    public int aCost = 0;
    public string aDescription = "Description of the hack";
}
