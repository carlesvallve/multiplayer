using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Anim : MonoBehaviour {

  public Animator animator;
  private PlayerState animState = PlayerState.Idle;

  public void Play(string stateName, float normalizedTime = 1f) {
    animator.Play(stateName, -1, normalizedTime);
  }

  public void CrossFade(string stateName, float normalizedTransitionDuration = 0.1f, int layer = -1, float normalizedTimeOffset = float.NegativeInfinity, float normalizedTransitionTime = 0.0f) {
    animator.CrossFade(stateName, normalizedTransitionDuration, layer, normalizedTimeOffset, normalizedTransitionTime);
  }


  public void UpdateAnimations(PlayerState state) {
    if (state == animState) {
      return;
    }

    CrossFade(state.ToString());
    animState = state;
  }
}
