using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] GameObject Logo = null;

    [SerializeField] GameObject LogoGlitchPrefab = null;

    GameObject LogoGlitchObj = null;
    
    [SerializeField] AudioSource audioSource = null;
    [SerializeField] PlayableDirector playableDirector = null;
    [SerializeField] Material material = null;


    [Header("Timelines")]
    [SerializeField] List<PlayableAsset> timelines = null;

    public bool finAppearAnim = false;
    public bool finDisappearAnim = false;

    Animator animator = null;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void ActivateGlitch()
    {
        material.SetFloat("_Active", 1.0f);
    }

    public void DisableGlitch()
    {

        material.SetFloat("_Active", 0.0f);
    }
    public void Reset()
    {
        finAppearAnim = false;
        finDisappearAnim = false;
    }
    public void StartLoadingScreen()
    {
        finAppearAnim = false;
        gameObject.SetActive(true);
        animator.SetTrigger("Appear");
    }

    public void FinishedAppearing()
    {
        //finAppearAnim = true;
    }


    public void EndLoadingScreen()
    {
        finDisappearAnim = false;
        animator.SetTrigger("Disappear");
    }

    public void FinishedDisappearing()
    {
        finDisappearAnim = true;
        gameObject.SetActive(false);
    }

    public void GlitchFinished()
    {
        finAppearAnim = true;
    }
    public void SwitchLogos()
    {
     
        if (Logo.activeInHierarchy)
        {
            DisableGlitch();
            int toDirect = Random.Range(0, timelines.Count * 1024);
            toDirect %= timelines.Count;
            playableDirector.playableAsset = timelines[toDirect];
            playableDirector.Play();
            LogoGlitchObj = Instantiate(LogoGlitchPrefab, gameObject.transform);
            Logo.SetActive(false);
            //audioSource.Play();
        }
        else
        {
            DisableGlitch();
            //audioSource.Stop();
            playableDirector.Stop();

            Logo.SetActive(true);
            Destroy(LogoGlitchObj);
        }
    }
}
