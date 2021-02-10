using System.Collections.Generic;
using UnityEngine;

namespace Faktori.Path
{
    public abstract class Path : MonoBehaviour
    {
        public abstract Vector3 GetPoint(int index);
        public abstract Vector3 GetPointAtTime(float t);
        public abstract Vector3 GetLocalPoint(int index);
        public abstract List<Vector3> GetPoints();
        public abstract void SetPoint(int index, Vector3 position);
    }
}
