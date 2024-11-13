using System.Collections.Generic;
using System.Threading.Tasks;
using Managers.Behaviours;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Managers
{
    public enum UIPosition
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        Center,
        CenterLeft,
        CenterRight,
        None
    }

    public class UICalc
    {
        private Dictionary<UIPosition, List<RectTransform>> m_Cumulatively;

        public UICalc()
        {
            m_Cumulatively = new Dictionary<UIPosition, List<RectTransform>>
            {
                [UIPosition.TopLeft] = new(),
                [UIPosition.TopRight] = new(),
                [UIPosition.BottomLeft] = new(),
                [UIPosition.BottomRight] = new(),
                [UIPosition.Center] = new(),
                [UIPosition.CenterLeft] = new(),
                [UIPosition.CenterRight] = new()
            };
        }
        
        public void Add(UIPosition position, RectTransform uiElement)
        {
            m_Cumulatively[position].Add(uiElement);
            float x = 0, y = 0;
            for (var i = m_Cumulatively[position].Count - 1; i >= 0; i--)
            {
                m_Cumulatively[position][i].anchoredPosition = new Vector2(x, y);
                x -= m_Cumulatively[position][i].rect.width;
                // todo: ?? y
            }


            // return Vector2.zero;
        }
    }
    
    public class UIManager // : MonoBehaviour
    {
        public Transform UIRoot;

        private static readonly Dictionary<string, UICalc> m_Calcs = new();
        public static UICalc GetCalc(string canvasName) => m_Calcs.GetValueOrDefault(canvasName);

        public static class MainCanvas
        {
            private static Canvas mainCanvas;
            public static Canvas Instance => mainCanvas != null ? mainCanvas : CreateCanvas();
            private static Canvas CreateCanvas()
            {
                var gameObject = new GameObject("MainCanvas");
                mainCanvas = gameObject.AddComponent<Canvas>();
                mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                // var canvasScaler = PHUtils.TryAddComponent<CanvasScaler>(mainCanvas.gameObject);
                // canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                // canvasScaler.referenceResolution = new Vector2(1280, 1920); // todo:
                // canvasScaler.matchWidthOrHeight = 0.8f;
                var graphicRayCaster = PHUtils.TryAddComponent<GraphicRaycaster>(mainCanvas.gameObject);
                m_Calcs.Add("MainCanvas", new UICalc());
                return mainCanvas;
            }

            public static Dictionary<string, GameObject> UIObjDir = new();
        }
        
        // todo: 右上角方法, 获取rect的累积
        public static void AddToScreen(GameObject ui, UIPosition uiPosition = UIPosition.None, Vector2 rect = default, Vector3 localScale = default, string key = null, bool onStack = true)
        {
            if (!string.IsNullOrEmpty(key))
            {
                MainCanvas.UIObjDir.TryAdd(key, ui);
            }
            ui.transform.SetParent(MainCanvas.Instance.transform);
            var uiRect = ui.GetComponent<RectTransform>(); // todo: 
            if (uiPosition != UIPosition.None)
            {
                SetUIPosition(uiPosition, uiRect, onStack);
            }

            if (rect != default)
            {
                // rect = Vector2.zero;
                uiRect.anchoredPosition = rect;
            }

            if (localScale == default) localScale = Vector3.one;
            uiRect.localScale = localScale;
        }

        public static void AddToTarget(GameObject ui, Transform target, Vector3 offset, string key = null)
        {
            if (!string.IsNullOrEmpty(key))
            {
                MainCanvas.UIObjDir.TryAdd(key, ui);
            }
            ui.transform.SetParent(MainCanvas.Instance.transform);
            ui.AddComponent<WorldToScreenPoint>().SetTarget(target, offset);
        }

        public static void Collect()
        {
            
        }

        public static void OpenPopup(GameObject ui)
        {
            ui.transform.SetParent(MainCanvas.Instance.transform);
            ui.SetActive(true);
            ui.transform.localScale = Vector3.zero;
            var uiRect = ui.GetComponent<RectTransform>(); // todo: 
            uiRect.anchoredPosition = Vector3.zero;
        }

        public static void Init()
        {
        }

        public static void SetUIPosition(UIPosition position, RectTransform uiElement, bool onStack)
        {
            if(onStack) GetCalc("MainCanvas").Add(position, uiElement);
            var anchoredPosition = Vector2.zero;
            switch (position)
            {
                case UIPosition.TopLeft:
                    uiElement.anchorMin = new Vector2(0, 1);
                    uiElement.anchorMax = new Vector2(0, 1);
                    uiElement.pivot = new Vector2(0, 1);
                    uiElement.anchoredPosition = Vector2.zero;
                    break;

                case UIPosition.TopRight:
                    uiElement.anchorMin = new Vector2(1, 1);
                    uiElement.anchorMax = new Vector2(1, 1);
                    uiElement.pivot = new Vector2(1, 1);
                    uiElement.anchoredPosition = Vector2.zero;
                    break;

                case UIPosition.BottomLeft:
                    uiElement.anchorMin = new Vector2(0, 0);
                    uiElement.anchorMax = new Vector2(0, 0);
                    uiElement.pivot = new Vector2(0, 0);
                    uiElement.anchoredPosition = Vector2.zero;
                    break;

                case UIPosition.BottomRight:
                    uiElement.anchorMin = new Vector2(1, 0);
                    uiElement.anchorMax = new Vector2(1, 0);
                    uiElement.pivot = new Vector2(1, 0);
                    uiElement.anchoredPosition = Vector2.zero;
                    break;

                case UIPosition.Center:
                    uiElement.anchorMin = new Vector2(0.5f, 0.5f);
                    uiElement.anchorMax = new Vector2(0.5f, 0.5f);
                    uiElement.pivot = new Vector2(0.5f, 0.5f);
                    uiElement.anchoredPosition = Vector2.zero;
                    break;

                case UIPosition.CenterLeft:
                    uiElement.anchorMin = new Vector2(0, 0.5f);
                    uiElement.anchorMax = new Vector2(0, 0.5f);
                    uiElement.pivot = new Vector2(0, 0.5f);
                    uiElement.anchoredPosition = Vector2.zero;
                    break;

                case UIPosition.CenterRight:
                    uiElement.anchorMin = new Vector2(1, 0.5f);
                    uiElement.anchorMax = new Vector2(1, 0.5f);
                    uiElement.pivot = new Vector2(1, 0.5f);
                    uiElement.anchoredPosition = Vector2.zero;
                    break;
            }
        }
        
        public static void EventBind()
        {

        }

        public static class Utils
        {
            public static Color StringToColor(string colorString)
            {
                // 尝试解析颜色字符串
                if (ColorUtility.TryParseHtmlString(colorString, out Color color))
                {
                    return color;
                }
                else
                {
                    // 如果解析失败，可以选择抛出异常或返回默认颜色
                    Debug.LogError("Invalid color string: " + colorString);
                    return Color.white; // 返回白色作为默认颜色
                }
            }
        }

        public static void Close(GameObject gameObject)
        {
            gameObject.SetActive(false);
        }
        public static void Close(string uiKey)
        {
            MainCanvas.UIObjDir.GetValueOrDefault(uiKey)?.SetActive(false);
        }
        public static bool TryShow(string uiKey)
        {
            if (MainCanvas.UIObjDir.GetValueOrDefault(uiKey) == null)
            {
                return false;
            }
            else
            {
                MainCanvas.UIObjDir.GetValueOrDefault(uiKey)?.SetActive(true);
                return true;
            }
        }

        public static GameObject GetGameObjectByKey(string key)
        {
            return MainCanvas.UIObjDir.GetValueOrDefault(key);
        }
    }
}
