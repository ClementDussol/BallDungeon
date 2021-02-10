using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Faktori.EasingFunctions;
using UnityEngine;
using UnityEngine.Events;

namespace Faktori.CameraTools {
    public class CameraController : Singleton<CameraController>
    {
        public CameraTransition transition;
        
        public UnityEvent onTransitionStart = new UnityEvent();
        public UnityEvent onTransitionEnd = new UnityEvent();

        private Camera _camera;
        private List<VirtualCamera> _virtualCameras = new List<VirtualCamera>();
        private VirtualCamera _activeVirtualCamera;
        private Coroutine _transitionRoutine;
        
        // Lambdas
        public static Camera Camera => Camera.main;
        public VirtualCamera ActiveVirtualCamera => _activeVirtualCamera;
        
        void Start()
        {
            _camera = Camera.main;
        }

        void Update()
        {
            if (_transitionRoutine != null || _virtualCameras.Count == 0)
                return;
            
            UpdateCameraPosition(ActiveVirtualCamera);
            UpdateCameraOrthographicSize(ActiveVirtualCamera);
        }

        void UpdateCameraPosition(VirtualCamera virtualCamera)
        {
            SetCameraPosition(virtualCamera.GetTargetPosition());
        }

        void UpdateCameraOrthographicSize(VirtualCamera virtualCamera)
        {
            SetCameraOrthographicSize(virtualCamera.orthographicSize);
        }

        void SetCameraPosition(Vector3 newPosition)
        {
            _camera.transform.position = newPosition;
        }

        void SetCameraOrthographicSize(float size)
        {
            Camera.orthographicSize = size;
        }

        public void SetActiveVirtualCamera(VirtualCamera newVirtualCamera)
        {
            if (!newVirtualCamera)
                return;
            
            if (_activeVirtualCamera && newVirtualCamera != _activeVirtualCamera)
                OnActiveVirtualCameraChange(newVirtualCamera);

            _activeVirtualCamera = newVirtualCamera;
        }
        
        public void AddVirtualCamera(VirtualCamera virtualCamera)
        {
            if(_virtualCameras.Contains(virtualCamera))
                return;
            
            _virtualCameras.Add(virtualCamera);
            ReorderVirtualCameras();
            SetActiveVirtualCamera(GetActiveVirtualCamera());
        }

        public void RemoveVirtualCamera(VirtualCamera virtualCamera)
        {
            if(!_virtualCameras.Contains(virtualCamera))
                return;
            
            _virtualCameras.Remove(virtualCamera);
            SetActiveVirtualCamera(GetActiveVirtualCamera());
        }

        private VirtualCamera GetActiveVirtualCamera()
        {
            if(_virtualCameras.Count == 0)
            {
                Debug.LogWarning("Didn't find any virtual camera");
                return null;
            }

            return _virtualCameras.Last(virtualCamera => virtualCamera.isActiveAndEnabled);
        }
        
        private IEnumerator TransitionRoutine(VirtualCamera virtualCamera, CameraTransition transition)
        {
            onTransitionStart.Invoke();
            virtualCamera.onTransitionStart.Invoke();

            Vector3 startPosition = _camera.transform.position;
            
            float startSize = _camera.orthographicSize;
            float endSize = virtualCamera.orthographicSize;

            float t = 0;

            while (t < transition.duration)
            {
                t += transition.ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
                float p = t / transition.duration;
                float i = Easing.Interpolate(p, transition.easing);

                SetCameraPosition(Vector3.LerpUnclamped(startPosition, virtualCamera.GetTargetPosition(), i));
                SetCameraOrthographicSize(Mathf.LerpUnclamped(startSize, endSize, i));
                
                yield return null;
            }

            onTransitionEnd.Invoke();
            virtualCamera.onTransitionEnd.Invoke();

            _transitionRoutine = null;
        }

        public void OnActiveVirtualCameraChange(VirtualCamera newCamera)
        {
            if(_transitionRoutine != null)
                StopCoroutine(_transitionRoutine);

            CameraTransition transition = newCamera.overrideTransition ? newCamera.transition : this.transition;
            
            if (transition.duration == 0)
            {
                onTransitionStart.Invoke();
                newCamera.onTransitionStart.Invoke();
                
                SetCameraPosition(newCamera.GetTargetPosition());
                SetCameraOrthographicSize(newCamera.orthographicSize);
                
                onTransitionEnd.Invoke();
                newCamera.onTransitionEnd.Invoke();
                
                return;
            }
            
            _transitionRoutine = StartCoroutine(TransitionRoutine(newCamera, transition));
        }

        private void ReorderVirtualCameras()
        {
            _virtualCameras = _virtualCameras.OrderBy(cam => cam.priority).ToList();
        }

        private void ClearVirtualCameras()
        {
            _virtualCameras.Clear();
        }
    }
    
    [System.Serializable]
    public class CameraTransition
    {
        public float duration = 1;
        public Easing.Functions easing = Easing.Functions.QuadraticInOut;
        public bool ignoreTimeScale = false;
    }
}