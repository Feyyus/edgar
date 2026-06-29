using UnityEngine;

namespace Edgar.Characters.Core
{
    /// <summary>
    /// Makes a sprite always face the camera.
    /// Useful for 2D sprites in 3D space (billboard effect).
    /// </summary>
    public class BillboardSprite : MonoBehaviour
    {
        [SerializeField] private bool _lockYAxis = true;

        private Camera _mainCamera;

        void Start()
        {
            _mainCamera = Camera.main;
        }

        void LateUpdate()
        {
            if (_mainCamera == null) return;

            if (_lockYAxis)
            {
                // Only rotate on Y axis (character stays upright)
                Vector3 directionToCamera = _mainCamera.transform.position - transform.position;
                directionToCamera.y = 0;
                if (directionToCamera.sqrMagnitude > 0.001f)
                {
                    transform.rotation = Quaternion.LookRotation(directionToCamera);
                }
            }
            else
            {
                // Full billboard (faces camera completely)
                transform.rotation = _mainCamera.transform.rotation;
            }
        }
    }
}