using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using RuntimeComponentAdjustTools;
using ScriptableObjectBase;
using UnityEngine;
namespace FPS
{
    /// <summary>
    /// 武器的前方应该是Z轴，武器的Pivot应该是扳机点，武器的挂在地方应该是人物手掌或手指头。
    /// 子弹的前方应该是z轴，子弹的pivot起码不能离开bounds太远，子弹的生成位置应该是武器的枪口。
    /// </summary>
    public class WeaponSO : SOBase
    {
        [Header("WeaponHolder , BulletSpawnPoint 需要配置的两个点位")]
        public string WeaponPrefabUrl;
        public string BulletPrefabUrl;

        // public float WeaponSizeFactor ; //相对于当前人物模型，进行大小修正
        // public float BulletSizeFactor ; //相对于枪的模型，进行大小修正
        // public string FireVFX;
        // public string BulletVFX;


        public float recoilAmount = 2f;            // 后坐力的垂直强度
        public float recoilHorizontalAmount = 0.5f;// 后坐力的水平强度
        public float recoilRecoverySpeed = 5f;     // 恢复速度
        
        public int Damage = 5; // 子弹伤害值 
        [Header("射程 : BulletSpeed * BulletLifeTime ")]
        public float BulletSpeed = 1; // 子弹速度 , units / seconds
        public float BulletLifeTime = 10f; // 子弹生命周期 seconds
        public float MinFireInterval = 0.1f; // 最小攻击间隔 seconds


        public FPSWeaponAnimationSO fPSWeaponAnimationSO;

        private GameObject bullet => GetPooledGameObject(BulletPrefabUrl);

        /// <summary>
        /// 默认的武器挂载位置为 "WeaponHolder"
        /// </summary>
        private static string WeaponHolderName = "WeaponHolder"; // 武器挂载的位置
        /// <summary>
        /// 默认的子弹生成位置为 "BulletSpawnPoint"
        /// </summary>
        private string BulletSpawnPointName = "BulletSpawnPoint"; // 子弹生成的位置

        public GameObject InstantiateWeapon(Transform parent) => Instantiate(GetWeaponPrefab(),GetWeaponHolder(parent));
        public GameObject GetWeaponPrefab() => GetPooledGameObject(WeaponPrefabUrl);
        public Transform GetWeaponHolder(Transform parent)=> Find(parent,WeaponHolderName);

        public override async UniTask Download()
        {
            if(fPSWeaponAnimationSO!=null) 
            {
                await fPSWeaponAnimationSO.Download();
            }
            await DownloadAndPostProcessGameObject(BulletPrefabUrl,PostProcessBullet);
            await DownloadAndPostProcessGameObject(WeaponPrefabUrl,PostProcessWeapon);
            
        }
        private GameObject PostProcessBullet(GameObject target)
        {
            BoundsTool.SetBestCollider(target);
            GameObject bullet =  target;
            bullet.GetComponent<BoxCollider>().isTrigger = true;
            bullet.AddComponent<Rigidbody>().isKinematic = true;
            BulletLogic bulletLogic = bullet.AddComponent<BulletLogic>();
            bulletLogic.Damage = Damage;
            bulletLogic.BulletSpeed = BulletSpeed;
            bulletLogic.BulletLifeTime = BulletLifeTime;

            return bullet;
        }
        private GameObject PostProcessWeapon(GameObject target)
        {
            GameObject weapon = target;
            WeaponLogic weaponLogic = weapon.AddComponent<WeaponLogic>();
            weaponLogic.MinFireInterval = MinFireInterval;
            weaponLogic.recoilAmount = recoilAmount;
            weaponLogic.recoilHorizontalAmount = recoilHorizontalAmount;
            weaponLogic.recoilRecoverySpeed = recoilRecoverySpeed;

            weaponLogic.bulletSpwanPoint = GetBulletSpawnPoint(weaponLogic.transform);
            weaponLogic.bulletPrefab = bullet;

            
            if(fPSWeaponAnimationSO!=null)
            {
                fPSWeaponAnimationSO.Apply(weapon);
            }

            return weapon;
        }
        
        
        private Transform GetBulletSpawnPoint(Transform weapon)=> Find(weapon,BulletSpawnPointName);
        private static Transform Find(Transform transform ,string name)
        {
            Transform[] transforms = transform.GetComponentsInChildren<Transform>();
            foreach(Transform t in transforms)
            {
                if(t.name == name)
                {
                    return t;
                }
            }
            return null;
        }

        public override void OnDestroy()
        {
            fPSWeaponAnimationSO?.OnDestroy();
        }
    }

    

    

}