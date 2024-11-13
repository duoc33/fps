using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public static class ParseUtils
    {
        private static readonly List<string> EnvironmentValues = new();

        
        public static void ProcessEnvironmentValues(ref string text)
        {
            for (var i = 0; i < EnvironmentValues.Count; i++)
            {
                text = text.Replace(EnvironmentValues[i], EnvLoader.GetEnvValue(EnvironmentValues[i]));
            }
        }

        public static void Print(string content)
        {
            Debug.Log("====================================================================");
            Debug.Log(">>>>> " + content + " <<<<<<");
            Debug.Log("====================================================================");
        }
    }
}