using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

namespace CAT
{
    public class CameraHandler : MonoBehaviour
    {
        public Transform targetTransform;
        public Transform cameraTransform;
        public Transform cameraPivotTransform;
        private Transform cameraHandlerTransform;
        private Vector3 cameraTransformPostion;
        private LayerMask igoneLayers;        

        public static CameraHandler singleton;

        public float lookSpeed = 0.1f;
        public float followSpeed = 0.01f;   //0.1f
        public float pivotSpeed = 0.03f;

        private float targetPostion;
        private float defaultPosition;
        private float lookAngle;
        private float pivotAngle;

        public float minimumPivot = -35;
        public float maximumPivot = 35;

        public float cameraSphereRadius = 0.2f;
        public float cameraCollisionOffset = 0.2f;
        public float minimumCollisionOffset = 0.2f;

        public void Awake()
        {
            singleton = this;
            cameraHandlerTransform = transform;
            defaultPosition = cameraTransform.localPosition.z;
            igoneLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
            targetTransform = FindObjectOfType<PlayerManager>().transform;
        }
        
        public void FollowTarget(float delta)
        {
            Vector3 targetPostion = Vector3.Lerp(cameraHandlerTransform.position, targetTransform.position, delta / followSpeed);
            cameraHandlerTransform.position = targetPostion;
            HandleCameraCollisions(delta);
        }

        public void HandleCameraRotation(float delta, float xInput, float yInput)
        {
            lookAngle += (xInput * lookSpeed) / delta;
            pivotAngle -= (yInput * pivotSpeed) / delta;
            pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot);

            Vector3 rotation = Vector3.zero;
            rotation.y = lookAngle;
            Quaternion targetRotation = Quaternion.Euler(rotation);
            cameraHandlerTransform.rotation = targetRotation;

            rotation = Vector3.zero;
            rotation.x = pivotAngle;

            targetRotation = Quaternion.Euler(rotation);
            cameraPivotTransform.localRotation = targetRotation;
        }

        private void HandleCameraCollisions(float delta)
        {
            targetPostion = defaultPosition;

            RaycastHit hit;
            Vector3 direction = cameraTransform.position - cameraPivotTransform.position;
            direction.Normalize();

            if (Physics.SphereCast
                (cameraPivotTransform.position, cameraSphereRadius, direction, out hit,
                Mathf.Abs(targetPostion),
                igoneLayers))
            {
                float dis = Vector3.Distance(cameraPivotTransform.position, hit.point);
                targetPostion = -(dis - cameraCollisionOffset);                    
            }

            cameraTransformPostion.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPostion, delta / 0.5f);
            cameraTransform.localPosition = cameraTransformPostion;
        }
    }
}