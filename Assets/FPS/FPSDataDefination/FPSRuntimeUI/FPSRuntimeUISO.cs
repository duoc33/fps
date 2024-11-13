using ScriptableObjectBase;
using UnityEngine;
namespace FPS
{
    public class FPSRuntimeUISO : SOBase
    {
        public string AimPointIcon = "Temp/AimPointImage/red-circle";
        private GameObject root;
        private const string FPSRuntimeUI = "FPSUI/FPSRuntimeCanvas";
        private const string SuccessPanelUI = "FPSUI/SuccessCanvas";
        private const string FailPanelUI = "FPSUI/FailedCanvas";

        public void InitRuntimeUI()
        {
            root ??= new GameObject("UI Root");
            runtimePanel ??= Instantiate(Resources.Load<GameObject>(FPSRuntimeUI),root.transform);
            // runtimePanel.AddComponent<FPSRuntimeUICanvas>().AimPointIcon = GetDownloadedSrpite(AimPointIcon);
            runtimePanel.AddComponent<FPSRuntimeUICanvas>().AimPointIcon = Resources.Load<Sprite>(AimPointIcon);
            runtimePanel.transform.SetAsLastSibling();
            successPanel ??= Instantiate(Resources.Load<GameObject>(SuccessPanelUI),root.transform);
            successPanel.transform.SetAsLastSibling();
            successPanel.SetActive(false);
            failPanel ??= Instantiate(Resources.Load<GameObject>(FailPanelUI),root.transform);
            failPanel.transform.SetAsLastSibling();
            failPanel.SetActive(false);
        }
        private GameObject runtimePanel;
        private GameObject successPanel;
        private GameObject failPanel;
        public GameObject GetFPSRuntimePanel() => runtimePanel;
        public GameObject GetSuccessPanel() => successPanel;

        public GameObject GetFailedPanel() => failPanel;

        public override void OnDestroy()
        {
            Destroy(root);
        }

    }
}

