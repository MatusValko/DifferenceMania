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

    void OnEnable()
    {
        StartCoroutine(playAnimations());
    }

    IEnumerator playAnimations()
    {
        while (true)
        {
            foreach (var animator in _eventAnimators)
            {
                // Debug.Log(animator.name);
                if (animator.gameObject.activeSelf == false)
                {
                    // Skip this animator if it is not active
                    continue;
                }
                animator.Play("Event", -1, 0);
                yield return new WaitForSeconds(5);
            }
        }
    }
}
