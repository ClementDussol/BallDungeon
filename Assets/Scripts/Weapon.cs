using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Weapon : MonoBehaviour
{
    public float fireRate;
    public ParticleSystem effect;

    public class ShootEvent : UnityEvent<Vector2>{}
    public ShootEvent OnShoot = new ShootEvent();

    public void StartShooting()
    {
        StartCoroutine(ShootRoutine());
    }

    public void StopShooting()
    {
        StopAllCoroutines();
    }

    public IEnumerator ShootRoutine()
    {
        while(true)
        {
            Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            Shoot(direction);
            yield return new WaitForSecondsRealtime(fireRate);
        }
    }

    void Shoot(Vector2 direction)
    {
        effect.transform.LookAt(transform.position + (Vector3) direction);
        effect.Play();
        OnShoot.Invoke(direction);
    }
}
