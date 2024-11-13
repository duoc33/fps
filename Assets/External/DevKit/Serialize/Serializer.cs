using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Converters;
using UnityEngine;
using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace Foundation
{
    public class Serializer
    {
        public static T Deserialize<T>(string jsonData) where T : class => JsonConvert.DeserializeObject<T>(jsonData, parseSettings);
        public static string Serialize<T>(T value) where T : class => JsonConvert.SerializeObject(value, parseSettings);
        public static void WriteAllText(string fileContent , string directoryPath = null , string filename = "data")
        {
            
            if(string.IsNullOrEmpty(directoryPath))
            {
                directoryPath = Application.streamingAssetsPath +"/"+filename+".json";
                if(!Directory.Exists(Application.streamingAssetsPath)) Directory.CreateDirectory(Application.streamingAssetsPath);
            }
            else{
                directoryPath = directoryPath +"/+"+filename+".json";
            }
            
            if (File.Exists(directoryPath)) 
            {
                File.Delete(directoryPath);
            }
            File.WriteAllText(directoryPath, fileContent);
        }
        public static string ReadAllText(string path) => File.ReadAllText(path);
        
        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        private static JsonSerializerSettings parseSettings = new JsonSerializerSettings()
        {
            Converters =
            {
                // new UnityObjectConverter(),
                new Vector3Converter(),
                new Vector3IntConverter(),
                new Vector2Converter(),
                new Vector2IntConverter(),
                new ColorConverter(),
                new StringEnumConverter(),
            },
            TypeNameHandling = TypeNameHandling.Objects,
            ContractResolver = new IgnoreFieldsContractResolver(),
            Formatting = Newtonsoft.Json.Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        };
        private class Vector3Converter : JsonConverter<Vector3>
        {
            public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
            {
                var jsonObject = new JObject
                {
                      { "x", value.x },
                      { "y", value.y },
                      { "z", value.z }
                };
                jsonObject.WriteTo(writer);
            }
            public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                JObject jsonObject = JObject.Load(reader);
                float x = jsonObject.GetValue("x").Value<float>();
                float y = jsonObject.GetValue("y").Value<float>();
                float z = jsonObject.GetValue("z").Value<float>();
                return new Vector3(x, y, z);
            }
        }
        private class Vector2Converter : JsonConverter<Vector2>
        {
            public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
            {
                var jsonObject = new JObject
                {
                      { "x", value.x },
                      { "y", value.y }
                };
                jsonObject.WriteTo(writer);
            }
            public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                JObject jsonObject = JObject.Load(reader);
                float x = jsonObject.GetValue("x").Value<float>();
                float y = jsonObject.GetValue("y").Value<float>();
                return new Vector2(x, y);
            }
        }
        private class Vector3IntConverter : JsonConverter<Vector3Int>
        {
            public override Vector3Int ReadJson(JsonReader reader, Type objectType, Vector3Int existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                JObject jsonObject = JObject.Load(reader);
                int x = jsonObject.GetValue("x").Value<int>();
                int y = jsonObject.GetValue("y").Value<int>();
                int z = jsonObject.GetValue("z").Value<int>();

                return new Vector3Int(x, y, z);
            }

            public override void WriteJson(JsonWriter writer, Vector3Int value, JsonSerializer serializer)
            {
                var jsonObject = new JObject
                {
                      { "x", value.x },
                      { "y", value.y },
                      { "z", value.z }
                };
                jsonObject.WriteTo(writer);
            }
        }
        private class Vector2IntConverter : JsonConverter<Vector2Int>
        {
            public override Vector2Int ReadJson(JsonReader reader, Type objectType, Vector2Int existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                JObject jsonObject = JObject.Load(reader);
                int x = jsonObject.GetValue("x").Value<int>();
                int y = jsonObject.GetValue("y").Value<int>();

                return new Vector2Int(x, y);
            }

            public override void WriteJson(JsonWriter writer, Vector2Int value, JsonSerializer serializer)
            {
                var jsonObject = new JObject
                {
                      { "x", value.x },
                      { "y", value.y },
                };
                jsonObject.WriteTo(writer);
            }
        }
        private class ColorConverter : JsonConverter<Color>
        {
            public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
            {
                JObject jsonObject = JObject.Load(reader);
                float r = jsonObject.GetValue("r").Value<float>();
                float g = jsonObject.GetValue("g").Value<float>();
                float b = jsonObject.GetValue("b").Value<float>();
                float a = jsonObject.GetValue("a").Value<float>();
                return new Color(r, g, b, a);
            }
            public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
            {
                var jsonObject = new JObject{
                       { "r", value.r },
                       { "g", value.g },
                       { "b", value.b },
                       { "a", value.a }
                };
                jsonObject.WriteTo(writer);
            }
        }
        private class IgnoreFieldsContractResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                JsonProperty property = base.CreateProperty(member, memberSerialization);

                // 忽略 name 和 hideFlags 字段
                if (property.PropertyName == "name" || property.PropertyName == "hideFlags")
                {
                    property.ShouldSerialize = _ => false;
                }

                return property;
            }
        }
        private class UnityObjectConverter : JsonConverter
        {
            public override bool CanConvert(System.Type objectType)
            {
                return typeof(UnityEngine.Object).IsAssignableFrom(objectType);
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                JObject jo = new JObject();

                foreach (var property in value.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    // 忽略 name 和 hideFlags 属性
                    if (property.Name == "name" || property.Name == "hideFlags")
                        continue;

                    if (property.CanRead)
                    {
                        object propertyValue = property.GetValue(value, null);
                        if (propertyValue != null)
                        {
                            jo.Add(property.Name, JToken.FromObject(propertyValue, serializer));
                        }
                    }
                }

                jo.WriteTo(writer);
            }

            public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
            {
                JObject jo = JObject.Load(reader);
                object obj = System.Activator.CreateInstance(objectType);

                foreach (var property in objectType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (property.CanWrite)
                    {
                        JToken token;
                        if (jo.TryGetValue(property.Name, out token))
                        {
                            object value = token.ToObject(property.PropertyType, serializer);
                            property.SetValue(obj, value, null);
                        }
                    }
                }

                return obj;
            }
        }
    }
}

