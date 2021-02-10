using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Faktori.Path
{
    public class BezierPath : Path
    {
        public bool closed = false;
        public int Count => _points.Length;

        [SerializeField] 
        private BezierPoint[] _points = new []
        {
            new BezierPoint(new Vector3(0, 0, 0), new Vector3(-1, -1, 0), new Vector3(1, 1, 0)),
            new BezierPoint(new Vector3(2, 0, 0), new Vector3(-1, -1, 0), new Vector3(1, 1, 0))
        };
    
        public override Vector3 GetPoint(int index)
        {
            return transform.TransformPoint(_points[index].position);
        }

        public override Vector3 GetPointAtTime(float t)
        {
            throw new System.NotImplementedException();
        }

        public BezierPoint GetBezierPoint(int index)
        {
            return transform.TransformBezierPoint(_points[index]);
        }

        public override Vector3 GetLocalPoint(int index)
        {
            return _points[index].position;
        }

        public override List<Vector3> GetPoints()
        {
            return _points.Select(p => transform.TransformPoint(p.position)).ToList();
        }

        public override void SetPoint(int index, Vector3 position)
        {
            _points[index].position = transform.InverseTransformPoint(position);
        }

        public void SetBezierPoint(int index, BezierPoint bezierPoint)
        {
            _points[index] = transform.InverseTransformBezierPoint(bezierPoint);
        }
    
        public Vector3 GetBezierPoint(float t)
        {
            return Bezier.GetPoint(GetPoint(0), GetPoint(1), GetPoint(2), t);
        }
    }

    public static class BezierTransformExtensions
    {
        public static BezierPoint TransformBezierPoint(this Transform t, BezierPoint bezierPoint)
        {
            return new BezierPoint(
                t.TransformPoint(bezierPoint.position), 
                t.TransformVector(bezierPoint.inTangent),
                t.TransformVector(bezierPoint.outTangent));
        }
    
        public static BezierPoint InverseTransformBezierPoint(this Transform t, BezierPoint bezierPoint)
        {
            return new BezierPoint(
                t.InverseTransformPoint(bezierPoint.position), 
                t.InverseTransformVector(bezierPoint.inTangent),
                t.InverseTransformVector(bezierPoint.outTangent));
        }
    }

    [System.Serializable]
    public struct BezierPoint
    {
        public TangentMode tangentMode;
        public Vector3 position;
        public Vector3 inTangent;
        public Vector3 outTangent;

        public BezierPoint(Vector3 position, Vector3 inTangent, Vector3 outTangent)
        {
            this.tangentMode = TangentMode.Broken;
            this.position = position;
            this.inTangent = inTangent;
            this.outTangent = outTangent;
        }
    }

    public enum TangentMode
    {
        Broken = 0, Continuous = 1, Linear = 2
    }
}