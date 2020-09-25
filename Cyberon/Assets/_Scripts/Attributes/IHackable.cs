using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHackable
{
    void Hack();
    bool IsHacked();
    GameObject GetGameObject();
    HackDescriptor GetHackInfo();
}
