using System;
using Events;
using PlayerSystem.Input.Data;
using UnityEngine;

// TODO: add head-bob
namespace Characters.PlayerSystem
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private bool canRotate = true;
    
        [Header("References")]
        [SerializeField] private Transform holdObjectPoint;

        [Header("Mouse Movement")]
        [SerializeField] private bool invertY = false;
        [SerializeField, Range(0.1f, 1f)] private float sensitivity = 0.2f;

        private Vector3 _eulerAngles;
    
        public Transform HoldObjectPoint => holdObjectPoint;
        public Vector3 EulerAngles => _eulerAngles;

        private void Awake()
        {
            canRotate = true;
        }

        private void OnEnable()
        {
            GameEventManager.Instance.SettingEventHandler.LookSettingsChanged += UpdateCameraSettings;
        }

        private void OnDisable()
        {
            GameEventManager.Instance.SettingEventHandler.LookSettingsChanged -= UpdateCameraSettings;
        }

        public void Initialize(Transform cameraTarget)
        {
            transform.position = cameraTarget.position;
            transform.eulerAngles = _eulerAngles = cameraTarget.eulerAngles;
        }

        public void UpdateCamera(CameraInput cameraInput)
        {
            RotateCamera(cameraInput.cameraLook);
        }

        private void UpdateCameraSettings(float cameraSensitivity, bool shouldInvert)
        {
            this.sensitivity = cameraSensitivity;
            this.invertY = shouldInvert;
        }

        private void RotateCamera(Vector2 rotateInput)
        {
            if (!canRotate) return;

            float yInput = invertY ? rotateInput.y : -rotateInput.y;
            _eulerAngles += new Vector3(yInput, rotateInput.x, 0f) * sensitivity;
            _eulerAngles.x = Mathf.Clamp(_eulerAngles.x, -80f, 80f);
            transform.eulerAngles = _eulerAngles;
        }

        public void MoveCameraPosition(Transform cameraTarget)
        {
            transform.position = cameraTarget.position;
        }

        // TODO: not correct camera look direction
        public void LoadCameraRotation(Vector3 savedRotation)
        {
            _eulerAngles = savedRotation;
            transform.eulerAngles = _eulerAngles;
        }
    }
}
