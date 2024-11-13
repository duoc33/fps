using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;


namespace Managers.Runtime
{

    public static class EnvironmentUtils
    {
        private static IEnvironment env;
        
        public static IEnvironment Env
        {
            get
            {
                if (env == null)
                {
                    env = new WebGLEnv();
                }

                return env;
            }
        }
        
    }
    
    public class WebGLEnv : IEnvironment
    {
        Dictionary<string, string> ParseQueryString(string url)
        {

            // 查找查询字符串的开始
            int questionMarkIndex = url.IndexOf("?");
            if (questionMarkIndex == -1)
                return Dict; // 如果没有查询字符串，返回空字典

            // 提取查询字符串
            string queryString = url.Substring(questionMarkIndex + 1);
        
            // 分割每个参数
            string[] paramPairs = queryString.Split('&');
            foreach (string param in paramPairs)
            {
                // 分割键和值
                string[] keyValue = param.Split('=');
                if (keyValue.Length == 2)
                {
                    string key = keyValue[0];
                    string value = keyValue[1];
                
                    // 解码 URL 编码的值（如 %20 -> 空格）
                    key = WWW.UnEscapeURL(key);
                    value = WWW.UnEscapeURL(value);

                    Dict[key] = value;
                }
            }

            return Dict;
        }



        public bool CanEnter()
        {
            if (Dict.TryGetValue("id", out var value))
            {
                ParseManager.RunFromID(value).Forget();
                return true;
            }

            return false;
        }

        public override void Initialize()
        {
            string url = Application.absoluteURL;
            // string url = "https://127.0.0.1:7890/game?id=6655443302";
            Debug.Log($"Application.absoluteURL: {url}");
            Dictionary<string, string> queryParams = ParseQueryString(url);
        
            foreach (var param in queryParams)
            {
                Debug.Log("Key: " + param.Key + ", Value: " + param.Value);
            }
        }
    }

    public abstract class IEnvironment
    {
        public abstract void Initialize();


        public Dictionary<string, string> Dict = new();
    }
}