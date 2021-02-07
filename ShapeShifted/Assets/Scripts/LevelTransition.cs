using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTransition : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] GameManagement gameManagement;

    public void StartTransition() 
    {
        animator.Play("TransitionStart");
    }

    public void StartTransitionFinished() 
    {
        gameManagement.ResetGame();
        animator.Play("TransitionEnd");
    }
}
