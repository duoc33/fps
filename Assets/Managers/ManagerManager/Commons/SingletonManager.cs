using UnityEngine;

namespace Managers
{
    public abstract class ISingleManager<T>: MonoBehaviour where T : Component
    {
        private static T _instance;
        private static bool HasInstance => _instance != null;
        public static T TryGetInstance() => HasInstance ? _instance : null;
        public static T Current => _instance;
        
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T> ();
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject ();
                        obj.name = typeof(T).Name + "_AutoCreated";
                        _instance = obj.AddComponent<T> ();
                    }
                }
                return _instance;
            }
        }
        
        protected virtual void Awake ()
        {
            InitializeSingleton();		
        }
        
        protected virtual void InitializeSingleton()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            _instance = this as T;
        }
    }
    
    
}