using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Experimental.Input;

namespace Zoo.Systems.Camera
{
    public class CameraController : MonoBehaviour //ICameraActions
    {
        Vector3 _lastMousePosition;

        GameObject _cameraTarget;

        //public InputControls Controls;

        /// <summary>
        /// Camera filter that is used to adjust brightness.
        /// </summary>
        //public CameraFilter brightnessFilter;

        //public Vector3 cameraStartingPosition;
        float CameraPanSpeed => MasterManager.Instance.GameConfig.CameraPanSpeed;
        float MinCameraZoom => MasterManager.Instance.GameConfig.MinCameraZoom;
        float MaxCameraZoom => MasterManager.Instance.GameConfig.MaxCameraZoom;
        float ZoomSpeed => MasterManager.Instance.GameConfig.ZoomSpeed;
        float MouseDragSensitivity => MasterManager.Instance.GameConfig.MouseDragSensitivity;

        void Awake()
        {
           // Controls.Camera.SetCallbacks(this);
        }

        // Use this for initialization
        void Start()
        {
            _cameraTarget = gameObject;
        }

        private void FixedUpdate()
        {
            //LockFiltersToCameraPosition();
        }

        public void OnEnable()
        {
          //  Controls.Camera.Enable();
        }
        public void OnDisable()
        {
          //  Controls.Camera.Disable();
        }

        public void PanUp()
        {
            transform.position += /*cameraTarget.*/transform.forward.normalized * CameraPanSpeed;
        }

        public void PanDown()
        {
            transform.position -= /*cameraTarget.*/transform.forward.normalized * CameraPanSpeed;
        }

        public void PanRight()
        {
            transform.position += /*cameraTarget.*/transform.right.normalized * CameraPanSpeed;
        }

        public void PanLeft()
        {
            transform.position -= /*cameraTarget.*/transform.right.normalized * CameraPanSpeed;
        }

        public void ZoomOut()
        {
            if (UnityEngine.Camera.main.orthographicSize < MaxCameraZoom)
                UnityEngine.Camera.main.orthographicSize += ZoomSpeed;
        }

        public void ZoomIn()
        {
            if (UnityEngine.Camera.main.orthographicSize > MinCameraZoom)
                UnityEngine.Camera.main.orthographicSize -= ZoomSpeed;
            
        }

        /// <summary>
        /// Must be once before initiating PanCamera.
        /// </summary>
        public void StartCameraPan()
        {
            _lastMousePosition = Input.mousePosition;
        }

        //public void PanCamera(InputAction.CallbackContext context)
        //{
        //    // This may need to move back at end and return to calling StartCameraPan first
        ////    _lastMousePosition = Input.mousePosition;
        ////    Vector3 mousePosChange = Input.mousePosition - _lastMousePosition;
        ////    /*cameraTarget.*/
        ////    transform.Translate(-(mousePosChange.x * MouseDragSensitivity), -(mousePosChange.y * MouseDragSensitivity), 0);

        //    var input = (Vector3)context.ReadValue<Vector2>();
        //    transform.Translate(input.x, 0, input.y);
        //}

        ///// <summary>
        ///// Sets all filters to camera position.
        ///// </summary>
        //void LockFiltersToCameraPosition()
        //{
        //    brightnessFilter.transform.position = transform.position;
        //}

        //public void OnPanLeft(InputAction.CallbackContext context)
        //{
        //    Debug.Log("Pan left");
        //    PanLeft();
        //}

        //public void OnPanRight(InputAction.CallbackContext context)
        //{
        //    Debug.Log("Pan right");
        //    PanRight();
        //}

        //public void OnPanUp(InputAction.CallbackContext context)
        //{
        //    Debug.Log("Pan up");
        //    PanUp();
        //}

        //public void OnPanDown(InputAction.CallbackContext context)
        //{
        //    Debug.Log("Pan down");

        //    PanDown();
        //}

        //public void OnPanStart(InputAction.CallbackContext context)
        //{
        //    Debug.Log("Start pan");
        //    StartCameraPan();
        //}

        //public void OnPan(InputAction.CallbackContext context)
        //{
        //    Debug.Log("Main pan");
        //    PanCamera(context);
        //}

        //public void OnZoom(InputAction.CallbackContext context)
        //{
        //    Debug.Log("Zoom");
        //    Debug.Log(context.ReadValue<Vector2>());
        //}

        //public void OnZoomIn(InputAction.CallbackContext context)
        //{
        //    Debug.Log("ZoomIn");
        //    ZoomIn();
        //}

        //public void OnZoomOut(InputAction.CallbackContext context)
        //{
        //    Debug.Log("ZoomOut");
        //    ZoomOut();
        //}
    }
}



