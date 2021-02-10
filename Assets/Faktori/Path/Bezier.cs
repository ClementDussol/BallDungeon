using UnityEngine;

namespace Faktori.Path
{
    public static class Bezier
    {
        public static Vector3 GetPoint(Vector3 start, Vector3 handle, Vector3 end, float t)
        {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            return oneMinusT * oneMinusT * start + 2f * oneMinusT * t * handle + t * t * end;
        }
    
        public static Vector3 GetFirstDerivative (Vector3 start, Vector3 handle, Vector3 end, float t) {
            return 2f * (1f - t) * (handle - start) + 2f * t * (end - handle);
        }
    
        public static Vector3 GetPoint (Vector3 startPosition, Vector3 startTangent, Vector3 endTangent, Vector3 endPosition, float t) {
            t = Mathf.Clamp01(t);
            float oneMinusT = 1f - t;
            return
                oneMinusT * oneMinusT * oneMinusT * startPosition +
                3f * oneMinusT * oneMinusT * t * startTangent +
                3f * oneMinusT * t * t * endTangent +
                t * t * t * endPosition;
        }
    }
}
