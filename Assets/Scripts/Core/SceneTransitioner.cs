using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CON.Core
{
    public class SceneTransitioner : MonoBehaviour
    {
        [SerializeField] float timeToWait = 1f;
        Animator animator;
        private void Awake()
        {
            animator = GetComponent<Animator>();
        }
        public IEnumerator EndScene()
        {
            animator.SetTrigger("endScene");
            yield return new WaitForSecondsRealtime(timeToWait);
        }
        public IEnumerator StartScene()
        {
            print("hello");
            animator.SetTrigger("startScene");
            yield return new WaitForSecondsRealtime(timeToWait);
        }
    }

}