using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
[RequireComponent (typeof(Animator))]
public class ResourceSilo : MonoBehaviour
{
    [SerializeField]
    private GameObject FloatingTextPrefab = null;

    [SerializeField]
    public int energyCount = 4;
    public bool isDepleted = false;
    private MeshRenderer meshRenderer = null;
    private Animator animator = null;
    private int accessDistance = 2;
    public int AccessDistance { get => accessDistance; set => accessDistance = value; }


    [SerializeField]
    private Material depleted = null;
    [SerializeField]
    private Material full = null;

    void Awake()
    {
        var sides = GetComponentsInChildren<MeshRenderer>();
        foreach (var side in sides)
        {
            side.material = full;
        }
        animator = GetComponent<Animator>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Gather()
    {
        if (isDepleted) {
            DisplayFloatingText("Depleted!");
            return;
        }
        DisplayFloatingText("Energy +" + energyCount.ToString());
        GameStateManager.Instance.PlayerStats.aEnergy += energyCount;
        // GameStateManager.Instance.PlayerStats.aActionPoints -= 1;
        UIManager.Instance.SetEnergy();


        Deplete();
    }

    public void Deplete()
    {
        energyCount = 0;
        animator.SetTrigger("Deplete");
        isDepleted = true;

        var sides = GetComponentsInChildren<MeshRenderer>();
        foreach (var side in sides)
        {
            side.material = depleted;
        }
    }

    void DisplayFloatingText(string text)
    {
        GameObject floatText = Instantiate(FloatingTextPrefab, transform.position, Quaternion.identity);
        floatText.GetComponentInChildren<TMP_Text>().text = text;
    }


    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Triger Enter");
            UIManager.Instance.EnableGatherButton();
        }
    }

    void OnTriggerExit(Collider other)
    {

        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Triger Exit");
            UIManager.Instance.DisableGatherButton();
        }
    }
}
