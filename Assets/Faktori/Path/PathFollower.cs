using System;
using UnityEngine;

namespace Faktori.Path
{
    public class PathFollower : MonoBehaviour
    {
        [SerializeField, SerializeReference]
        public Path path;

        public float duration;
        public AnimationCurve curve;
        public bool ignoreTimeScale = false;
        
        [Range(0f, 1f)]
        public float offset = 0;

        private float _time = 0;

        public float Progress => _time / duration + offset;
        private void Update()
        {
            _time += ignoreTimeScale? Time.unscaledTime : Time.deltaTime;
            FollowPath(path, Progress);
        }

        private void OnEnable()
        {
            _time = 0;
        }

        private void FollowPath(Path path, float progress)
        {
            transform.position = path.GetPointAtTime(curve.Evaluate(progress));
        }

        private void OnValidate()
        {
            if(path)
                FollowPath(path, Progress);
        }
    }
}
