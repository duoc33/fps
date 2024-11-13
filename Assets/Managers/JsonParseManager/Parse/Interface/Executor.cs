using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Managers;
using Newtonsoft.Json;
using UnityEngine;

namespace Managers
{
    // public abstract class ExecutorSo<T> : Executor<T> where T: ScriptableObject
    // {
    //     public abstract T CreateSo();
    //     public abstract Task<T> SetSheetUp(T t);
    //     public override async Task<T> Config(T t)
    //     {
    //         var sheet = CreateSo();
    //         sheet = await SetSheetUp(sheet);
    //         SaveAsset(sheet);
    //         return sheet;
    //     }
    //
    //     private void SaveAsset(ScriptableObject sheet)
    //     {
    //         if (sheet == null)
    //         {
    //             Debug.Log("Script object is null!");
    //             return;
    //         }
    //         context.AddSObj(sheet);
    //         sheet.name = name;
    //         EditorNRuntimeUtils.SaveAsset(sheet, context.getSaveFolder() + name + ".asset");
    //     }
    //     
    // }

    

    
    ///// <summary>
    ///// 游戏中每个实际Executor通过继承抽象的Executor, 处理一些通过API处理
    ///// </summary>
    ///// <typeparam name="T">实现了IManager接口的类型</typeparam>
    //[Serializable]
    //public abstract class Executor<T>: IExecutor where T: IManager, new()
    //{
    //    #region BasicProperties
    //    public string description;
    //    [JsonIgnore] public T manager = new();

    //    #endregion

        
    //    private GameObject _thisObject;
        
    //    protected GlobalContext context;

    //    #region API
    //    public GameObject getGameObject() => _thisObject;

        

    //    #endregion

    //    #region Settings

    //    private void EventProcess()
    //    {
    //        if (manager is StringEvent stringEvent)
    //        {
                
    //        }
    //    }
        
    //    private void DataProcess()
    //    {
    //        Type managerType = manager.GetType();
    //        Type interfaceType = typeof(HasStructData<>);
    //        // 查找实现的接口
    //        Type implementedInterface = managerType.GetInterfaces()
    //            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType);
    //        if (implementedInterface != null)
    //        {
    //            Debug.Log("is a HasStructData");
    //            // 获取data属性
    //            PropertyInfo dataProperty = implementedInterface.GetProperty("data");
    //            if (dataProperty != null)
    //            {
    //                object dataValue = dataProperty.GetValue(manager);
    //                Debug.Log($"data value: {dataValue}");
    //            }
    //        }
    //    }

    //    private void TypeProcess()
    //    {
    //        if (manager is InheritManager inheritManager)
    //        {
    //            inheritManager.Process(_thisObject);
    //        }
    //        if (this is ITemplate template)
    //        {
    //            // template.SetTemplate()
    //        }
    //        if (this is IModelNeed modelNeed)
    //        {
                
    //        }
    //    }

    //    public void Run()
    //    {
    //        EventProcess();
    //        DataProcess();
    //        TypeProcess();
    //    }
    //    #endregion
        
        
        
    //    public async Task<T> SetUp(T t, GlobalContext newContext)
    //    {
    //        // var TType = typeof(T);
    //        // if (string.IsNullOrEmpty(name))
    //        // {
    //        //     var typeString = TType.ToString();
    //        //     name = typeString + LittleUtils.GetTypeCount(typeString);
    //        // }
    //        //
    //        // this.context = newContext;
    //        // if (this is IModelNeed modelNeed)
    //        // {
    //        //     await modelNeed.GetModelAction(o =>
    //        //     {
    //        //         var animator = o.GetComponent<Animator>();
    //        //         // if (animator != null && t is MonoBehaviour entity)
    //        //         // {
    //        //         //     entity.animator.avatar = animator.avatar;
    //        //         //     o.transform.SetParent(entity.transform);
    //        //         // }
    //        //         Debug.Log($"GetModel {name} Done");
    //        //     });
    //        // }
    //        // if (t == null && this is ITemplate iTemplate)
    //        // {
    //        //
    //        // }
    //        // if (this is NeedDownload attrNeed)
    //        // {
    //        //     attrNeed.Collect();
    //        //     await context.Download();
    //        // }
    //        // Debug.Log("Call: SetUp  - " + TType + "  " + name);
    //        // Debug.Log(t);
    //        // return t;
    //        return default;
    //    }

        
    //}
    
    
    
    
    
    
    // #region not use

    // public virtual async Task CustomAsyncConfig(T t)
    // {
    //     
    // }
        
    // public async Task ConfigAsync(T t)
    // {
    //     await CustomAsyncConfig(t, context);
    //     if (this is NeedDownload needDownload)
    //     {
    //         // 如果需要下载，则收集并下载
    //         needDownload.Collect(context);
    //         await needDownload.Download(context);
    //     }
    //     await Config(ref t, context);
    // }
    
        
    // public Task ConfigRoot(ref T t)
    // {
    //     return ConfigAsync(t, context);
    // }
        
        
    // public async Task ConfigSo(GlobalContext context,string saveName)
    // {
    //     T skillSheet = ScriptableObject.CreateInstance<T>();
    //
    //     if (this is BaseAttrNeed<T> baseAttrNeed)
    //     {
    //         await baseAttrNeed.ConfigRoot(ref skillSheet, context);
    //     }
    //     
    //     CorgiUtils.SaveAsset(skillSheet, context.getSaveFolder() + saveName + ".asset");
    // }

    // #endregion

}