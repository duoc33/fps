using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FPS
{
    /// <summary>
    /// 左手坐标系 ， z轴是枪的正方向
    /// </summary>
    public class WeaponLogic : MonoBehaviour
    {
        public Transform bulletSpwanPoint;
        public GameObject bulletPrefab;
        public float MinFireInterval;

        public float recoilAmount = 2f;            // 后坐力的垂直强度
        public float recoilHorizontalAmount = 0.5f;// 后坐力的水平强度
        public float recoilRecoverySpeed = 5f;     // 恢复速度

        // private Vector3 deltaRotation;
        private Vector3 originalRotation;          // 初始旋转角度
        private Vector3 currentRecoil;             // 当前后坐力的位移
        float timeelapsed = 0;
        
        FPSWeaponAnimator animator;

        private void Start()
        {
            originalRotation = transform.localRotation.eulerAngles;
            animator = GetComponentInChildren<FPSWeaponAnimator>();
        }

        public bool Fire()
        {
            if(!CanFire())return false;
            if(animator!=null && animator.IsReloading()) return false;
            animator?.Fire();
            ApplyRecoil();
            InstantiateBullet();
            return true;
        }
        public bool PlayerFire(Vector3 dir)
        {
            if(!CanFire())return false;
            if(animator!=null && animator.IsReloading()) return false;
            animator?.Fire();
            ApplyRecoil();
            InstantiateBullet(dir);
            return true;
        }
        public bool IsReloading()=>animator.IsReloading();
        public bool Reload()
        {
            if(animator.IsReloading())
            {
                return false;
            }
            animator.Reload();
            return true;
        }
        void ApplyRecoil()
        {
            // deltaRotation = transform.localEulerAngles;
            // 在垂直和水平方向上施加后坐力
            currentRecoil += new Vector3(-recoilAmount, Random.Range(-recoilHorizontalAmount, recoilHorizontalAmount), 0);
        }
        void Update()
        {
            // 逐步恢复到初始位置，模拟后坐力恢复
            currentRecoil = Vector3.Lerp(currentRecoil, Vector3.zero, recoilRecoverySpeed * Time.deltaTime);

            transform.localEulerAngles = originalRotation + currentRecoil;
        }
        public void UnFire()
        {
            transform.localRotation = Quaternion.identity;
        }
        private bool CanFire()
        {
            if(Time.time - timeelapsed > MinFireInterval)
            {
                timeelapsed = Time.time;
                return true;
            }
            return false;
        }
        private GameObject InstantiateBullet()
        {
            GameObject Bullet = Instantiate(bulletPrefab);
            Bullet.transform.position = bulletSpwanPoint.transform.position;
            Bullet.transform.rotation = bulletSpwanPoint.transform.rotation;
            Bullet.GetComponent<BulletLogic>().Emit();
            return Bullet;
        }
        private GameObject InstantiateBullet(Vector3 dir)
        {
            GameObject Bullet = Instantiate(bulletPrefab);
            Bullet.transform.position = bulletSpwanPoint.transform.position;
            Bullet.transform.rotation = bulletSpwanPoint.transform.rotation;
            Bullet.GetComponent<BulletLogic>().Emit(dir);
            return Bullet;
        }
        
    }

}
