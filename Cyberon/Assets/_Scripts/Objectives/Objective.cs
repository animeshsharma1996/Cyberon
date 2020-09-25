using UnityEngine;

public abstract class Objective : MonoBehaviour
{
    [SerializeField] public string Title = "";

    [SerializeField] public string Description = "";

    [SerializeField] public string Hint = "";

    [SerializeField] public int order = 9999;

    [SerializeField] private Transform Arrow = null;

    public string score = null;

    // Update is called once per frame
    public abstract bool isComplete();

    public abstract int AddScore(int value);

    public void OnUpdate()
    {
        Vector3 lookAtVector = CyberonCamera.Instance.gameObject.transform.position;
        lookAtVector.y = Arrow.position.y;
        Arrow.LookAt(lookAtVector);
    }

}
