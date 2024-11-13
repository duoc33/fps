using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnlimitedScrollUI;

namespace InventorySystem
{
    public class InventoryCellLogic : MonoBehaviour
    {
        private InventoryUIController Inventory;

        private RegularCell regularCell;

        private void Awake()
        {
            if (Inventory == null) Inventory = GetComponentInParent<InventoryUIController>();
            if (regularCell == null) regularCell = GetComponent<RegularCell>();
        }
        private void OnEnable()
        {
            regularCell.onGenerated.AddListener(OnGen);
            regularCell.onBecomeInvisible.AddListener(OnInvisable);
        }
        private void OnDisable()
        {
            regularCell.onGenerated.RemoveListener(OnGen);
            regularCell.onBecomeInvisible.RemoveListener(OnInvisable);
        }
        private void OnGen(int index) => Inventory?.OnCellBevisable?.Invoke(index, gameObject);
        private void OnInvisable(ScrollerPanelSide scrollerPanelSide) => Inventory?.OnCellBeInvisable?.Invoke(gameObject);
    }
}
