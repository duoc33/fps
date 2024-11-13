using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using System.Text.RegularExpressions;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
#if FLUTTER_UNITY
using FlutterUnityIntegration;
#endif
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif

public class PHUtils
{

    public static JsonSerializerSettings jsSetting = null;

    public static void WriteTxt(string path, string content, bool editorPrint = false)
    {
        if (editorPrint) Debug.LogFormat("Write to {0}:{1}", path, content);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        File.WriteAllText(path, content);
    }
    public static void EditorWriteTxt(string path, string content, bool editorPrint = false)
    {
#if UNITY_EDITOR
        WriteTxt(path, content, editorPrint);
#endif
    }
    public static void WriteObjectTxt(string path, object o, bool editorPrint = false)
    {
        var content = ConvertObject2String(o);
        WriteTxt(path, content, editorPrint);
    }
    public static void EditorWriteObjectTxt(string path, object o, bool editorPrint=false)
    {
#if UNITY_EDITOR
        WriteObjectTxt(path, o, editorPrint);
#endif
    }
    public static void WriteObjectListTxt<T>(string path, IEnumerable<T> o, bool editorPrint = false)
    {
        var content = ConvertObject2StringList(o);
        WriteTxt(path, content, editorPrint);
    }
    public static void EditorWriteObjectTxt<T>(string path, IEnumerable<T> o, bool editorPrint = false)
    {
#if UNITY_EDITOR
        WriteObjectListTxt(path, o);
#endif
    }

    public static T ConvertStringToObject<T>(string json) where T : class
    {
        if (String.IsNullOrEmpty(json)) return null;
        T obj = JsonConvert.DeserializeObject<T>(json);
        return obj;
    }
    
    public static T ConvertObjectToObject<T>(object o, JsonSerializerSettings jsonSerializerSettings = null) where T : class
    {
        string obj_str = JsonConvert.SerializeObject(o, jsonSerializerSettings);
        var obj = ConvertStringToObject<T>(obj_str);
        return obj;
    }
    
    public static List<T> ConvertStringToObjectList<T>(string json) where T : class
    {
        if (String.IsNullOrEmpty(json)) return null;
        if (jsSetting == null)
        {
            jsSetting = new JsonSerializerSettings();
            jsSetting.NullValueHandling = NullValueHandling.Ignore;
        }
        List<T> descJsonStu = JsonConvert.DeserializeObject<List<T>>(json, jsSetting);//反序列化
        return descJsonStu;
    }

    public static void ShowEdidtorDialogue(string title, string hint)
    {
        #if UNITY_EDITOR
        EditorUtility.DisplayDialog(title, hint, "OK", "Cancel");
        #endif
    }

    public static bool TryingToAdd<T>(List<T> l, T e)
    {
        bool isValid = false;

        if (e != null && !l.Contains(e))
        {
            l.Add(e);
            isValid = true;
        }
        return isValid;
    }



    public static T[] ArrayMove<T>(T[] array, int fromIndex, int toIndex)
    {
        if (
        fromIndex > array.Length - 1 ||
        toIndex > array.Length - 1 ||
        fromIndex == toIndex
        ) return array;

        T[] tempArray = new T[array.Length];
        if (fromIndex > toIndex)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (i == toIndex)
                {
                    tempArray[i] = array[fromIndex];
                }
                else
                {
                    if (i > fromIndex || i < toIndex)
                    {
                        tempArray[i] = array[i];
                    }
                    else
                    {
                        tempArray[i] = array[i - 1];
                    }
                }
            }
        }
        else if (fromIndex < toIndex)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (i == toIndex)
                {
                    tempArray[i] = array[fromIndex];
                }
                else
                {
                    if (i < fromIndex || i > toIndex)
                    {
                        tempArray[i] = array[i];
                    }
                    else
                    {
                        tempArray[i] = array[i + 1];
                    }
                }
            }
        }
        array = tempArray;

        return array;
    }

    public static T TryAddComponent<T>(GameObject target) where T: Component
    {
        var type = typeof(T);
        var comp = target.GetComponent(type);
        if (comp == null) comp = target.AddComponent(type);
        return comp as T;
    }

    public static void TryDestroyComponent<T>(GameObject target, bool im=false) where T : Component
    {
        var type = typeof(T);
        if (target == null)
        {
            return;
        }
        var comp = target.GetComponent(type);

        if (comp != null)
        {
#if UNITY_EDITOR
            Undo.DestroyObjectImmediate(comp);
#else
            if(im)UnityEngine.Object.DestroyImmediate(comp);
            else UnityEngine.Object.Destroy(comp);
#endif
        }


    }

    public static Dictionary<T, Q> ModifyDicKey<T,Q>(Dictionary<T, Q> dic, T from, T to)
    {
        if (from.Equals(to)) return dic;
        dic[to] = dic[from];
        dic.Remove(from);
        return dic;
    }

    public static GameObject TryFindGameObject(string gameObjectName)
    {
        GameObject Holder = GameObject.Find(gameObjectName);
        if (Holder != null) UnityEngine.Object.DestroyImmediate(Holder);
        Holder = new GameObject(gameObjectName);
        return Holder;
    }


    public static List<T> ListMove<T>(List<T> l, int fromIndex, int toIndex)
    {
        var array = l.ToArray();
        array = ArrayMove(array, fromIndex, toIndex);
        return array.ToList();
        
    }
    public static int randn = 0;
    public static T GetRandom<T>(IEnumerable<T> list)
    {

        var c = list.Count();
        if (c == 0) return default;
        else
        {
            int seed = DateTime.Now.Millisecond + randn;
            randn += 100;
            var index = new System.Random(seed).Next(0, c);
            return list.ElementAt(index);
        }
        
    }
    public static uint GetRandomUint()
    {
        return (uint)Mathf.FloorToInt(UnityEngine.Random.value * int.MaxValue);
    }

    public static int GetRandomInt(int min, int max)
    {
        return new System.Random().Next(min, max);
    }

    public static IEnumerable<T> PickSomeInRandomOrder<T>(IEnumerable<T> someTypes, int maxCount)
    {
        System.Random random = new System.Random(DateTime.Now.Millisecond);

        Dictionary<double, T> randomSortTable = new Dictionary<double, T>();

        foreach (T someType in someTypes)
            randomSortTable[random.NextDouble()] = someType;

        return randomSortTable.OrderBy(KVP => KVP.Key).Take(maxCount).Select(KVP => KVP.Value);
    }

    public static string GenerateTimestampName(string prefix, string ext)
    {
        return string.Format("{0}/{1}-{2}{3}",
            Application.persistentDataPath,
            prefix,
            DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss"),
            ext);
    }

    public static string GenerateTimeStampName()
    {
        return DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
    }
    public static string ConvertObject2StringList(object o, bool newLine=true)
    {
        var type = newLine?Formatting.Indented:Formatting.None;
        return o == null ? "null" : JArray.FromObject(o).ToString(type);
        //return JsonConvert.SerializeObject(o);
    }

    public static string ConvertObject2String(object o, bool newLine=true)
    {
        //return JsonConvert.SerializeObject(o);
        var type = newLine?Formatting.Indented:Formatting.None;
        return o==null?"null":JObject.FromObject(o).ToString(type);
    }

    public static void CreateFile(string path, byte[] bytes)
    {
        Stream sw;

        FileInfo file = new FileInfo(path);
        if (file.Exists)
        {
            file.Delete();
        }
        sw = file.Create();
        sw.Write(bytes, 0, bytes.Length);
        sw.Close();
        sw.Dispose();
    }

    public static List<string> SplitString(string s, string pattern)
    {
        string[] sArray = Regex.Split(s, pattern, RegexOptions.IgnoreCase);
        return sArray.ToList();
    }
    
    public static string SplitUrl(string url)
    {
        if (url.Contains("##==##"))
        {
            string[] sArray = Regex.Split(url, "##==##", RegexOptions.IgnoreCase);
            
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                url = sArray[1];
            }
            else if(Application.platform == RuntimePlatform.WebGLPlayer)
            {
                url = sArray[2];
            }
            else
            {
                url = sArray[0];
            }
        }
        return url;
    }
    public static string SplitModelName(string modelName)
    {
        //Debug.Log("Split " + modelName);
        if (modelName.Contains("#"))
        {
            string[] sArray = Regex.Split(modelName, "#", RegexOptions.IgnoreCase);
            modelName = sArray[0];
        }
        return modelName;
    }
    public static float[] ConvertByteToFloat(byte[] array)
    {
        float[] floatArr = new float[array.Length / 4];
        for (int i = 0; i < floatArr.Length; i++)
        {
            floatArr[i] = BitConverter.ToSingle(array, i * 4) / 0x80000000;
        }
        return floatArr;
    }
    public static T ToEnum<T>(string str) where T:Enum
    {
        return (T)Enum.Parse(typeof(T), str);
    }
    public static int TryGetEnumInt<T>(string str, int defaultValue) where T:Enum
    {
        // int result = default;
        try
        {
            return (int)Enum.Parse(typeof(T), str);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            
        }
        return defaultValue;
    }

    public static void PrintObject(object o, object hint)
    {
        var json = ConvertObject2String(o);
        Debug.Log(hint + "==>" + json);
    }
    public static void PrintObjectList(object o, object hint)
    {
        var json = ConvertObject2StringList(o);
        Debug.Log(hint + "==>" + json);
    }

    public static void EditorPrint(object message)
    {
#if UNITY_EDITOR
        //Debug.Log(message);
#endif
    }

    public static void EditorPrintObject(object o, object hint)
    {
#if UNITY_EDITOR
        PrintObject(o, hint);
#endif
    }

    public static void EditorPrintObjectList(object o, object hint)
    {
#if UNITY_EDITOR
        PrintObjectList(o, hint);
#endif
    }

    public static void EditorPrintObjectFast(object o)
    {
        EditorPrintObject(o, o);
    }
    public static void EditorPrintObjectListFast(object o)
    {
        EditorPrintObjectList(o, o);
    }

    /// <summary>
    /// 字符串转Vector3
    /// </summary>
    /// <param name="p_sVec2">需要转换的字符串</param>
    /// <returns></returns>
    public static Vector3 GetVec3ByString(string p_sVec2)
    {
        if (p_sVec2.Length <= 0)
            return Vector3.zero;

        string[] tmp_sValues = p_sVec2.Trim(' ').Replace("(","").Replace(")", "").Split(',');
        if (tmp_sValues != null && tmp_sValues.Length == 3)
        {
            float tmp_fX = float.Parse(tmp_sValues[0]);
            float tmp_fY = float.Parse(tmp_sValues[1]);
            float tmp_fZ = float.Parse(tmp_sValues[2]);

            return new Vector3(tmp_fX, tmp_fY, tmp_fZ);
        }
        return Vector3.zero;
    }
    public static Vector2 GetVec2ByString(string p_sVec2)
    {
        if (p_sVec2.Length <= 0)
            return Vector2.zero;

        string[] tmp_sValues = p_sVec2.Trim(' ').Replace("(","").Replace(")", "").Split(',');
        if (tmp_sValues != null && tmp_sValues.Length == 2)
        {
            float tmp_fX = float.Parse(tmp_sValues[0]);
            float tmp_fY = float.Parse(tmp_sValues[1]);

            return new Vector2(tmp_fX, tmp_fY);
        }
        return Vector2.zero;
    }
    /// <summary>
    /// 字符串转换Quaternion
    /// </summary>
    /// <param name="p_sVec3">需要转换的字符串</param>
    /// <returns></returns>
    public static Quaternion GetQuaByString(string p_sVec3)
    {
        if (p_sVec3.Length <= 0)
            return Quaternion.identity;

        string[] tmp_sValues = p_sVec3.Trim(' ').Replace("(", "").Replace(")", "").Split(',');
        if (tmp_sValues != null && tmp_sValues.Length == 4)
        {
            float tmp_fX = float.Parse(tmp_sValues[0]);
            float tmp_fY = float.Parse(tmp_sValues[1]);
            float tmp_fZ = float.Parse(tmp_sValues[2]);
            float tmp_fH = float.Parse(tmp_sValues[3]);

            return new Quaternion(tmp_fX, tmp_fY, tmp_fZ, tmp_fH);
        }
        return Quaternion.identity;
    }

    public static Texture2D RenderTexture2Texture2D(RenderTexture renderTexture)
    {
        int width = renderTexture.width;
        int height = renderTexture.height;
        Texture2D texture2D = new Texture2D(width, height, TextureFormat.ARGB32, false);
        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        texture2D.Apply();
        return texture2D;
    }
    public static void SaveRenderTextureToPNG(RenderTexture png, int index, string contents = "tempPngs")
    {
        Texture2D m_DrawTexture = new Texture2D(448, 448, TextureFormat.ARGB32, false);

        m_DrawTexture.filterMode = FilterMode.Point;
        RenderTexture.active = png;
        m_DrawTexture.ReadPixels(new Rect(0, 0, png.width, png.height), 0, 0);
        m_DrawTexture.Apply();

        byte[] bytes = m_DrawTexture.EncodeToPNG();
        if (!Directory.Exists(contents))
            Directory.CreateDirectory(contents);

        FileStream file = File.Open(contents + "/" + index.ToString() + ".png", FileMode.Create);
        BinaryWriter writer = new BinaryWriter(file);
        writer.Write(bytes);
        writer.Close();
        Debug.Log(file.Name);
        file.Close();
    }

    public static void SaveTexture2DToPNG(Texture2D png, int index, string contents = "tempPngs")
    {

        byte[] bytes = png.EncodeToPNG();
        if (!Directory.Exists(contents))
            Directory.CreateDirectory(contents);

        FileStream file = File.Open(contents + "/" + index.ToString() + ".png", FileMode.Create);
        BinaryWriter writer = new BinaryWriter(file);
        writer.Write(bytes);
        writer.Close();
        Debug.Log(file.Name);
        file.Close();

    }

    public static Vector3 GetPlaneIntersection(Camera camera)
    {
        Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        float delta = ray.origin.y - 0f;
        Vector3 dirNorm = ray.direction / ray.direction.y;
        Vector3 IntersectionPos = ray.origin - dirNorm * delta;
        return IntersectionPos;
    }
    public static float[] StringArrayToFloatArray(string[] ss)
    {
        return ss.Select(x => Convert.ToSingle(x)).ToArray();
    }
    public static int[] StringArrayToIntArray(string[] ss)
    {
        return ss.Select(x => Convert.ToInt32(x)).ToArray();
    }
    public static string UpperEveryWord(string str)
    {
        return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
    }
    public static string[] FloatArrayToStringArray(float[] ss)
    {
        return ss.Select(x => Convert.ToString(x)).ToArray();
    }
    public static string ThreeDTextDic2String(Dictionary<string,string> dic)
    {
        return JObject.FromObject(dic).ToString();
    }
    public static Dictionary<string, string> ThreeDTextString2Dic(string dic)
    {
        return ConvertStringToObject<Dictionary<string, string>>(dic);
    }

    public static int ToARGB(Color color)
    {
        Color32 c = (Color32)color;
        byte[] b = new byte[] { c.b, c.g, c.r, c.a };
        return System.BitConverter.ToInt32(b, 0);
    }

    public static string GenerateUUID()
    {
        return System.Guid.NewGuid().ToString();
    }


    public static List<T> GetAtPathEditor<T>(string folder) where T : UnityEngine.Object
    {
        List<T> result = new List<T>();
#if UNITY_EDITOR
        if (Directory.Exists(folder))
        {
            string[] fileEntries = Directory.GetFiles(folder);

            foreach (string fileName in fileEntries)
            {
                if (fileName.Contains(".asset.meta")) continue;
                T res = GetAssetAtPathEditor<T>(fileName);
                result.Add(res);
            }


        }
        else
        {
            Debug.LogWarning("File or directory not exist: " + folder);
        }
#endif
        return result;
    }


    public static T GetAssetAtPathEditor<T>(string filePath) where T : UnityEngine.Object
    {
        T res = null;
#if UNITY_EDITOR
        res = AssetDatabase.LoadAssetAtPath(filePath, typeof(T)) as T;
#endif
        return res;
    }


    public static List<T> LoadRPGAssets<T>(string folder, bool useResource = false) where T : UnityEngine.Object
    {
        List<T> result;
        if (useResource)
        {
            result = Resources.LoadAll<T>(folder).ToList();
        }
        else
        {
            result = GetAtPathEditor<T>(folder);
        }
        return result;
    }

    public static T LoadRPGAsset<T>(string filePath, bool useResource = false) where T : UnityEngine.Object
    {
        T res = null;
        if (useResource)
        {
            res = Resources.Load<T>(filePath);
        }
        else
        {
            res = GetAssetAtPathEditor<T>(filePath);
        }
        return res;

    }

#if UNITY_EDITOR
    public static T LoadRPGAssetEditor<T>(string path) where T:UnityEngine.Object
    {
        string fileName = @"Assets\ThirdParty\TPRPGBlink\Tools\RPGBuilder\ThirdPartyAssets\PONETI\Basic_RPG_Icons\Items\Resources/" + path;
        return GetAssetAtPathEditor<T>(fileName);
    }
#endif
    public static void SendMessageToFlutter(string message, bool editorPrint=false)
    {
        if (editorPrint) EditorPrint(message);
#if FLUTTER_UNITY
        UnityMessageManager.Instance.SendMessageToFlutter(message);
#endif
    }
    public static void SendMessageToFlutter(string head, object body, bool editorPrint = false)
    {
        string message = head +"=>" + body;
        if (editorPrint) EditorPrint(message);
        
        SendMessageToFlutter(message);
    }
    public static string ForSnapThumbnails(Camera captureCamera, int resWidth, int resHeight, string name = null)
    {
        string ScreenShotName(int width, int height)
        {
            // 输出路径和文件名（自带尺寸和日期）
            return string.Format("{0}/screen_{1}x{2}_{3}.png",
                Application.persistentDataPath,
                width, height,
                DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
        }


        // create an renderTexture to save the image data from camera
        RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
        captureCamera.targetTexture = rt;
        // create an texture2d to recieve the renderTexture data
        Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
        // render from camera
        captureCamera.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
        // disable renderTexture to avoid errors
        captureCamera.targetTexture = null;
        RenderTexture.active = null;
        UnityEngine.Object.DestroyImmediate(rt);
        // Save to png file
        byte[] bytes = screenShot.EncodeToPNG();
        string filename = name == null ? ScreenShotName(resWidth, resHeight) : name;
        File.WriteAllBytes(filename, bytes);

        //var prefix = Application.platform == RuntimePlatform.IPhonePlayer ? "file://" : "";
        Debug.Log(string.Format("Took screenshot to: {0}", filename));
        //return prefix + filename;


        return filename;
    }
    

    public static bool SaveRenderTextureToPNG(Texture inputTex, Shader outputShader, string filename)
    {
        RenderTexture temp = RenderTexture.GetTemporary(inputTex.width, inputTex.height, 0, RenderTextureFormat.ARGB32);
        Material mat = new Material(outputShader);
        Graphics.Blit(inputTex, temp, mat);
        bool ret = SaveRenderTextureToPNG(temp, filename);
        RenderTexture.ReleaseTemporary(temp);
        return ret;

    }

    //将RenderTexture保存成一张png图片
    public static bool SaveRenderTextureToPNG(RenderTexture rt, string filename)
    {
        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = rt;
        Texture2D png = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);
        png.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        byte[] bytes = png.EncodeToPNG();
        FileStream file = File.Open(filename, FileMode.Create);
        BinaryWriter writer = new BinaryWriter(file);
        writer.Write(bytes);
        file.Close();
        Texture2D.DestroyImmediate(png);
        png = null;
        RenderTexture.active = prev;
        
        return true;

    }


    public static GameObject[] MoveSceneObjectUp()
    {
        var scene = SceneManager.GetActiveScene();
        var objs = scene.GetRootGameObjects();
        foreach (var obj in objs)
        {
            UnityEngine.Object.DontDestroyOnLoad(obj);
        }
        return objs;
    }
    public static void MoveSceneObjectDown(Scene newScene, GameObject[] objs)
    {
        foreach (var obj in objs)
        {
            SceneManager.MoveGameObjectToScene(obj, newScene);
        }
    }
    public static void AdjustPosition(int maxAttempts)
    {
        Debug.Log("In AdjustPosition, less than " + maxAttempts);
        if (maxAttempts < 0) return;
        var spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint");
        if (spawnPoint != null)
        {
            Collider[] colliders = Physics.OverlapSphere(spawnPoint.transform.position, 2.0f);
            if (colliders.Length == 0 || colliders.Length == 1 && (colliders[0].gameObject.name == "MainPlatform" || colliders[0].gameObject.name == "NewPlatform"))
            {
                Debug.Log("This position is valid. " + spawnPoint.transform.position);
            }
            else
            {
                spawnPoint.transform.position = spawnPoint.transform.position + new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f));
                Debug.Log("Move SpawnPoint to " + spawnPoint.transform.position);
                AdjustPosition(maxAttempts - 1);
            }
        }
    }
    public static void AdjustPosition(Vector3 initPos, int maxAttempts)
    {
        Debug.Log("In AdjustPosition, less than " + maxAttempts);
        if (maxAttempts < 0) return;
        Collider[] colliders = Physics.OverlapSphere(initPos, 2.0f);
        if (colliders.Length == 0 || colliders.Length == 1 && (colliders[0].gameObject.name.Contains("Terrain")))
        {
            Debug.Log("This position is valid. " + initPos);
        }
        else
        {
            var newPos = initPos + new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f));
            Debug.Log("Move SpawnPoint to " + newPos);
            AdjustPosition(newPos, maxAttempts - 1);
        }
    }

    public static List<Vector3> AdjustTarget(GameObject target)
    {
        Debug.Log("Adust target " + target);
        List<Vector3> result = new List<Vector3>();
        int totalNum = 1;
        Transform parent = target.transform;
        Vector3 postion = parent.position;
        Quaternion rotation = parent.rotation;
        Vector3 scale = parent.localScale;
        parent.position = Vector3.zero;
        parent.rotation = Quaternion.Euler(Vector3.zero);
        parent.localScale = Vector3.one;

        var col = target.GetComponent<BoxCollider>();
        if (col != null) UnityEngine.Object.DestroyImmediate(col, true);

        Collider[] colliders = parent.GetComponentsInChildren<Collider>();
        foreach (Collider child in colliders)
        {
            if (!child.name.Contains("angle") && !child.name.Contains("Rotate"))
            {
                UnityEngine.Object.DestroyImmediate(child, true);
            }
        }
        Vector3 center = Vector3.zero;
        Renderer[] renders = parent.GetComponentsInChildren<Renderer>();
        foreach (Renderer child in renders)
        {
            if (!child.name.Contains("angle") && !child.name.Contains("Rotate"))
            {
                center += child.bounds.center;
                totalNum++;
            }

        }
        //print(center);
        center /= totalNum;
        //print(center);
        Bounds bounds = new Bounds(center, Vector3.zero);
        foreach (Renderer child in renders)
        {
            bounds.Encapsulate(child.bounds);
        }
        BoxCollider boxCollider = parent.gameObject.AddComponent<BoxCollider>();
        //print(bounds.center);
        //print(parent.position);
        boxCollider.center = bounds.center - parent.position;
        boxCollider.size = bounds.size;

        parent.position = postion;
        parent.rotation = rotation;
        parent.localScale = scale;

        result.Add(new Vector3(boxCollider.center.x, boxCollider.size.y / 2, boxCollider.center.z));

        result.Add(boxCollider.size);

        //print(result[0]);
        return result;
    }

    public static void ApplyDefaultShaderEnv(GameObject obj)
    {
        var render = obj.GetComponentsInChildren<Renderer>();
        foreach (var child in render)
        {
            foreach (var mat in child.materials)
            {
                mat.shader = Shader.Find("Unlit/Texture");
            }
        }
    }

    public static void ApplyDefaultShader(GameObject obj)
    {
        var render = obj.GetComponentsInChildren<Renderer>();
        foreach (var child in render)
        {
            foreach (var mat in child.materials)
            {
                // mat.shader =Shader.Find("Custom/MindCoordCharacterShader");
                mat.shader = Shader.Find("Unlit/Texture");
            }
        }
    }
    public static Vector3 AdjustPositionWithLocation(Vector3 location)
    {
        Collider[] colliders = Physics.OverlapSphere(location, 2.0f);
        bool condition = true;
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.layer == LayerMask.NameToLayer("Object") && !colliders[i].gameObject.name.Contains("SpawnPoint"))
            {
                condition = false;
                break;
            }
        }
        if (condition)
        {
            Debug.Log("This position is valid. " + location);
            return location;
        }
        else
        {
            location = location + new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f));
            Debug.Log("Move SpawnPoint to " + location);
            return AdjustPositionWithLocation(location);
        }
    }

    
    public static void PrintCurrentFunc(object func, string hint="")
    {
        Debug.Log("This is in func " + func+ ":" + hint);
    }
    
    public static void DisableCG(CanvasGroup cg)
    {
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }
    public static void EnableCG(CanvasGroup cg)
    {
        cg.alpha = 1;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }
    
    public static Texture2D ResizeTexture(Texture2D texture2D,int targetX,int targetY)
    {
        RenderTexture rt=new RenderTexture(targetX, targetY,24);
        RenderTexture.active = rt;
        Graphics.Blit(texture2D,rt);
        Texture2D result=new Texture2D(targetX,targetY);
        result.ReadPixels(new Rect(0,0,targetX,targetY),0,0);
        result.Apply();
        return result;
    }

    public static TDerived ForceConvert<TBase, TDerived>(TBase source)
        where TBase : class
        where TDerived : TBase, new()
    {
        TDerived destination = new TDerived();
        // var tmp = typeof(TBase).GetProperties();
        // foreach (PropertyInfo item in tmp)
        // {
        //     item.SetValue(destination, item.GetValue(source));
        // }

        // var tmp2 = typeof(TBase).GetFields();
        FieldInfo[] fields = typeof(TBase).GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (FieldInfo field in fields)
        {
            field.SetValue(destination, field.GetValue(source));
        }

        return destination;
    }

    public static void SetParentInEditor(GameObject parent, GameObject child)
    {
        #if UNITY_EDITOR && !DEBUG_RUNTIME
        // Apply the changes to PrefabB
        // PrefabUtility.ApplyAddedGameObject((GameObject)PrefabUtility.InstantiatePrefab(child),AssetDatabase.GetAssetPath(parent), InteractionMode.AutomatedAction);
// 创建Prefab的根GameObject
        GameObject instanceA = PrefabUtility.InstantiatePrefab(child) as GameObject;
        GameObject instanceB = PrefabUtility.InstantiatePrefab(parent) as GameObject;
        if(!instanceA||!instanceB) return;

        instanceA.transform.SetParent(instanceB.transform, false);
        instanceA.transform.localPosition=Vector3.zero;
        // 创建新的Prefab
        PrefabUtility.SaveAsPrefabAsset(instanceB, AssetDatabase.GetAssetPath(parent));
        UnityEngine.Object.DestroyImmediate(instanceA);
        UnityEngine.Object.DestroyImmediate(instanceB);
        AssetDatabase.Refresh();
        #endif
    }
    
    public static void SavePrefab(GameObject instanceA, string path)
    {
#if UNITY_EDITOR && !DEBUG_RUNTIME
        PrefabUtility.SaveAsPrefabAsset(instanceA, path);
        UnityEngine.Object.DestroyImmediate(instanceA);
        AssetDatabase.Refresh();
#endif
    } 

    public static Dictionary<string, string> GetQueryDicFromURL(string url)
    {
        var queryDict = new Dictionary<string, string>();

        // split ?
        var arr = url.Split('?');
        if (arr.Length < 2)
        {
            Debug.LogError("No query string ?");
            return queryDict;
        }
        var query = arr[1];
        // split &
        var queryArr = query.Split('&');
        if (queryArr.Length < 1)
        {
            Debug.LogError("No query string &");
            return queryDict;
        }
        foreach (var item in queryArr)
        {
            var kv = item.Split('=');
            if (kv.Length < 2) continue;
            queryDict.Add(kv[0], kv[1]);
        }

        return queryDict;
    }

//     public static void AutoAdjustSpriteCollider2DByBound(GameObject g)
//     {
//         Vector2 S = g.GetComponent<SpriteRenderer>().sprite.bounds.size;
//         g.GetComponent<BoxCollider2D>().size = S;
//         g.GetComponent<BoxCollider2D>().offset = new Vector2 (0, 0);
//     }
//     public static void AutoAdjustSpriteCollider2D(GameObject g, float yOffset=0.0f)
//     {

//         UniDo(async() =>
//         {
//             // await Task.Delay(200);
// // #if UNITY_EDITOR
// //             EditorUtility.SetDirty(g);
// //
// // #endif
//             await UniTask.Yield();
//             BoxCollider2D boxCollider = g.GetComponentInChildren<BoxCollider2D>();
//             if(boxCollider==null)return;
//             boxCollider.enabled = false;
//             var c2 = boxCollider.gameObject.AddComponent<BoxCollider2D>();
//             boxCollider.size = c2.size - new Vector2(0.1f, 0.1f);
//             boxCollider.offset = new Vector2(c2.offset.x, c2.offset.y + yOffset);
//             UnityEngine.Object.DestroyImmediate(c2,true);
//             boxCollider.enabled = true;
// //
// // #if UNITY_EDITOR
// //             AssetDatabase.SaveAssets();
// //
// // #endif


//         });


//     }
    public delegate void OnExecute();
    public static async UniTask UniDo(OnExecute t2)
    {
        await UniTask.SwitchToMainThread();
        t2();
        await UniTask.SwitchToTaskPool();
    }

    public static async void UniDoUniTask(Func<UniTask> t2)
    {
        await UniTask.SwitchToMainThread();
        await t2();
        await UniTask.SwitchToTaskPool();
    }
    

    public static void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (null == obj)
        {
            return;
        }

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (null == child)
            {
                continue;
            }

            SetLayerRecursively(child.gameObject, newLayer);
        }
    }

    public static void SetParent(GameObject parent, GameObject child)
    {
#if UNITY_EDITOR && !DEBUG_RUNTIME
        SetParentInEditor(parent, child);
#else
        child.transform.SetParent(parent.transform);
#endif
    }

    public static GameObject SetParentByInstantiate(GameObject parent, GameObject child)
    {
        GameObject copiedObject = null;
#if UNITY_EDITOR && !DEBUG_RUNTIME
        SetParentInEditor(parent, child);
        copiedObject = parent.transform.Find(child.name).gameObject;
#else
        copiedObject = UnityEngine.Object.Instantiate(child, parent.transform);
#endif
        return copiedObject;
    }
    
    public static Texture2D Sprite2Texture2D(Sprite sprite)
    {
        Texture2D texture = sprite.texture;

        Texture2D newTexture = new Texture2D(texture.width, texture.height);
        newTexture.SetPixels(texture.GetPixels());
        newTexture.Apply();
        newTexture.name = sprite.name;
        Debug.Log("Sprite2Texture2D"+sprite);
        return newTexture;
    }

    public static void SaveSpriteToPNG(Sprite sprite, string path)
    {
        Texture2D texture = Sprite2Texture2D(sprite);
        byte[] bytes = texture.EncodeToPNG();
        File.WriteAllBytes(path, bytes);
    }
    
    #if UNITY_EDITOR
    public static void SetACAnimation(AnimatorController ac, AnimationClip clip, string motionName)
    {
        foreach (var layer in ac.layers)
        {
            foreach (var state in layer.stateMachine.states)
            {
                if (state.state.name == motionName)
                {
                    state.state.motion = clip;
                }
            }
        }
    }
    #endif

    
    public static void SetDirty(Object ob)
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(ob);
#endif
    }
}

public static class MCExtensions
{
    public static IEnumerable<T> RemoveNull<T>(this IEnumerable<T> enumerable) where T : class
    {
        //If the generic type inherits from UnityEngine.Object, this needs to be performed to handle the == overload.
        if (typeof(UnityEngine.Object).IsAssignableFrom(typeof(T)))
        {
            enumerable = enumerable.Where(e => (e as UnityEngine.Object) != null);
        }
        else
        {
            enumerable = enumerable.Where(e => e != null);
        }

        return enumerable;
    }


}
