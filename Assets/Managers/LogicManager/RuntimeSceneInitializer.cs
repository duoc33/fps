using System.Collections.Generic;
using Managers.Runtime;
using Managers.ScriptableObjects;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
    public static class RuntimeSceneInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeSceneAndUI()
        {
            // 检查是否有场景已经加载
            if (SceneManager.sceneCount == 0)
            {
                // 如果没有场景，则创建一个新的场景
                CreateNewScene();
            }

            // 创建基本UI
            // CreateUI();
            // CreateJson2GameUI();
            CreateGameEntranceUI();
        }

        private static void CreateGameEntranceUI()
        {
            var gameEntranceUI = Object.Instantiate(Resources.Load<GameObject>("UI/GameEntranceUI"));
            UIManager.AddToScreen(gameEntranceUI);
            var uiElement = gameEntranceUI.GetComponent<RectTransform>();
            uiElement.anchorMin = new Vector2(0, 0f);
            uiElement.anchorMax = new Vector2(1f, 1f);
            uiElement.offsetMin = new Vector2(0, 0);
            uiElement.offsetMax = new Vector2(0, 0);

            if (GameGeneratorSettings.instance.BG_Image != null)
            {
                gameEntranceUI.GetComponentInChildren<Image>().sprite = GameGeneratorSettings.instance.BG_Image;
            }
            else
            {
                gameEntranceUI.GetComponentInChildren<Image>().color = GameGeneratorSettings.instance.BG_Color;
            }
            
            var inputField = gameEntranceUI.GetComponentInChildren<InputField>();
            
            EnvironmentUtils.Env.Initialize();

            inputField.text = EnvironmentUtils.Env.Dict.GetValueOrDefault("id");
            
            gameEntranceUI.GetComponentInChildren<Button>().onClick.AddListener(async () =>
            {
                Debug.Log($"InputField: {inputField.text}");
                if (string.IsNullOrEmpty(inputField.text))
                {
                    return;
                }
                
                if (inputField.text.StartsWith("http") || inputField.text.StartsWith("file")){
                    await ParseManager.RunFromUrl(inputField.text);
                    gameEntranceUI.SetActive(false);
                }
                else{
                    await ParseManager.RunFromID(inputField.text, () =>
                    {
                        gameEntranceUI.SetActive(false);
                    });
                }
            });
        }


        private static void CreateNewScene()
        {
            // 创建一个新的空场景并设置为活动场景
            Scene newScene = SceneManager.CreateScene("GeneratedScene");
            SceneManager.SetActiveScene(newScene);

            Debug.Log("New scene created: GeneratedScene");
        }

        private static void CreateUI()
        {
            // 创建Canvas
            GameObject canvasObject = new GameObject("Canvas");
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            // 添加CanvasScaler和GraphicRaycaster以确保UI显示正常
            canvasObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasObject.AddComponent<GraphicRaycaster>();

            // 创建Text UI
            GameObject textObject = new GameObject("WelcomeText");
            textObject.transform.SetParent(canvasObject.transform);

            Text text = textObject.AddComponent<Text>();
            text.text = "Welcome to the auto-generated scene!";
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.alignment = TextAnchor.MiddleCenter;

            // 设置Text的尺寸和位置
            RectTransform rectTransform = text.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(600, 200);
            rectTransform.anchoredPosition = Vector2.zero;

            Debug.Log("UI created with welcome message.");
        }
    }
}