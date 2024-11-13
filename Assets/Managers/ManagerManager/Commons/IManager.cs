using UnityEngine;

namespace Managers
{
    public interface IManager
    {
        
    }

    public interface ManagerNeedInit : IManager
    {
        
    }

    public interface InheritManager : IManager
    {
        public void Process(GameObject gameObject);
    }

    // public interface StringEvent
    // {
    //     
    // }
    
    
    /// <summary>
    /// 实现了该接口的Manager类，需要提供一个IStruct类型, 即该Manager下的必须提供数据
    /// </summary>
    /// <typeparam name="T">IStruct类型, 即该Manager的数据类</typeparam>
    public interface HasStructData<T> where T : IStruct
    {
        public T data { get; set; }
    }
    
    /// <summary>
    /// Manager中需要的数据
    /// </summary>
    public interface IStruct
    {
    }


}