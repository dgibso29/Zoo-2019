//// GENERATED AUTOMATICALLY FROM 'Assets/Settings/InputControls.inputactions'

//using System;
//using UnityEngine;
//using UnityEngine.Events;
//using UnityEngine.Experimental.Input;


//[Serializable]
//public class InputControls : InputActionAssetReference
//{
//    public InputControls()
//    {
//    }
//    public InputControls(InputActionAsset asset)
//        : base(asset)
//    {
//    }
//    private bool m_Initialized;
//    private void Initialize()
//    {
//        // Camera
//        m_Camera = asset.GetActionMap("Camera");
//        m_Camera_PanLeft = m_Camera.GetAction("PanLeft");
//        if (m_CameraPanLeftActionStarted != null)
//            m_Camera_PanLeft.started += m_CameraPanLeftActionStarted.Invoke;
//        if (m_CameraPanLeftActionPerformed != null)
//            m_Camera_PanLeft.performed += m_CameraPanLeftActionPerformed.Invoke;
//        if (m_CameraPanLeftActionCancelled != null)
//            m_Camera_PanLeft.cancelled += m_CameraPanLeftActionCancelled.Invoke;
//        m_Camera_PanRight = m_Camera.GetAction("PanRight");
//        if (m_CameraPanRightActionStarted != null)
//            m_Camera_PanRight.started += m_CameraPanRightActionStarted.Invoke;
//        if (m_CameraPanRightActionPerformed != null)
//            m_Camera_PanRight.performed += m_CameraPanRightActionPerformed.Invoke;
//        if (m_CameraPanRightActionCancelled != null)
//            m_Camera_PanRight.cancelled += m_CameraPanRightActionCancelled.Invoke;
//        m_Camera_PanUp = m_Camera.GetAction("PanUp");
//        if (m_CameraPanUpActionStarted != null)
//            m_Camera_PanUp.started += m_CameraPanUpActionStarted.Invoke;
//        if (m_CameraPanUpActionPerformed != null)
//            m_Camera_PanUp.performed += m_CameraPanUpActionPerformed.Invoke;
//        if (m_CameraPanUpActionCancelled != null)
//            m_Camera_PanUp.cancelled += m_CameraPanUpActionCancelled.Invoke;
//        m_Camera_PanDown = m_Camera.GetAction("PanDown");
//        if (m_CameraPanDownActionStarted != null)
//            m_Camera_PanDown.started += m_CameraPanDownActionStarted.Invoke;
//        if (m_CameraPanDownActionPerformed != null)
//            m_Camera_PanDown.performed += m_CameraPanDownActionPerformed.Invoke;
//        if (m_CameraPanDownActionCancelled != null)
//            m_Camera_PanDown.cancelled += m_CameraPanDownActionCancelled.Invoke;
//        m_Camera_Pan = m_Camera.GetAction("Pan");
//        if (m_CameraPanActionStarted != null)
//            m_Camera_Pan.started += m_CameraPanActionStarted.Invoke;
//        if (m_CameraPanActionPerformed != null)
//            m_Camera_Pan.performed += m_CameraPanActionPerformed.Invoke;
//        if (m_CameraPanActionCancelled != null)
//            m_Camera_Pan.cancelled += m_CameraPanActionCancelled.Invoke;
//        m_Camera_Zoom = m_Camera.GetAction("Zoom");
//        if (m_CameraZoomActionStarted != null)
//            m_Camera_Zoom.started += m_CameraZoomActionStarted.Invoke;
//        if (m_CameraZoomActionPerformed != null)
//            m_Camera_Zoom.performed += m_CameraZoomActionPerformed.Invoke;
//        if (m_CameraZoomActionCancelled != null)
//            m_Camera_Zoom.cancelled += m_CameraZoomActionCancelled.Invoke;
//        m_Camera_ZoomIn = m_Camera.GetAction("ZoomIn");
//        if (m_CameraZoomInActionStarted != null)
//            m_Camera_ZoomIn.started += m_CameraZoomInActionStarted.Invoke;
//        if (m_CameraZoomInActionPerformed != null)
//            m_Camera_ZoomIn.performed += m_CameraZoomInActionPerformed.Invoke;
//        if (m_CameraZoomInActionCancelled != null)
//            m_Camera_ZoomIn.cancelled += m_CameraZoomInActionCancelled.Invoke;
//        m_Camera_ZoomOut = m_Camera.GetAction("ZoomOut");
//        if (m_CameraZoomOutActionStarted != null)
//            m_Camera_ZoomOut.started += m_CameraZoomOutActionStarted.Invoke;
//        if (m_CameraZoomOutActionPerformed != null)
//            m_Camera_ZoomOut.performed += m_CameraZoomOutActionPerformed.Invoke;
//        if (m_CameraZoomOutActionCancelled != null)
//            m_Camera_ZoomOut.cancelled += m_CameraZoomOutActionCancelled.Invoke;
//        m_Initialized = true;
//    }
//    private void Uninitialize()
//    {
//        if (m_CameraActionsCallbackInterface != null)
//        {
//            Camera.SetCallbacks(null);
//        }
//        m_Camera = null;
//        m_Camera_PanLeft = null;
//        if (m_CameraPanLeftActionStarted != null)
//            m_Camera_PanLeft.started -= m_CameraPanLeftActionStarted.Invoke;
//        if (m_CameraPanLeftActionPerformed != null)
//            m_Camera_PanLeft.performed -= m_CameraPanLeftActionPerformed.Invoke;
//        if (m_CameraPanLeftActionCancelled != null)
//            m_Camera_PanLeft.cancelled -= m_CameraPanLeftActionCancelled.Invoke;
//        m_Camera_PanRight = null;
//        if (m_CameraPanRightActionStarted != null)
//            m_Camera_PanRight.started -= m_CameraPanRightActionStarted.Invoke;
//        if (m_CameraPanRightActionPerformed != null)
//            m_Camera_PanRight.performed -= m_CameraPanRightActionPerformed.Invoke;
//        if (m_CameraPanRightActionCancelled != null)
//            m_Camera_PanRight.cancelled -= m_CameraPanRightActionCancelled.Invoke;
//        m_Camera_PanUp = null;
//        if (m_CameraPanUpActionStarted != null)
//            m_Camera_PanUp.started -= m_CameraPanUpActionStarted.Invoke;
//        if (m_CameraPanUpActionPerformed != null)
//            m_Camera_PanUp.performed -= m_CameraPanUpActionPerformed.Invoke;
//        if (m_CameraPanUpActionCancelled != null)
//            m_Camera_PanUp.cancelled -= m_CameraPanUpActionCancelled.Invoke;
//        m_Camera_PanDown = null;
//        if (m_CameraPanDownActionStarted != null)
//            m_Camera_PanDown.started -= m_CameraPanDownActionStarted.Invoke;
//        if (m_CameraPanDownActionPerformed != null)
//            m_Camera_PanDown.performed -= m_CameraPanDownActionPerformed.Invoke;
//        if (m_CameraPanDownActionCancelled != null)
//            m_Camera_PanDown.cancelled -= m_CameraPanDownActionCancelled.Invoke;
//        m_Camera_Pan = null;
//        if (m_CameraPanActionStarted != null)
//            m_Camera_Pan.started -= m_CameraPanActionStarted.Invoke;
//        if (m_CameraPanActionPerformed != null)
//            m_Camera_Pan.performed -= m_CameraPanActionPerformed.Invoke;
//        if (m_CameraPanActionCancelled != null)
//            m_Camera_Pan.cancelled -= m_CameraPanActionCancelled.Invoke;
//        m_Camera_Zoom = null;
//        if (m_CameraZoomActionStarted != null)
//            m_Camera_Zoom.started -= m_CameraZoomActionStarted.Invoke;
//        if (m_CameraZoomActionPerformed != null)
//            m_Camera_Zoom.performed -= m_CameraZoomActionPerformed.Invoke;
//        if (m_CameraZoomActionCancelled != null)
//            m_Camera_Zoom.cancelled -= m_CameraZoomActionCancelled.Invoke;
//        m_Camera_ZoomIn = null;
//        if (m_CameraZoomInActionStarted != null)
//            m_Camera_ZoomIn.started -= m_CameraZoomInActionStarted.Invoke;
//        if (m_CameraZoomInActionPerformed != null)
//            m_Camera_ZoomIn.performed -= m_CameraZoomInActionPerformed.Invoke;
//        if (m_CameraZoomInActionCancelled != null)
//            m_Camera_ZoomIn.cancelled -= m_CameraZoomInActionCancelled.Invoke;
//        m_Camera_ZoomOut = null;
//        if (m_CameraZoomOutActionStarted != null)
//            m_Camera_ZoomOut.started -= m_CameraZoomOutActionStarted.Invoke;
//        if (m_CameraZoomOutActionPerformed != null)
//            m_Camera_ZoomOut.performed -= m_CameraZoomOutActionPerformed.Invoke;
//        if (m_CameraZoomOutActionCancelled != null)
//            m_Camera_ZoomOut.cancelled -= m_CameraZoomOutActionCancelled.Invoke;
//        m_Initialized = false;
//    }
//    public void SetAsset(InputActionAsset newAsset)
//    {
//        if (newAsset == asset) return;
//        var CameraCallbacks = m_CameraActionsCallbackInterface;
//        if (m_Initialized) Uninitialize();
//        asset = newAsset;
//        Camera.SetCallbacks(CameraCallbacks);
//    }
//    public override void MakePrivateCopyOfActions()
//    {
//        SetAsset(ScriptableObject.Instantiate(asset));
//    }
//    // Camera
//    private InputActionMap m_Camera;
//    private ICameraActions m_CameraActionsCallbackInterface;
//    private InputAction m_Camera_PanLeft;
//    [SerializeField] private ActionEvent m_CameraPanLeftActionStarted;
//    [SerializeField] private ActionEvent m_CameraPanLeftActionPerformed;
//    [SerializeField] private ActionEvent m_CameraPanLeftActionCancelled;
//    private InputAction m_Camera_PanRight;
//    [SerializeField] private ActionEvent m_CameraPanRightActionStarted;
//    [SerializeField] private ActionEvent m_CameraPanRightActionPerformed;
//    [SerializeField] private ActionEvent m_CameraPanRightActionCancelled;
//    private InputAction m_Camera_PanUp;
//    [SerializeField] private ActionEvent m_CameraPanUpActionStarted;
//    [SerializeField] private ActionEvent m_CameraPanUpActionPerformed;
//    [SerializeField] private ActionEvent m_CameraPanUpActionCancelled;
//    private InputAction m_Camera_PanDown;
//    [SerializeField] private ActionEvent m_CameraPanDownActionStarted;
//    [SerializeField] private ActionEvent m_CameraPanDownActionPerformed;
//    [SerializeField] private ActionEvent m_CameraPanDownActionCancelled;
//    private InputAction m_Camera_Pan;
//    [SerializeField] private ActionEvent m_CameraPanActionStarted;
//    [SerializeField] private ActionEvent m_CameraPanActionPerformed;
//    [SerializeField] private ActionEvent m_CameraPanActionCancelled;
//    private InputAction m_Camera_Zoom;
//    [SerializeField] private ActionEvent m_CameraZoomActionStarted;
//    [SerializeField] private ActionEvent m_CameraZoomActionPerformed;
//    [SerializeField] private ActionEvent m_CameraZoomActionCancelled;
//    private InputAction m_Camera_ZoomIn;
//    [SerializeField] private ActionEvent m_CameraZoomInActionStarted;
//    [SerializeField] private ActionEvent m_CameraZoomInActionPerformed;
//    [SerializeField] private ActionEvent m_CameraZoomInActionCancelled;
//    private InputAction m_Camera_ZoomOut;
//    [SerializeField] private ActionEvent m_CameraZoomOutActionStarted;
//    [SerializeField] private ActionEvent m_CameraZoomOutActionPerformed;
//    [SerializeField] private ActionEvent m_CameraZoomOutActionCancelled;
//    public struct CameraActions
//    {
//        private InputControls m_Wrapper;
//        public CameraActions(InputControls wrapper) { m_Wrapper = wrapper; }
//        public InputAction @PanLeft { get { return m_Wrapper.m_Camera_PanLeft; } }
//        public ActionEvent PanLeftStarted { get { return m_Wrapper.m_CameraPanLeftActionStarted; } }
//        public ActionEvent PanLeftPerformed { get { return m_Wrapper.m_CameraPanLeftActionPerformed; } }
//        public ActionEvent PanLeftCancelled { get { return m_Wrapper.m_CameraPanLeftActionCancelled; } }
//        public InputAction @PanRight { get { return m_Wrapper.m_Camera_PanRight; } }
//        public ActionEvent PanRightStarted { get { return m_Wrapper.m_CameraPanRightActionStarted; } }
//        public ActionEvent PanRightPerformed { get { return m_Wrapper.m_CameraPanRightActionPerformed; } }
//        public ActionEvent PanRightCancelled { get { return m_Wrapper.m_CameraPanRightActionCancelled; } }
//        public InputAction @PanUp { get { return m_Wrapper.m_Camera_PanUp; } }
//        public ActionEvent PanUpStarted { get { return m_Wrapper.m_CameraPanUpActionStarted; } }
//        public ActionEvent PanUpPerformed { get { return m_Wrapper.m_CameraPanUpActionPerformed; } }
//        public ActionEvent PanUpCancelled { get { return m_Wrapper.m_CameraPanUpActionCancelled; } }
//        public InputAction @PanDown { get { return m_Wrapper.m_Camera_PanDown; } }
//        public ActionEvent PanDownStarted { get { return m_Wrapper.m_CameraPanDownActionStarted; } }
//        public ActionEvent PanDownPerformed { get { return m_Wrapper.m_CameraPanDownActionPerformed; } }
//        public ActionEvent PanDownCancelled { get { return m_Wrapper.m_CameraPanDownActionCancelled; } }
//        public InputAction @Pan { get { return m_Wrapper.m_Camera_Pan; } }
//        public ActionEvent PanStarted { get { return m_Wrapper.m_CameraPanActionStarted; } }
//        public ActionEvent PanPerformed { get { return m_Wrapper.m_CameraPanActionPerformed; } }
//        public ActionEvent PanCancelled { get { return m_Wrapper.m_CameraPanActionCancelled; } }
//        public InputAction @Zoom { get { return m_Wrapper.m_Camera_Zoom; } }
//        public ActionEvent ZoomStarted { get { return m_Wrapper.m_CameraZoomActionStarted; } }
//        public ActionEvent ZoomPerformed { get { return m_Wrapper.m_CameraZoomActionPerformed; } }
//        public ActionEvent ZoomCancelled { get { return m_Wrapper.m_CameraZoomActionCancelled; } }
//        public InputAction @ZoomIn { get { return m_Wrapper.m_Camera_ZoomIn; } }
//        public ActionEvent ZoomInStarted { get { return m_Wrapper.m_CameraZoomInActionStarted; } }
//        public ActionEvent ZoomInPerformed { get { return m_Wrapper.m_CameraZoomInActionPerformed; } }
//        public ActionEvent ZoomInCancelled { get { return m_Wrapper.m_CameraZoomInActionCancelled; } }
//        public InputAction @ZoomOut { get { return m_Wrapper.m_Camera_ZoomOut; } }
//        public ActionEvent ZoomOutStarted { get { return m_Wrapper.m_CameraZoomOutActionStarted; } }
//        public ActionEvent ZoomOutPerformed { get { return m_Wrapper.m_CameraZoomOutActionPerformed; } }
//        public ActionEvent ZoomOutCancelled { get { return m_Wrapper.m_CameraZoomOutActionCancelled; } }
//        public InputActionMap Get() { return m_Wrapper.m_Camera; }
//        public void Enable() { Get().Enable(); }
//        public void Disable() { Get().Disable(); }
//        public bool enabled { get { return Get().enabled; } }
//        public InputActionMap Clone() { return Get().Clone(); }
//        public static implicit operator InputActionMap(CameraActions set) { return set.Get(); }
//        public void SetCallbacks(ICameraActions instance)
//        {
//            if (m_Wrapper.m_CameraActionsCallbackInterface != null)
//            {
//                PanLeft.started -= m_Wrapper.m_CameraActionsCallbackInterface.OnPanLeft;
//                PanLeft.performed -= m_Wrapper.m_CameraActionsCallbackInterface.OnPanLeft;
//                PanLeft.cancelled -= m_Wrapper.m_CameraActionsCallbackInterface.OnPanLeft;
//                PanRight.started -= m_Wrapper.m_CameraActionsCallbackInterface.OnPanRight;
//                PanRight.performed -= m_Wrapper.m_CameraActionsCallbackInterface.OnPanRight;
//                PanRight.cancelled -= m_Wrapper.m_CameraActionsCallbackInterface.OnPanRight;
//                PanUp.started -= m_Wrapper.m_CameraActionsCallbackInterface.OnPanUp;
//                PanUp.performed -= m_Wrapper.m_CameraActionsCallbackInterface.OnPanUp;
//                PanUp.cancelled -= m_Wrapper.m_CameraActionsCallbackInterface.OnPanUp;
//                PanDown.started -= m_Wrapper.m_CameraActionsCallbackInterface.OnPanDown;
//                PanDown.performed -= m_Wrapper.m_CameraActionsCallbackInterface.OnPanDown;
//                PanDown.cancelled -= m_Wrapper.m_CameraActionsCallbackInterface.OnPanDown;
//                Pan.started -= m_Wrapper.m_CameraActionsCallbackInterface.OnPan;
//                Pan.performed -= m_Wrapper.m_CameraActionsCallbackInterface.OnPan;
//                Pan.cancelled -= m_Wrapper.m_CameraActionsCallbackInterface.OnPan;
//                Zoom.started -= m_Wrapper.m_CameraActionsCallbackInterface.OnZoom;
//                Zoom.performed -= m_Wrapper.m_CameraActionsCallbackInterface.OnZoom;
//                Zoom.cancelled -= m_Wrapper.m_CameraActionsCallbackInterface.OnZoom;
//                ZoomIn.started -= m_Wrapper.m_CameraActionsCallbackInterface.OnZoomIn;
//                ZoomIn.performed -= m_Wrapper.m_CameraActionsCallbackInterface.OnZoomIn;
//                ZoomIn.cancelled -= m_Wrapper.m_CameraActionsCallbackInterface.OnZoomIn;
//                ZoomOut.started -= m_Wrapper.m_CameraActionsCallbackInterface.OnZoomOut;
//                ZoomOut.performed -= m_Wrapper.m_CameraActionsCallbackInterface.OnZoomOut;
//                ZoomOut.cancelled -= m_Wrapper.m_CameraActionsCallbackInterface.OnZoomOut;
//            }
//            m_Wrapper.m_CameraActionsCallbackInterface = instance;
//            if (instance != null)
//            {
//                PanLeft.started += instance.OnPanLeft;
//                PanLeft.performed += instance.OnPanLeft;
//                PanLeft.cancelled += instance.OnPanLeft;
//                PanRight.started += instance.OnPanRight;
//                PanRight.performed += instance.OnPanRight;
//                PanRight.cancelled += instance.OnPanRight;
//                PanUp.started += instance.OnPanUp;
//                PanUp.performed += instance.OnPanUp;
//                PanUp.cancelled += instance.OnPanUp;
//                PanDown.started += instance.OnPanDown;
//                PanDown.performed += instance.OnPanDown;
//                PanDown.cancelled += instance.OnPanDown;
//                Pan.started += instance.OnPan;
//                Pan.performed += instance.OnPan;
//                Pan.cancelled += instance.OnPan;
//                Zoom.started += instance.OnZoom;
//                Zoom.performed += instance.OnZoom;
//                Zoom.cancelled += instance.OnZoom;
//                ZoomIn.started += instance.OnZoomIn;
//                ZoomIn.performed += instance.OnZoomIn;
//                ZoomIn.cancelled += instance.OnZoomIn;
//                ZoomOut.started += instance.OnZoomOut;
//                ZoomOut.performed += instance.OnZoomOut;
//                ZoomOut.cancelled += instance.OnZoomOut;
//            }
//        }
//    }
//    public CameraActions @Camera
//    {
//        get
//        {
//            if (!m_Initialized) Initialize();
//            return new CameraActions(this);
//        }
//    }
//    [Serializable]
//    public class ActionEvent : UnityEvent<InputAction.CallbackContext>
//    {
//    }
//}
//public interface ICameraActions
//{
//    void OnPanLeft(InputAction.CallbackContext context);
//    void OnPanRight(InputAction.CallbackContext context);
//    void OnPanUp(InputAction.CallbackContext context);
//    void OnPanDown(InputAction.CallbackContext context);
//    void OnPan(InputAction.CallbackContext context);
//    void OnZoom(InputAction.CallbackContext context);
//    void OnZoomIn(InputAction.CallbackContext context);
//    void OnZoomOut(InputAction.CallbackContext context);
//}
