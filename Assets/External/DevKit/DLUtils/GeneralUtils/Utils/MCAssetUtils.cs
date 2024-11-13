using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

namespace MindCoord.AssetTools
{
    public class MCAssetUtils
    {
        
        public static Bounds GetBounds(GameObject obj)
        {
            Bounds bounds = new Bounds();
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            if (renderers.Length > 0)
            {
                //Find first enabled renderer to start encapsulate from it
                foreach (Renderer renderer in renderers)
                {
                    if (renderer.enabled)
                    {
                        bounds = renderer.bounds;
                        break;
                    }
                }
                //Encapsulate for all renderers
                foreach (Renderer renderer in renderers)
                {
                    if (renderer.enabled)
                    {
                        bounds.Encapsulate(renderer.bounds);
                    }
                }
            }
            return bounds;
        }
    
        public void AutoAdjustGameObjectScale(GameObject obj, Camera cam, float resizeFactor=2.0f)
        {
            var size = GetBounds(obj).size;
            Debug.Log(size);
 
            float height = cam.orthographicSize * 2;
            float width = height * Screen.width/ Screen.height; // basically height * screen aspect ratio
            obj.transform.localScale = Vector3.one * width / resizeFactor / size.x;
 
        }
    
        public static string GetNameByManifest(string manifestpath)
        {
            string str = File.ReadAllText(manifestpath, Encoding.UTF8);
            string[] sArray=Regex.Split(str,"Assets:",RegexOptions.IgnoreCase);
            string[] sArray2=Regex.Split(sArray[sArray.Length-1],"/",RegexOptions.IgnoreCase);
            var name = sArray2[sArray2.Length - 1];
            string[] sArray3=Regex.Split(name,"\\.");
            var ext = "."+sArray3[sArray3.Length-1];
            var name2 = name.Replace(ext,"");
            return name2;
        }

        public static string GetStringTimeStampName()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").Replace(" ", "-").Replace(":", "-");
        }

        public static string normalizeName(string name)
        {
            name = name.ToLower();
            string[] chars = new string[]
            {
                " ",
                "_",
                "(",
                ")",
            };
            foreach (string c in chars)
            {
                name = name.Replace(c, "-");
            }
            return name;
        }

        public static void MakeDirs(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
        }
        
        public static T GenRandom<T>(List<T> list)
        {
            int a = list.Count;
            var index = new System.Random().Next(0, a);
            return list[index];
        }
    }
}

