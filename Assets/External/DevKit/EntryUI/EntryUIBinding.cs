using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Entry
{
    public class EntryUIBinding : MonoBehaviour
    {
        public Func<string,UniTask<bool>> OnTGenerate; 
        public Func<string,UniTask<bool>> OnGenerate; 
        public Func<UniTask<bool>> OnGenerateTemplate;
        public Action OnFailed;

        [Header("UI")]
        public TMP_InputField input;
        public Button createTemplate;
        public Button Regenerate;
        public GameObject Tip;
        void Start()
        {
            Tip.SetActive(false);
            Regenerate.gameObject.SetActive(false);

            createTemplate.gameObject.SetActive(true);
            Regenerate.gameObject.SetActive(true);
        }
        private void OnEnable()
        {
            
            Regenerate.onClick.AddListener(OnBack);
            createTemplate.onClick.AddListener(()=>GenerateTemplate().Forget());
            input.onEndEdit.AddListener(url => Generate(url).Forget());
        }
        private void OnDisable()
        {
            Regenerate.onClick.RemoveAllListeners();
            input.onEndEdit.RemoveAllListeners();
            createTemplate.onClick.RemoveAllListeners();
        }
        private async UniTask GenerateTemplate()
        {
            Tip.SetActive(true);
            createTemplate.gameObject.SetActive(false);
            Regenerate.gameObject.SetActive(false);
            input.gameObject.SetActive(false);
            bool res = await OnGenerateTemplate.Invoke();
            if(res)
            {
                OnSuccess();
            }
            else{
                OnBack();
            }
        }   
        private async UniTask Generate(string url)
        {
            Tip.SetActive(true);
            createTemplate.gameObject.SetActive(false);
            Regenerate.gameObject.SetActive(false);
            input.gameObject.SetActive(false);
            // Serializer
            bool res = await OnGenerate.Invoke(url);
            if(res)
            {
                OnSuccess();
            }
            else{
                OnBack();
            }
        }

        private void OnBack()
        {
            OnFailed?.Invoke();
            Tip.SetActive(false);
            input.gameObject.SetActive(true);
            createTemplate.gameObject.SetActive(true);
            Regenerate.gameObject.SetActive(false);
        }
        private void OnSuccess()
        {
            Tip.SetActive(false);
            input.gameObject.SetActive(false);
            createTemplate.gameObject.SetActive(false);
            Regenerate.gameObject.SetActive(true);
        }
    }
}

