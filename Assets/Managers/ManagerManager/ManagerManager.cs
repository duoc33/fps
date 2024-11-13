using UnityEngine;

namespace Managers
{
    public static class ManagerManager
    {
        private static ManagerLifeLoop m_lifeLoop = null;
        public static GameOwnerInfo m_GameOwnerInfo = null;

        public static ManagerLifeLoop LifeLoop
        {
            get
            {
                if (m_lifeLoop == null)
                {
                    ManagerLifeLoopStart();
                }
                return m_lifeLoop;
            }
        }
        
        
        
        public static void ManagerLifeLoopStart()
        {
            m_lifeLoop = Object.FindObjectOfType<ManagerLifeLoop>();
            if (m_lifeLoop == null)
            {
                GameObject managerManager = new GameObject("ManageLifeLoop");
                m_lifeLoop = managerManager.AddComponent<ManagerLifeLoop>();   
            }
        }
    }
}