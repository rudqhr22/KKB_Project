using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CAT
{
    public class AnimatorManager : MonoBehaviour
    {
        public Animator anim;
        public void PlayTagetAnimation(string targetAnim, bool isInteracting)
        {
            anim.applyRootMotion = isInteracting;
            anim.SetBool("isInteracting", isInteracting);
            anim.CrossFade(targetAnim, 0.2f);
        }


    }
}