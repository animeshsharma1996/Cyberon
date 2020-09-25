using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Terminal : MonoBehaviour
{
    private int accessDistance = 2;
    public int AccessDistance { get => accessDistance; set => accessDistance = value; }


    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Triger Enter");
            UIManager.Instance.EnableHackButton();
        }
    }

    void OnTriggerExit(Collider other)
    {

        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Triger Exit");
            UIManager.Instance.DisableHackButton();
        }
    }
}