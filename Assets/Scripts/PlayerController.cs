using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Faktori;
using Faktori.CameraTools;

public class PlayerController : MonoBehaviour
{
    [Header("Controls")]
    public float dashForce = 5;
    public float maxForce = 5;
    public float slowMotion = .1f;

    [Header("Visuals")]
    public float arrowLength = 1f;
    public Transform model;
    public SpriteRenderer dottedLine;   
    public SpriteRenderer arrow;
    public ParticleSystem smokeParticles;
    
    // References
    private new Rigidbody2D _rigidbody;
    
    // Trackers
    private Vector2 _startPos;
    private Vector2 _curPos;
    private bool _isDashing = false;
    private bool _isHolding = false;
    
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        ShowArrow(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            OnClick(Input.mousePosition);
        else if (Input.GetMouseButton(0))
            OnHold(Input.mousePosition);
        if (Input.GetMouseButtonUp(0))
            OnRelease(Input.mousePosition);

        if(_isDashing)
            UpdateRotation(_rigidbody.velocity);

        model.transform.localScale = Vector3.Lerp(model.transform.localScale, Vector3.one, .05f * Time.timeScale);
    }

    public void OnClick(Vector2 mousePosition)
    {
        _startPos = mousePosition;
        UpdateArrow(Vector2.zero);
        ShowArrow(true);
        TimeController.SetTimeScale(slowMotion);
        _isHolding = true;
    }

    public void OnHold(Vector2 mousePosition)
    {
        _curPos = mousePosition;
        Vector2 delta = _startPos - _curPos;
        UpdateArrow(Vector2.ClampMagnitude(delta * dashForce, maxForce));
    }

    public void OnRelease(Vector2 mousePosition)
    {
        _curPos = mousePosition;

        Vector2 delta = _startPos - _curPos;
        Vector2 direction = delta.normalized;
        float force = Mathf.Clamp(delta.magnitude * dashForce, 0, maxForce);

        Dash(direction, force);

        _isHolding = false;
        _isDashing = true;
            
        model.transform.localScale = new Vector3(1.5f + force / 75, .5f - force / 75, 1);
        UpdateRotation(direction);
        ShowArrow(false);
        TimeController.SetTimeScale(1f);
    }

    public void Dash(Vector2 direction, float force)
    {
        _rigidbody.velocity = Vector2.zero;
        _rigidbody.AddForce(direction * force, ForceMode2D.Impulse);
    }

    void UpdateRotation(Vector2 velocity)
    {
        float deg = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        model.transform.rotation = Quaternion.Euler(0, 0, deg);
    }

    void UpdateArrow(Vector2 force)
    {
        force *= arrowLength;
        
        float deg = Mathf.Atan2(force.y, force.x) * Mathf.Rad2Deg;
        
        dottedLine.transform.position = (Vector2) transform.position + force / 2f;
        dottedLine.transform.rotation = Quaternion.Euler(0, 0, deg);
        dottedLine.size = new Vector2(force.magnitude, dottedLine.size.y);
       
        arrow.transform.position = (Vector2) transform.position + force;
        arrow.transform.rotation = Quaternion.Euler(0, 0, deg - 135);
    }

    void ShowArrow(bool show)
    {
        dottedLine.gameObject.SetActive(show);
        arrow.gameObject.SetActive(show);
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        _isDashing = false;
        model.transform.localScale = Vector3.one;

        ContactPoint2D contact = other.GetContact(0);
        float angleDifference = (contact.relativeVelocity.normalized - contact.normal).magnitude;
        
        Debug.DrawLine(contact.point, contact.point + contact.normal, Color.red, 2f);
        Debug.DrawLine(contact.point, contact.point + _rigidbody.velocity.normalized, Color.cyan, 2f);
        
        if(!_isHolding && contact.relativeVelocity.magnitude > 7 && angleDifference < 1f)
        {
            float deg = Mathf.Atan2(contact.normal.y, contact.normal.x) * Mathf.Rad2Deg;
            smokeParticles.transform.rotation = Quaternion.Euler(0, 0, deg + 90);
            smokeParticles.transform.position = contact.point;
            smokeParticles.Play();
            CameraShake.Shake(contact.relativeVelocity.magnitude * .01f, .05f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.gameObject.CompareTag("Deadly"))
            GameEvents.OnPlayerDied.Invoke();

        if (collider.gameObject.CompareTag("Enemy"))
        {
            collider.GetComponent<Animator>().SetTrigger("Hit");
            StartCoroutine(KillEnemyRoutine(collider));
        }
    }

    private IEnumerator KillEnemyRoutine(Collider2D enemy)
    {
        CameraShake.Shake(.1f, .25f);
        TimeController.SetTimeScale(.01f);
        yield return new WaitForSecondsRealtime(0.1f);
        Rigidbody2D rigidbody2D = enemy.attachedRigidbody;
        rigidbody2D.isKinematic = false;
        rigidbody2D.AddForce(_rigidbody.velocity.normalized * 5, ForceMode2D.Impulse);
        rigidbody2D.AddTorque(80);
        
        TimeController.SetTimeScale(1f);
    }
    
    private void OnCollisionStay2D(Collision2D other)
    {
        if (!other.gameObject.CompareTag("Slide")) return;
        ContactPoint2D contact = other.GetContact(0);
        float angleDifference = (contact.relativeVelocity.normalized - contact.normal).magnitude;
        if(!_isHolding)
        {
            float deg = Mathf.Atan2(contact.normal.y, contact.normal.x) * Mathf.Rad2Deg;
            smokeParticles.transform.rotation = Quaternion.Euler(0, 0, deg + 90);
            smokeParticles.transform.position = contact.point;
            smokeParticles.Play();
        }
    }
}
