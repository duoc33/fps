using Cysharp.Threading.Tasks;
using ScriptableObjectBase;
using UnityEngine;
using UnityEngine.InputSystem;
namespace FPS
{
    public class PlayerConfigCenterSO : SOBase
    {
        public PlayerStateSO playerStateSO;
        public PlayerMotionSO playerMotionSO;
        public WeaponSO weaponSO;
        private const string PlayerPrefabPath = "RuntimeFPSPrefab/FPSPlayerTemplate/Player";
        public override async UniTask Download()
        {
            await weaponSO.Download();
        }
        public GameObject InitPlayer(Vector3 position = default)
        {
            GameObject player = Instantiate(Resources.Load<GameObject>(PlayerPrefabPath),position,Quaternion.identity);
            player.name = "Player";
            
            playerStateSO.Apply(player);
            Camera.main.transform.SetParent(playerStateSO.GetCamerHolder());
            Camera.main.transform.localPosition = Vector3.zero;
            Camera.main.transform.localRotation = Quaternion.identity;

            CameraDithering cameraDithering = Camera.main.gameObject.AddComponent<CameraDithering>();

            FPSMotionController fPSMotionController = player.AddComponent<FPSMotionController>();
            fPSMotionController.playerMotionSO = playerMotionSO;
            fPSMotionController.CameraTransform = playerStateSO.GetCamerHolder();
            fPSMotionController.cameraDithering = cameraDithering;

            GameObject weapon = weaponSO.InstantiateWeapon(player.transform);

            FPSPlayerCenter playerCenter = player.AddComponent<FPSPlayerCenter>();
            playerCenter.weaponLogic = weapon.GetComponent<WeaponLogic>();
            // playerCenter.weaponAnimator = weapon.GetComponent<FPSWeaponAnimator>();
            playerCenter.AimTransform = Camera.main.transform;

            return player;
        }

        public override void OnDestroy()
        {
            weaponSO.OnDestroy();
            playerMotionSO.OnDestroy();
            playerStateSO.OnDestroy();
        }
    }
}

