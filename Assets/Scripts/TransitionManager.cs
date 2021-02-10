using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public void Transition(float inDuration, float waitDuration, float outDuration, Action callback)
    {
        StartCoroutine(TransitionRoutine(inDuration, waitDuration, outDuration, callback));
    }

    private IEnumerator TransitionRoutine(float inDuration, float waitDuration, float outDuration, Action callback)
    {
        _animator.speed = 1 / inDuration;
        _animator.SetTrigger("TransitionIn");

        yield return new WaitForSecondsRealtime(inDuration);

        callback();

        yield return new WaitForSecondsRealtime(waitDuration);
        
        _animator.speed = 1 / outDuration;
        _animator.SetTrigger("TransitionOut");
    }
}
