using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


[RequireComponent (typeof(CyberonActor))]
public class Detector : MonoBehaviour
{
    public bool isDeadly = false;
    private float range = 4;
    private List<Node> detectionLayer = new List<Node>();

    private Node lastNode = null;

    public bool IsActive { get; set; }
    public GameObject DetectedObject { get; private set; }

    void Awake()
    {
        range = GetComponent<CyberonActor>().range;
        IsActive = true;
        //ActivateDetectionLayer();
    }

    public bool Detect()
    {
        List<Detectable> detectableObjects = GameStateManager.Instance.AllDetectables;
        foreach (var detectable in detectableObjects)
        {
            Node detectableNode = GridControl.Instance.GetMapNode(detectable.gameObject.transform.position);
            foreach (Node node in detectionLayer)
            {
                if (node == detectableNode)
                {
                    Debug.Log("Detected");
                    DetectedObject = detectable.gameObject;
                    if(gameObject.name.StartsWith("Drone"))
                    {
                        StartCoroutine(PlayHuntingVoice());
                    }
                    return true;
                }
            }
            //if (GridControl.Instance.IsInRange(transform.position, detectable.transform.position, range))
            //{
            //    Debug.Log("Detected");
            //    DetectedObject = detectable.gameObject;
            //    return true;
            //}
        }
        return false;
    }
    private IEnumerator PlayHuntingVoice()
    {
        yield return new WaitForSeconds(2.0f);
        UIManager.Instance.PlayVoiceSound("huntingVoice");
    }

    public void RedrawDetectionLayer()
    {
        Node currentNode = GridControl.Instance.GetMapNode(gameObject.transform.position);
        if (currentNode != lastNode)
        {
            var newDetectionLayer = GridControl.Instance.SetAndGetDetectorMapLayer(gameObject.transform.position, (int)range);
            GridControl.Instance.RemoveDetectorMapLayer(detectionLayer);

            detectionLayer = newDetectionLayer;
            lastNode = currentNode;

        }
    }

    public void ForceRedrawDetectionLayer()
    {
        Node currentNode = GridControl.Instance.GetMapNode(gameObject.transform.position);
        var newDetectionLayer = GridControl.Instance.SetAndGetDetectorMapLayer(gameObject.transform.position, (int)range);
        GridControl.Instance.RemoveDetectorMapLayer(detectionLayer);

        detectionLayer = newDetectionLayer;
        lastNode = currentNode;
    }

    public void ShouldDetect(bool active)
    {
        IsActive = active;
        if (IsActive)
            ActivateDetectionLayer();
        else DeactivateDetectionLayer();
    }

    public void ActivateDetectionLayer()
    {
        detectionLayer = GridControl.Instance.SetAndGetDetectorMapLayer(gameObject.transform.position, (int)range);
    }

    public void DeactivateDetectionLayer()
    {
        GridControl.Instance.RemoveDetectorMapLayer(detectionLayer);
    }

    public bool DoesItDetect(Node nodeToDetect)
    {
        foreach (Node node in detectionLayer)
        {
            if (node == nodeToDetect) return true;
        }

        return false;
    }
}
