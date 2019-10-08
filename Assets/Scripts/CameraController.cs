using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CameraController
{
    public static Coroutine Shake(float amplitude, float duration)
    {
        GameObject routineRunner = new GameObject("TEMP_RoutineRunner");
        RoutineRunner behaviour = routineRunner.AddComponent<RoutineRunner>();
        
        return behaviour.StartCoroutine(ShakeRoutine(amplitude, duration, routineRunner));
    }

    private static IEnumerator ShakeRoutine(float amplitude, float duration, GameObject routineRunner)
    {
        Camera camera = Camera.main;
        Vector3 originalPosition = camera.transform.position;
        float t = 0;
        
        while (t < duration)
        {
            t += Time.deltaTime;
            camera.transform.position = originalPosition + Random.insideUnitSphere * amplitude;
            yield return null;
        }

        camera.transform.position = originalPosition;
        
        GameObject.DestroyImmediate(routineRunner);
    }
    
    private class RoutineRunner : MonoBehaviour {}
}
