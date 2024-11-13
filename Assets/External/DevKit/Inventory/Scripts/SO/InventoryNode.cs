using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnlimitedScrollUI;

namespace InventorySystem
{
    public class InventoryNode : ScriptableObject
    {
        public AnchorType anchorType = AnchorType.CenterMiddle;
        public PivotType pivotType = PivotType.CenterMiddle;
        public Vector2 position = Vector2.zero;
        public Vector2 size = Vector2.zero;


        public string TitleText;
        public Color TitleColor = Color.black;
        public ScrollViewType scrollViewType = ScrollViewType.Grid;
        public Vector2 cellSize = new Vector2(100, 100);
        [Header("Hor 和 Ver 模式只取值x")]
        public Vector2Int cellSpacing = new Vector2Int(15, 15);

        /// <summary>
        /// Grid有效
        /// </summary>
        [Header("Grid 模式有效")]
        public int CellPerRow = 6;
        public int PaddingLeft = 15;
        public int PaddingRight = 15;
        public int PaddingTop = 15;
        public int PaddingBottom = 15;

        public GameObject CellTemplate;
        public GameObject InventoryPrefab;

        public InventoryUIController Get(Transform parent)
        {
            GameObject template = Instantiate(InventoryPrefab);
            template.name = "Inventory";
            template.transform.Find("TitleBg/Title").GetComponent<TextMeshProUGUI>().text = TitleText;


            Transform Content = template.transform.Find("ScrollView/Viewport/Content");
            // Content.SetAnchor(AnchorType.FullStrentch);
            Content.gameObject.TryGetComponent(out GridLayoutGroup component);
            if (component != null) DestroyImmediate(component);
            for (int i = 0; i < Content.childCount; i++)
            {
                DestroyImmediate(Content.GetChild(i).gameObject);
            }

            ScrollRect scrollRect = template.transform.Find("ScrollView").GetComponent<ScrollRect>();
            InitGroup(scrollRect, Content);

            Transform cell = InitCell(template.transform);


            InventoryUIController inventory = InitInventory(parent,cell,template);

            return inventory;
        }
        
        public void ApplyCell(Transform cell , Sprite bgSprite , Sprite ItemSprite,string text)
        {
            cell.GetComponent<Image>().sprite = bgSprite;
            cell.Find("icon").GetComponent<Image>().sprite = ItemSprite;
            cell.Find("data").GetComponent<TextMeshProUGUI>().text = text;
        }

        private InventoryUIController InitInventory(Transform parent , Transform cell , GameObject template)
        {
            InventoryUIController inventory = template.AddComponent<InventoryUIController>();
            inventory.CellPrefab = cell.gameObject;
            inventory.transform.SetParent(parent);

            RectTransform rect = inventory.GetComponent<RectTransform>();
            SetAnchor(rect, anchorType);
            SetPivot(rect, pivotType);
            rect.sizeDelta = size;
            // rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            // rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.y);
            
            rect.anchoredPosition = position;
            rect.localScale = Vector3.one;

            return inventory;
        }

        private Transform InitCell(Transform template)
        {
            Transform cell = Instantiate(CellTemplate, template.transform.Find("CellCache")).transform;
            cell.name = "Cell";
            cell.GetComponent<RectTransform>().sizeDelta = cellSize;
            // cell.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, cellSize.x);
            // cell.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, cellSize.y);

            CanvasGroup canvasGroup = cell.gameObject.AddComponent<CanvasGroup>();
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.ignoreParentGroups = false;

            RegularCell regularCell = cell.gameObject.AddComponent<RegularCell>();
            regularCell.animationType = AnimationType.FadeAndScale;
            regularCell.animInterval = 0.6f;
            regularCell.fadeFrom = 0.2f;
            regularCell.scaleFrom = 0.4f;

            cell.gameObject.AddComponent<InventoryCellLogic>();

            return cell;
        }


        /// <summary>
        /// 设置锚点
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="anchorType"></param>
        private void SetAnchor(RectTransform rect, AnchorType anchorType)
        {
            int anchorIndex = (int)anchorType;

            // 处理前9个固定点的情况
            if (anchorIndex <= 8)
            {
                // 定义锚点的坐标系范围
                Vector2[] anchorPoints = new Vector2[]
                {
                    new Vector2(0f, 0f),  // LeftBottom
                    new Vector2(0.5f, 0f), // CenterBottom
                    new Vector2(1f, 0f),  // RightBottom
                    new Vector2(1f, 0.5f), // RightMiddle
                    new Vector2(1f, 1f),  // RightTop
                    new Vector2(0.5f, 1f), // CenterTop
                    new Vector2(0f, 1f),  // LeftTop
                    new Vector2(0f, 0.5f), // LeftMiddle
                    new Vector2(0.5f, 0.5f) // CenterMiddle
                };

                // 根据索引设置锚点
                rect.anchorMin = anchorPoints[anchorIndex];
                rect.anchorMax = anchorPoints[anchorIndex];
            }
            // 处理Stretch（拉伸）情况
            else
            {
                switch (anchorType)
                {
                    case AnchorType.LeftStrentch:
                        rect.anchorMin = new Vector2(0f, 0f);
                        rect.anchorMax = new Vector2(0f, 1f);
                        break;
                    case AnchorType.CenterStrentch:
                        rect.anchorMin = new Vector2(0.5f, 0f);
                        rect.anchorMax = new Vector2(0.5f, 1f);
                        break;
                    case AnchorType.RightStrentch:
                        rect.anchorMin = new Vector2(1f, 0f);
                        rect.anchorMax = new Vector2(1f, 1f);
                        break;
                    case AnchorType.TopStrentch:
                        rect.anchorMin = new Vector2(0f, 1f);
                        rect.anchorMax = new Vector2(1f, 1f);
                        break;
                    case AnchorType.MiddleStrentch:
                        rect.anchorMin = new Vector2(0f, 0.5f);
                        rect.anchorMax = new Vector2(1f, 0.5f);
                        break;
                    case AnchorType.BottomStrentch:
                        rect.anchorMin = new Vector2(0f, 0f);
                        rect.anchorMax = new Vector2(1f, 0f);
                        break;
                    case AnchorType.FullStrentch:
                        rect.anchorMin = new Vector2(0f, 0f);
                        rect.anchorMax = new Vector2(1f, 1f);
                        break;
                }
            }
        }

        /// <summary>
        /// 设置中心点
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="pivotType"></param>
        private void SetPivot(RectTransform rect, PivotType pivotType)
        {
            int pivotIndex = (int)pivotType;

            if (pivotIndex <= 8)
            {
                Vector2[] pivotPoints = new Vector2[]
                {
                    new Vector2(0f, 0f),  // LeftBottom
                    new Vector2(0.5f, 0f), // CenterBottom
                    new Vector2(1f, 0f),  // RightBottom
                    new Vector2(1f, 0.5f), // RightMiddle
                    new Vector2(1f, 1f),  // RightTop
                    new Vector2(0.5f, 1f), // CenterTop
                    new Vector2(0f, 1f),  // LeftTop
                    new Vector2(0f, 0.5f), // LeftMiddle
                    new Vector2(0.5f, 0.5f) // CenterMiddle
                };
                rect.pivot = pivotPoints[pivotIndex];
            }
        }


        private void InitGroup(ScrollRect scrollRect, Transform Content)
        {
            switch (scrollViewType)
            {
                case ScrollViewType.Grid:
                    scrollRect.horizontal = true;
                    scrollRect.vertical = true;
                    GridUnlimitedScroller grid = Content.gameObject.AddComponent<GridUnlimitedScroller>();
                    ProcessGrid(grid, scrollRect);
                    break;
                case ScrollViewType.Ver:
                    scrollRect.horizontal = false;
                    scrollRect.vertical = true;
                    VerticalUnlimitedScroller ver = Content.gameObject.AddComponent<VerticalUnlimitedScroller>();

                    ProcessVer(ver, scrollRect);
                    break;
                case ScrollViewType.Hor:
                    scrollRect.horizontal = true;
                    scrollRect.vertical = false;
                    HorizontalUnlimitedScroller hor = Content.gameObject.AddComponent<HorizontalUnlimitedScroller>();
                    ProcessHor(hor, scrollRect);
                    break;
            }
        }
        private void ProcessGrid(GridUnlimitedScroller grid, ScrollRect scrollRect)
        {
            grid.padding.left = PaddingLeft;
            grid.padding.right = PaddingRight;
            grid.padding.top = PaddingTop;
            grid.padding.bottom = PaddingBottom;
            grid.cellSize = cellSize;
            grid.spacing = cellSpacing;
            grid.cellPerRow = CellPerRow;
            grid.scrollRect = scrollRect;
        }
        private void ProcessHor(HorizontalUnlimitedScroller hor, ScrollRect scrollRect)
        {
            hor.childControlWidth = true;
            hor.childControlHeight = true;

            hor.padding.left = PaddingLeft;
            hor.padding.right = PaddingRight;
            hor.padding.top = PaddingTop;
            hor.padding.bottom = PaddingBottom;
            hor.spacing = cellSpacing.x;

            hor.scrollRect = scrollRect;
        }
        private void ProcessVer(VerticalUnlimitedScroller ver, ScrollRect scrollRect)
        {
            ver.childControlWidth = true;
            ver.childControlHeight = true;

            ver.padding.left = PaddingLeft;
            ver.padding.right = PaddingRight;
            ver.padding.top = PaddingTop;
            ver.padding.bottom = PaddingBottom;
            ver.spacing = cellSpacing.x;
            ver.scrollRect = scrollRect;
        }

        public enum AnchorType
        {
            LeftBottom = 0,
            CenterBottom = 1,
            RightBottom = 2,
            RightMiddle = 3,
            RightTop = 4,
            CenterTop = 5,
            LeftTop = 6,
            LeftMiddle = 7,
            CenterMiddle = 8,

            LeftStrentch = 9,
            CenterStrentch = 10,
            RightStrentch = 11,
            TopStrentch = 12,
            MiddleStrentch = 13,
            BottomStrentch = 14,
            FullStrentch = 15,
        }
        public enum PivotType
        {
            LeftBottom = 0,
            CenterBottom = 1,
            RightBottom = 2,
            RightMiddle = 3,
            RightTop = 4,
            CenterTop = 5,
            LeftTop = 6,
            LeftMiddle = 7,
            CenterMiddle = 8,
        }
        public enum ScrollViewType
        {
            Grid,
            Ver,
            Hor
        }
    }

}
