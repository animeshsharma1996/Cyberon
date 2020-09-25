using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XMark : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Vector3 lookAtVector = CyberonCamera.Instance.gameObject.transform.position;
        lookAtVector.y = transform.position.y;
        transform.LookAt(lookAtVector);
    }
}
