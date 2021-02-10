using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Faktori.Behaviours
{
    public class FollowTransform : MonoBehaviour
    {
        public Transform target;
        public float smoothness = 1;
        public Vector3 offset;
        public Vector3 influence = Vector3.one;
        public bool snapToTarget = false;

        private Vector3 _velocity;
        private Vector3 _startPosition;

        void Start()
        {
            _startPosition = transform.position;
        }
        
        void Update()
        {
            if(!target)
                return;
        
            Follow(target);   
        }

        private void OnEnable()
        {
            if(snapToTarget)
                transform.position = ProcessTargetPosition();
        }

        private void Follow(Transform target)
        {
            transform.position = ProcessTargetPosition();
        }

        private Vector3 ProcessTargetPosition()
        {
            Vector3 delta = target.position - _startPosition;
            return Vector3.SmoothDamp(transform.position, _startPosition + offset + Vector3.Scale(delta, influence), ref _velocity, smoothness);
        }
    }
}