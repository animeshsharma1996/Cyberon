using UnityEngine;

public class Hidden : MonoBehaviour
{
    [SerializeField]
    private GameObject hidden;  
    [SerializeField]
    private Vector3 position;

    private void OnTriggerEnter(Collider other)
    {
        Instantiate(hidden, position,Quaternion.identity);
    }
}
