using System;
using UnityEngine;
using UnityEngine.UI;

namespace Managers
{
    public class ScreenFade : SingletonMono<ScreenFade> {

        //Make instance of this script to be able reference from other scripts!

        [Header("Initialization")]
        //Game objects used by this code
        public Image fadeScreenImage;

        [Header("Fade Settings")]
        //For changing the duration of the fade
        public float fadeSpeed = 1;
        
        //Check to allow fade in/out
        // [HideInInspector]
        public bool fadeToBlack;
        // [HideInInspector]
        public bool fadeFromBlack;
        // [HideInInspector]
        public bool fading = false;

        private Action fadeToBlackDone; 
        private Action fadeFromBlackDone; 

	    // Use this for initialization
	    void Start () {
            instance = this;
            DontDestroyOnLoad(gameObject);
            fadeScreenImage = Instantiate(Resources.Load<GameObject>("UI/Screen Fade"), UIManager.MainCanvas.Instance.transform).GetComponent<Image>();
            fadeScreenImage.raycastTarget = false;
        }
	    
	    // Update is called once per frame
	    void Update () {
            // 每帧将这个UI元素移到它的父节点的最后
            // fadeScreenImage.transform.SetAsLastSibling();
            
            //Set alpha of fade screen image to 1 over time (fadespeed) in order to fade to black
            if (fadeToBlack)
            {
                fadeScreenImage.color = new Color(fadeScreenImage.color.r, fadeScreenImage.color.g, fadeScreenImage.color.b, Mathf.MoveTowards(fadeScreenImage.color.a, 1f, fadeSpeed * Time.deltaTime));

                if(fadeScreenImage.color.a == 1f)
                {
                    fadeToBlack = false;
                    fadeToBlackDone?.Invoke();
                }
            }

            //Set alpha of fade screen image to 0 over time (fadespeed) in order to fade from black
            if (fadeFromBlack)
            {
                fadeScreenImage.color = new Color(fadeScreenImage.color.r, fadeScreenImage.color.g, fadeScreenImage.color.b, Mathf.MoveTowards(fadeScreenImage.color.a, 0f, fadeSpeed * Time.deltaTime));

                if (fadeScreenImage.color.a == 0f)
                {
                    fadeFromBlack = false;
                    fadeFromBlackDone?.Invoke();
                }
            }
        }

        //Method to activae fading
        public void FadeToBlack(float speed = 1, Action callback = null)
        {
            fadeToBlackDone = callback;
            fadeSpeed = speed;
            fadeToBlack = true;
            fadeFromBlack = false;
            fading = true;

        }

        //Method to activae fading
        public void FadeFromBlack(float speed = 1, Action callback = null)
        {
            fadeFromBlackDone = callback;
            fadeSpeed = speed;
            fadeToBlack = false;
            fadeFromBlack = true;
            fading = false;
        }
    }

    
    //Mono单例 重写Awake决定是否持续存在
    public class SingletonMono<T> : MonoBehaviour where T : Component
    {
        protected static T instance;
        public static bool bHasInstance => instance != null;

        public bool bPersistent = false;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                    if (instance == null)
                    {
                        var go = new GameObject("[" + typeof(T).Name + "_Auto-Generated" + "]");
                        instance = go.AddComponent<T>();
                    }
                }
                return instance;
            }
        }

        protected virtual void Awake()
        {
            Init(bPersistent);
        }

        protected void Init(bool bPersistent)
        {
            if (!Application.isPlaying)
                return;

            if (instance == null)
            {
                instance = this as T;

                OnInit();

                if (bPersistent)
                    DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }

        protected virtual void OnInit()
        { 
            //这里写一些额外的初始化内容
        }

    }
}