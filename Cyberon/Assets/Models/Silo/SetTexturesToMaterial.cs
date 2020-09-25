using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTexturesToMaterial : StateMachineBehaviour
{
    public Material material;
    public Texture2D albedoMap;
    public Texture2D emissionMap;

    // This will be called when the animator first transitions to this state.
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        material.SetTexture("_MainTex", albedoMap);
        material.SetTexture("_EmissionMap", emissionMap);
    }
}
