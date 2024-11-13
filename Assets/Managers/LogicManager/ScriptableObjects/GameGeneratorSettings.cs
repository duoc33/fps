using UnityEngine;

namespace Managers.ScriptableObjects
{
    /// <summary>
    /// 设置游戏 Generator
    /// </summary>
    [CreateAssetMenu(fileName = "GameGeneratorSettings", menuName = "Settings/Game Generator Settings")]
    public sealed class GameGeneratorSettings : ScriptableObject
    {
        private static GameGeneratorSettings _instance;

        public static GameGeneratorSettings instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<GameGeneratorSettings>("SO/GameGeneratorSettings");
                }
                return _instance;
            }
        }
        [Header("API Settings")]
        public string API_Key = null;
        public string API_Secret = null;
        
        [Header("UI Settings")]
        public Color BG_Color = Color.yellow;
        public Sprite BG_Image = null;
        
        // 保存更改的设置
        public void Save()
        {
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }
    }
}