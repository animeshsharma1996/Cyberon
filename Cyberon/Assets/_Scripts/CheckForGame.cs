using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckForGame : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        bool initTheGame = GameStateManager.Instance.isInit;
    }

}
