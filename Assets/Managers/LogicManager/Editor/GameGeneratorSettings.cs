using Managers.ScriptableObjects;
using UnityEditor;
using UnityEngine;

namespace Managers.EditorManager
{    
    public sealed class GameGeneratorSettingsProvider : SettingsProvider
    {
        // 记录用户选择的是背景颜色还是背景图片
        private bool useBackgroundColor = true;  // 默认使用颜色
        
        public GameGeneratorSettingsProvider() : base("Project/Game Generator", SettingsScope.Project) { }

        public override void OnGUI(string search)
        {
            var settings = GameGeneratorSettings.instance;
            var api_Key = settings.API_Key;
            var api_Secret = settings.API_Secret;
            var bg_Color = settings.BG_Color;
            var bg_Image = settings.BG_Image;

            EditorGUI.BeginChangeCheck();
            api_Key = EditorGUILayout.TextField("API Key", api_Key);
            api_Secret = EditorGUILayout.TextField("API Secret", api_Secret);
            
            // 使用 Toggle 或 Popup 选择是颜色还是图片
            useBackgroundColor = EditorGUILayout.Toggle("Use Background Color", useBackgroundColor);

            // 如果选择使用背景颜色
            if (useBackgroundColor)
            {
                bg_Color = EditorGUILayout.ColorField("BackGroundColor", bg_Color);
                bg_Image = null;  // 如果使用颜色，背景图片置为 null
            }
            else
            {
                // 如果选择使用背景图片
                bg_Image = (Sprite)EditorGUILayout.ObjectField("BackGroundImage", bg_Image, typeof(Sprite), false);
                // bg_Color = Color.clear;  // 如果使用图片，背景颜色置为透明
            }
            
            if (EditorGUI.EndChangeCheck())
            {
                settings.API_Key = api_Key;
                settings.API_Secret = api_Secret;
                settings.BG_Color = bg_Color;
                settings.BG_Image = bg_Image;
                settings.Save();
            }

            EditorGUILayout.Space(15);
            EditorGUILayout.LabelField("WebGL Building Settings");
            EditorGUILayout.LinkButton("https://hfbc4zhv5y.larksuite.com/wiki/TJ8Cwkhj5iatupkBlVnuiVq2sfc?open_in_browser=true");
        }

        [SettingsProvider]
        public static SettingsProvider CreateCustomSettingsProvider() => new GameGeneratorSettingsProvider();
    }
}
 