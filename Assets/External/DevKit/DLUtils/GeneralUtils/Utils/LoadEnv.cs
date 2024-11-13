using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class EnvLoader
{
    private static Dictionary<string, string> envValues;

    public static string GetEnvValue(string key)
    {
        if (envValues == null)
        {
            LoadEnv();
        }

        string value;
        if (envValues.TryGetValue(key, out value))
        {
            return value;
        }

        // 如果找不到对应的键，返回一个默认值或者抛出异常，视情况而定
        return null;
    }

    private static void LoadEnv()
    {
        string envFilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/.env";

        if (!File.Exists(envFilePath))
        {
            Debug.LogError($"This error occurs because the .env file is not added to the User directory which is {envFilePath}"
                           + "\n, and the environment variables required for this project are not configured in the .env file."
                           + "\nYou should configure it like this:\nAIGCPath=D:/UnityProjects/2D/AIGC");
            return;
        }
        string fileContent = File.ReadAllText(envFilePath);
        {
            envValues = new Dictionary<string, string>();

            string[] lines = fileContent.Split('\n');
            foreach (string line in lines)
            {
                // 忽略注释行
                if (!line.Trim().StartsWith("#"))
                {
                    string[] keyValue = line.Split('=');
                    if (keyValue.Length == 2)
                    {
                        string key = keyValue[0].Trim();
                        string value = keyValue[1].Trim();
                        envValues[key] = value;
                    }
                }
            }
        }
    }
}