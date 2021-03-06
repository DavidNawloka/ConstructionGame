using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace CON.Core
{
    public class SceneTransitioner : MonoBehaviour
    {
        [SerializeField] string[] tips;
        [SerializeField] Sprite[] screenshots;
        [SerializeField] float animationTime = 1f;
        [SerializeField] float startingTime = .5f;
        [SerializeField] Image screenshotImage;
        [SerializeField] TextMeshProUGUI tipTMPro;

        Animator animator;
        BackgroundMusicManager backgroundMusicManager;

        static int tipIndex = -1;
        static int screenshotIndex = -1;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            backgroundMusicManager = FindObjectOfType<BackgroundMusicManager>();
        }
        private void Start()
        {
            if(tipIndex != -1)UpdateScreenshotAndTip();
            StartCoroutine(StartScene());
        }
        public void LoadScene(int sceneIndex)
        {
            StartCoroutine(LoadSceneDelayed(sceneIndex));
        }
        public IEnumerator LoadSceneDelayed(int sceneIndex)
        {
            yield return EndScene();
            SceneManager.LoadScene(sceneIndex);
        }
        public IEnumerator EndScene()
        {
            
            tipIndex = GetRandomIndex(tips);
            screenshotIndex = GetRandomIndex(screenshots);
            UpdateScreenshotAndTip();

            animator.SetTrigger("endScene");
            yield return StartCoroutine(backgroundMusicManager.MuteMusic());
            yield return new WaitForSecondsRealtime(animationTime);
        }
        public IEnumerator StartScene()
        {
            yield return new WaitForSecondsRealtime(startingTime);
            animator.SetTrigger("startScene");
            yield return new WaitForSecondsRealtime(animationTime);
        }
        private void UpdateScreenshotAndTip()
        {
            screenshotImage.sprite = screenshots[screenshotIndex];
            tipTMPro.text = tips[tipIndex];
        }
        private int GetRandomIndex(Array array)
        {
            return UnityEngine.Random.Range(0, array.Length);
        }
    }

}