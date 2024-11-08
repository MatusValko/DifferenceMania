using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public class Events : MonoBehaviour
{
    [Header("Events")]
    [SerializeField]
    private Animator[] _eventAnimators = new Animator[4];
    // Start is called before the first frame update
    void Start()
    {
    }
    void OnEnable()
    {
        StartCoroutine(playAnimations());
    }

    IEnumerator playAnimations()
    {
        // Debug.Log("TU");
        while (true)
        {
            foreach (var animator in _eventAnimators)
            {
                Debug.Log(animator.name);
                animator.Play("Event", -1, 0);
                yield return new WaitForSeconds(5);
            }
        }
    }

    public void PlayAnimation(string animationName)
    {
        // Play the specific animation, ignoring the state machine transitions
        // animator.Play(animationName);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
