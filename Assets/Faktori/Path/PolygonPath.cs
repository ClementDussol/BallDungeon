using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Faktori.Path
{
    public class PolygonPath : Path
    {
        public bool closed = false;
        public int Count => _points.Length;
        public float Length => GetLength();
        
        public bool updateLineRenderer = false;

        private LineRenderer _linerenderer;

        [SerializeField] internal Vector3[] _points = new []{ new Vector3(), new Vector3(1, 1, 0) };

        public override Vector3 GetPoint(int index)
        {
            return transform.TransformPoint(_points[index]);
        }

        public Vector3 GetClosestPoint(Vector3 position)
        {
            Vector3 localPosition = transform.InverseTransformPoint(position);
            Vector3 closestPoint = _points.OrderBy(p => Vector3.Distance(localPosition, p)).First();

            return closestPoint;
        }

        private Vector3 GetClosestPointOnLine(Vector3 position, Vector3 start, Vector3 end)
        {
            Vector3 delta = end - start;
            Vector3 lhs = position - start;
            float dot = Vector3.Dot(lhs, delta.normalized);
            dot = Mathf.Clamp(dot, 0f, delta.magnitude);
            return start + delta.normalized * dot;
        }

        public Vector3 GetClosestPointOnPath(Vector3 position)
        {
            Vector3 localPosition = transform.InverseTransformPoint(position);
            int closestPointIndex = 0;
            float distance = Mathf.Infinity;
            
            for (int i = 0; i < _points.Length; i++)
            {
                float dist = Vector3.Distance(_points[i], localPosition);
                if (dist < distance)
                {
                    closestPointIndex = i;
                    distance = dist;
                }
            }

            if (!closed && closestPointIndex == 0)
                return transform.TransformPoint(GetClosestPointOnLine(localPosition, _points[0], _points[1]));
            if (!closed && closestPointIndex == _points.Length - 1)
                return transform.TransformPoint(GetClosestPointOnLine(localPosition, _points[_points.Length - 1], _points[_points.Length - 2]));

            
            int nextIndex = (closestPointIndex + 1) % _points.Length;
            int prevIndex = closestPointIndex == 0 ? _points.Length - 1 : (closestPointIndex - 1) % _points.Length;
            
            Vector3 pointOnPrevSegment = GetClosestPointOnLine(localPosition, _points[closestPointIndex], _points[prevIndex]);
            Vector3 pointOnNextSegment = GetClosestPointOnLine(localPosition, _points[closestPointIndex], _points[nextIndex]);

            Vector3 localResult =
                Vector3.Distance(localPosition, pointOnPrevSegment) <
                Vector3.Distance(localPosition, pointOnNextSegment)
                    ? pointOnPrevSegment
                    : pointOnNextSegment;

            return transform.TransformPoint(localResult);
        }

        public override Vector3 GetPointAtTime(float t)
        {
            float totalLength = Length;
            float currentLength = 0;
            
            if (Count == 2)
                return transform.TransformPoint(Vector3.Lerp(_points[0], _points[1], t));
            
            for (int i = 0; i < (closed ? Count : Count - 1); i++)
            {
                Vector3 current = _points[i];
                Vector3 next = _points[(i + 1) % Count];

                float distance = Vector3.Distance(current, next);
                if (t <= (currentLength + distance) / totalLength)
                {
                    float p = (t * totalLength - currentLength) / distance;
                    return transform.TransformPoint(Vector3.Lerp(current, next, p));
                }

                currentLength += distance;
            }

            return Vector3.zero;
        }

        public override List<Vector3> GetPoints()
        {
            return _points.Select(p => transform.TransformPoint(p)).ToList();
        }
    
        public override void SetPoint(int index, Vector3 position)
        {
            _points[index] = transform.InverseTransformPoint(position);
        }

        public void AddPoint(Vector3 position)
        {
            _points = _points.Append(transform.InverseTransformPoint(position)).ToArray();
        }

        public void RemoveAt(int index)
        {
            List<Vector3> points = _points.ToList();
            points.RemoveAt(index);
            _points = points.ToArray();
        }
        
        public void InsertPoint(int index, Vector3 position)
        {
            var points = _points.ToList();
            points.Insert(index, transform.InverseTransformPoint(position));
            _points = points.ToArray();
        }
    
        public override Vector3 GetLocalPoint(int index)
        {
            return _points[index];
        }

        private float GetLength()
        {
            float l = 0;
            
            for (int i = 0; i < (closed ? Count : Count - 1); i++)
            {
                Vector3 current = _points[i];
                Vector3 next = _points[(i + 1) % Count];

                l += Vector3.Distance(current, next);
            }

            return l;
        }

        public void OnValidate()
        {
            if(!updateLineRenderer)
                return;
            
            _linerenderer = GetComponent<LineRenderer>();
                
            if(!_linerenderer)
                return;

            _linerenderer.positionCount = Count;
            _linerenderer.SetPositions(_points);
            _linerenderer.loop = closed;
            _linerenderer.useWorldSpace = false;
        }

        public void OnDrawGizmos()
        {
            DrawLine(Color.grey);
        }

        public void OnDrawGizmosSelected()
        {
            //DrawLine(Color.white);
        }

        private void DrawLine(Color color)
        {
            Gizmos.color = color;
            for (int i = 0; i < (closed ? _points.Length : _points.Length - 1); i++)
            {
                Gizmos.DrawLine(GetPoint(i), GetPoint((i + 1) % _points.Length));
            }
        }
    }
}
