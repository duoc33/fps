using UnityEngine;

namespace Managers.Behaviours
{
    public class WorldToScreenPoint: MonoBehaviour
    {
        [SerializeField] private Transform target;
        private Vector3 _offset;
        private Camera _camera;

        public void SetTarget(Transform _t, Vector3 offset)
        {
            target = _t;
            _offset = offset;
        }

        private void Start()
        {
#if CAMERA_MANAGER
            _camera = CameraManager.CameraBrain.OutputCamera;
#else
            _camera = Camera.main;
#endif
        }


        private void Update()
        {
            var screenPoint = _camera.WorldToScreenPoint(target.position + _offset);
            transform.position = screenPoint;
        }

    }
}