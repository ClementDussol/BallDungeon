using UnityEngine;

namespace Faktori.EasingFunctions.Examples {
    public class EasingFunctionDisplayer : MonoBehaviour
    {
        public Easing.Functions easing;
        public RectTransform lineDot;
        public RectTransform cursor;

        public void Init(Easing.Functions easing)
        {
            this.easing = easing;
            for(float t = 0f; t < 1f; t += 0.01f)
            {
                RectTransform newDot = Instantiate(lineDot, CalculatePosition(t, easing), Quaternion.identity, lineDot.transform.parent);
            }

            Destroy(lineDot.gameObject);
        }

        private Vector2 CalculatePosition(float time, Easing.Functions easing)
        {
            Rect rect = (transform as RectTransform).rect;
            return (Vector2) transform.position + new Vector2(Mathf.LerpUnclamped(rect.xMin, rect.xMax, time), Mathf.LerpUnclamped(rect.yMin, rect.yMax, Easing.Interpolate(time, easing)));
        }

        public void Update()
        {
            cursor.position = CalculatePosition(Time.time % 1f, easing);
        }
    }
}
