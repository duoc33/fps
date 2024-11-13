using System;
using System.Collections.Generic;


namespace InventorySystem
{
    public class Inventory<T> where T : class
    {
        public int Count => ItemsIndex.Count;
        private Dictionary<string, T> ItemsRecord = new Dictionary<string, T>();
        private List<string> ItemsIndex = new List<string>();

        public void Clear()
        {
            ItemsIndex?.Clear();
            ItemsRecord?.Clear();
        }
        public T GetItemByIndex(int index)
        {
            string key = ItemsIndex[index];
            if (ItemsRecord.ContainsKey(key))
            {
                T val = ItemsRecord[key];
                return val;
            }
            return null;
        }
        public T GetItemByName(string itemName)
        {
            if (ItemsRecord.ContainsKey(itemName))
            {
                T val = ItemsRecord[itemName];
                return val;
            }
            return null;
        }
        public int GetItemIndex(string itemName)=>ItemsIndex.FindIndex(0, str => str.Equals(itemName));
        public string GetItemKeyByIndex(int index)
        {
            if(index >= Count) return null;
            return ItemsIndex[index];
        }
        
        public void OperateItem(string itemName , Action<T> handler)
        {
            if (ItemsRecord.ContainsKey(itemName))
            {
                T val = ItemsRecord[itemName];
                if(val!=null)
                {
                    handler?.Invoke(val);
                }
            }
        }
        public void Add(string ItemName , T val)
        {
            if (ItemsRecord.ContainsKey(ItemName)) return;
            ItemsIndex.Add(ItemName);
            ItemsRecord[ItemName] = val;
        }
        public void Remove(string ItemName)
        {
            if (ItemsRecord.ContainsKey(ItemName))
            {
                ItemsIndex.Remove(ItemName);
                ItemsRecord.Remove(ItemName);
            }
        }
    }
}



