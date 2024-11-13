using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnlimitedScrollUI;

namespace InventorySystem
{
    public class InventoryUIController : MonoBehaviour
    {
        /// <summary>
        /// cell显示回调
        /// </summary>
        public Action<int, GameObject> OnCellBevisable;
        /// <summary>
        /// cell消失回调
        /// </summary>
        public Action<GameObject> OnCellBeInvisable;
        public int InitUpdateCount = 30;
        /// <summary>
        /// 跳转
        /// </summary>
        /// <param name="index"></param>
        public void Jump(uint index)
        {
            unlimitedScroller.JumpTo(index, JumpToMethod.Center);
        }
        
        /// <summary>
        /// 刷新
        /// </summary>
        public void UpdateInventory()
        {
            unlimitedScroller?.Clear();
            StartCoroutine(Gen(InitUpdateCount,CellPrefab));
        }

        #region For Init

        void Start()
        {
            unlimitedScroller??= gameObject.GetComponentInChildren<IUnlimitedScroller>();
            UpdateInventory();
        }
        public GameObject CellPrefab;

        void OnDestroy()
        {
            OnCellBevisable = null;
            OnCellBeInvisable = null;
        }

        private IUnlimitedScroller unlimitedScroller;
        private IEnumerator Gen(int initCount , GameObject prefab)
        {
            yield return new WaitForEndOfFrame();
            Generate(initCount,prefab);
        }
        private void Generate(int count , GameObject prefab)
        {
            unlimitedScroller.Generate(prefab, count, (index, iCell) =>
            {
                var regularCell = iCell as RegularCell;
                if (regularCell != null) regularCell.onGenerated?.Invoke(index);
            });
        }
        #endregion

        
    }
}
