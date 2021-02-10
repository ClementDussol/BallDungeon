using System.Collections;
using UnityEngine;

public static class CameraShake
{
    public static Coroutine Shake(float amplitude, float duration, bool ignoreTimeScale = true)
    {
        GameObject routineRunner = new GameObject("TEMP_RoutineRunner");
        RoutineRunner behaviour = routineRunner.AddComponent<RoutineRunner>();
        
        return behaviour.StartCoroutine(ShakeRoutine(amplitude, duration, ignoreTimeScale, routineRunner));
    }

    private static IEnumerator ShakeRoutine(float amplitude, float duration, bool ignoreTimeScale, GameObject routineRunner)
    {
        GameObject container = new GameObject("TEMP_CameraContainer");
        Camera camera = Camera.main;

        camera.transform.SetParent(container.transform, true);

        Vector3 originalPosition = container.transform.position;
        float t = 0;
        
        while (t < duration)
        {
            t += ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
            container.transform.position = originalPosition + Random.insideUnitSphere * amplitude;
            yield return null;
        }

        container.transform.position = originalPosition;
        container.transform.DetachChildren();

        GameObject.DestroyImmediate(container);
        GameObject.DestroyImmediate(routineRunner);
    }

    private class RoutineRunner : MonoBehaviour {}
}
