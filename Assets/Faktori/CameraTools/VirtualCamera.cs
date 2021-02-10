using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Faktori.CameraTools {
    public class VirtualCamera : MonoBehaviour
    {
        public int priority = 1;
        public float orthographicSize = 5;

        public bool overrideTransition = false;
        public CameraTransition transition;
        
        public UnityEvent onTransitionStart = new UnityEvent();
        public UnityEvent onTransitionEnd = new UnityEvent();

        void OnEnable()
        {
            Debug.Log("Enable virtual camera " + gameObject.name);
            CameraController.Instance.AddVirtualCamera(this);
        }

        void OnDisable()
        {
            Debug.Log("Disable virtual camera " + gameObject.name);
            CameraController.Instance?.RemoveVirtualCamera(this);
        }
        
        void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(transform.position, GetRectangleSize());
            Gizmos.color = new Color(1, 1, 1, 0.5f);
            Gizmos.DrawCube(transform.position, GetRectangleSize());
        }

        public Vector3 GetTargetPosition()
        {
            return transform.position;
        }

        private Vector3 GetRectangleSize()
        {
            return new Vector3(CameraController.Camera.aspect * orthographicSize * 2, orthographicSize * 2, 0);
        }
    }
}