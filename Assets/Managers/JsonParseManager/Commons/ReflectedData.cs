using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Managers
{
    public abstract class ReflectedData<T>
    {
        protected abstract Task<T> Config();
        
        
        public async Task<T> SetUp(T t)  // , GlobalContext newContext)
        {
            var TType = typeof(T);
            // if (string.IsNullOrEmpty(name))
            // {
                // var typeString = TType.ToString();
                // name = typeString + MMOUtils.GetTypeCount(typeString);
            // }
            // this.context = newContext;
            // if (this is IModelNeed modelNeed)
            // {
            //     AssetManager.AddModelEvent(modelNeed.modelAddress);
            //     await modelNeed.GetModelAction(
            //         onMaterialsLoad: o =>
            //         {
            //             var animator = o.GetComponent<Animator>();
            //             if (animator != null && t is Entity entity)
            //             {
            //                 entity.animator.avatar = animator.avatar;
            //                 o.transform.SetParent(entity.transform, false);
            //             }
            //             AssetManager.TriggerModelEvent(modelNeed.modelAddress);
            //         },
            //         onProgress: (loaderContext, f) =>
            //         {
            //         });
            // }
            // if (t == null && this is ITemplate iTemplate)
            // {
            //     thisObject = await iTemplate.SetTemplate(context, name);
            //     thisObject.name = name;
            //     if (thisObject.TryGetComponent(TType, out var component))
            //     {
            //         t = thisObject.GetComponent<T>();
            //         context.SetElement(TType, name, thisObject);
            //     }
            // }
            // if (this is NeedDownload attrNeed)
            // {
            //     attrNeed.Collect();
            //     await context.Download();
            // }
            t = await Config();
            Debug.Log("Call: SetUp  - " + TType + "  " + t);
            return t;
        }
        
    }
}