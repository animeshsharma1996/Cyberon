using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    public float DestroyTime = 3.0f;
    public Vector3 offset = new Vector3(0.0f, 2.0f, 0.0f);
    // Start is called before the first frame update
    Camera cybCamera = null;
    public bool isLookingAtCamera = true;

    void Start()
    {
        transform.localPosition += offset;
        cybCamera = CyberonCamera.Instance.gameObject.GetComponent<Camera>();
        StartCoroutine(DisplaySaveTxtDestroy(this.gameObject));
    }

    void LateUpdate()
    {
        if (!isLookingAtCamera) return;
        transform.LookAt(transform.position + cybCamera.transform.rotation * Vector3.forward,
            cybCamera.transform.rotation * Vector3.up);
    }


    IEnumerator DisplaySaveTxtDestroy(GameObject savetxt)
    {

        yield return new WaitForSecondsRealtime(DestroyTime);
        Destroy(savetxt);

        yield break;
    }
}
