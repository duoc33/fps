using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;
using ScriptableObjectBase;
using FPS;
using Foundation;

namespace Managers
{
    public class ParseManager
    {
        private static string apiURL = "https://data.dev.agione.ai/api/v1/data/mongo/";
        private static async UniTask<string> getGameDetailByID(string id)
        {
            // 处理逻辑
            Debug.Log("调用了 getGameDetailByID，传入的ID是: " + id);

            // Set up API URL for the "read" endpoint
            string apiUrl = apiURL + "read";

            // 创建外层字典，键为 "collection"，值为一个字符串
            Dictionary<string, object> outerDictionary = new Dictionary<string, object>();

            // 你可以将 collection 和 id 赋值为字符串变量
            string collection = "test_collection";

            // 创建内层字典，键为 "id"，值为一个字符串
            Dictionary<string, string> innerDictionary = new Dictionary<string, string>{{ "id", id }};

            // 将 "collection" 和 "query" 添加到外层字典
            outerDictionary.Add("collection", collection);
            outerDictionary.Add("query", innerDictionary);

            // 将字典转换为 JSON 字符串
            string jsonBody = JsonConvert.SerializeObject(outerDictionary);
            Debug.Log("JSON Body: " + jsonBody); // Log the request body for inspection

            // Create UnityWebRequest for POST request
            //UnityWebRequest www = UnityWebRequest.Post(apiUrl, jsonBody);
            UnityWebRequest www = new UnityWebRequest(apiUrl, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();

            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("api-key", "mc-n_KDyqNaDLJs6NOeoO-Y42rjtN_sJbV1kn7OqdBvIu0fMNxqrp1_P4f05Ns1a1EJ");
            await www.SendWebRequest();
            if (www.isDone == false || www.error != null)
            {
                Debug.LogError("Request = " + www.error);
                return "https://herepose-upload-local.s3.us-west-2.amazonaws.com/assets-create/2024/09/26/dc239c47-de0.json";
            }

            var result = www.downloadHandler.text;
            Debug.Log("api result:" + result);

            // 解析JSON字符串为JObject
            JObject jsonObject = JObject.Parse(result);

            //// 移除_id和id键值对
            //jsonObject.Remove("_id");
            //jsonObject.Remove("id");

            // 获取 game_json 字段
            var gameJson = jsonObject["game_json"];

            // 重新序列化为JSON字符串
            //string modifiedJsonString = jsonObject.ToString();
            string modifiedJsonString = gameJson.ToString();
            Debug.Log("modifiedJsonString:" + modifiedJsonString);

            return modifiedJsonString;
        }

        /// <summary>
        /// 游戏加载入口
        /// </summary>
        /// <param name="url"></param>
        public static async UniTask RunFromID(string id, Action callback = null)
        {
            var agi_game_json_str = await getGameDetailByID(id);
            await ParseExecutors(agi_game_json_str);
            ManagerManager.ManagerLifeLoopStart();
            callback?.Invoke();
        }

        /// <summary>
        /// 游戏加载入口
        /// </summary>
        /// <param name="url"></param>
        public static async UniTask RunFromUrl(string url)
        {
            ParseUtils.Print(url);
            var agi_game_json_str = await DLUtils.DownloadJson(url);
            ParseUtils.ProcessEnvironmentValues(ref agi_game_json_str);
            await ParseExecutors(agi_game_json_str);
            ManagerManager.ManagerLifeLoopStart();
        }

        public static async void RunFromString(string content)
        {
            ParseUtils.Print(content);
            await ParseExecutors(content);
            ManagerManager.ManagerLifeLoopStart();
        }

#pragma warning disable CS1998
        private static async UniTask ParseExecutors(string txt)
#pragma warning restore CS1998
        {
            // SOBase.InitLocal();
            SOBase.InitServer();

            SOBase Config = Serializer.Deserialize<SOBase>(txt);

            await Config.Download();

            Config.StartMixComponents();
        }

    }

}