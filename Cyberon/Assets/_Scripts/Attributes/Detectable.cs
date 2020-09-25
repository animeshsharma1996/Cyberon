using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent (typeof(CyberonActor))]
public class Detectable : MonoBehaviour
{
    [SerializeField] private Animator playerAnimator = null;

    public bool IsDetected { get; private set; }

    private void Start()
    {
        IsDetected = false;
       
    }
   
    void Awake()
    {
        playerAnimator = GetComponentInChildren<Animator>();
        Assert.AreNotEqual(playerAnimator, null, "Detectable could not find Animator");
    }

    private void SetIsDetected()
    {
        IsDetected = true;
        UponDetection();
    }

    //If Detected and dies, than the player will have to stop moving
    //Will have to remove this later on because the player wont die just because its Detected
    public void UponDetection()
    {
        playerAnimator.SetBool("Die", true);
    }

}
