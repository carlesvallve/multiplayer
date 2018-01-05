using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anim : MonoBehaviour {

  //public Animator animator;
  public Animation anim;
  private PlayerState animState = PlayerState.Idle;

  // public void Play(string stateName, float normalizedTime = 1f) {
  //   //animation.Play(stateName, 0.2f); //-1, normalizedTime);
  // }

  // public void CrossFade(string stateName, float duration = 0.2f) {
  //   //animation.CrossFade(stateName, normalizedTransitionDuration, layer, normalizedTimeOffset, normalizedTransitionTime);
  //   anim.CrossFade(stateName, duration);
  // }


  public void UpdateAnimations(PlayerState state) {
    //Debug.Log("animState:" + animState + "state: " + state);
    if (state == animState) {
      return;
    }

    string animName = GetAnimationClipFromState(state);
    Debug.Log(state + " " + animName);

    //CrossFade(state.ToString());
    anim.CrossFade(animName, 0.2f);
    animState = state;
  }


  private string GetAnimationClipFromState(PlayerState state) {
    switch (state) {
      case PlayerState.Idle:
        return "soldierIdleRelaxed";
      case PlayerState.Run:
        return "soldierRun";
      case PlayerState.Aim:
        return "soldierIdle";
      case PlayerState.Shoot:
        return "soldierFiring";
      case PlayerState.Hit:
        return "soldierHitFront";
      case PlayerState.Die:
        return "soldierDieFront";
    }

    return "soldierIdleRelaxed";
  }
}
