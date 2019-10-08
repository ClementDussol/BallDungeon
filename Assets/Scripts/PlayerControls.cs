using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public float dashForce = 5;
    public float maxForce = 5;

    public float arrowLength = 1f;
    public SpriteRenderer dottedLine;
    public SpriteRenderer arrow;
    public ParticleSystem smokeParticles;
    
    // References
    private Rigidbody2D rigidbody;
    private SpriteRenderer sprite;
    
    // Trackers
    private Vector2 startPos;
    private Vector2 curPos;
    private bool isDashing = false;
    private bool isHolding = false;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        
        ShowArrow(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPos = Input.mousePosition;
            UpdateArrow(Vector2.zero);
            ShowArrow(true);
            TimeController.SetTimeScale(.1f);
            isHolding = true;

        } else if (Input.GetMouseButton(0))
        {
            curPos = Input.mousePosition;
            Vector2 delta = startPos - curPos;
            UpdateArrow(Vector2.ClampMagnitude(delta * dashForce, maxForce));
        }

        if (Input.GetMouseButtonUp(0))
        {
            curPos = Input.mousePosition;
            Vector2 delta = startPos - curPos;
            Vector2 force = Vector2.ClampMagnitude(delta * dashForce, maxForce);
            
            rigidbody.velocity = Vector2.zero;
            rigidbody.AddForce(force, ForceMode2D.Impulse);

            isHolding = false;
            isDashing = true;
            
            sprite.transform.localScale = new Vector3(1.5f + force.magnitude / 75, .5f - force.magnitude / 75, 1);
            UpdateRotation(force);
            ShowArrow(false);
            TimeController.SetTimeScale(1f);
        }
        
        if(isDashing)
            UpdateRotation(rigidbody.velocity);
        
        sprite.transform.localScale = Vector3.Lerp(sprite.transform.localScale, Vector3.one, .066f);
    }

    void UpdateRotation(Vector2 velocity)
    {
        float deg = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        sprite.transform.rotation = Quaternion.Euler(0, 0, deg);
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
        isDashing = false;
        sprite.transform.localScale = Vector3.one;

        ContactPoint2D contact = other.GetContact(0);
        float angleDifference = (contact.relativeVelocity.normalized - contact.normal).magnitude;
        
        Debug.Log(angleDifference);
        
        Debug.DrawLine(contact.point, contact.point + contact.normal, Color.red, 2f);
        Debug.DrawLine(contact.point, contact.point + rigidbody.velocity.normalized, Color.cyan, 2f);
        
        if(!isHolding && contact.relativeVelocity.magnitude > 7 && angleDifference < 1f)
        {
            float deg = Mathf.Atan2(contact.normal.y, contact.normal.x) * Mathf.Rad2Deg;
            smokeParticles.transform.rotation = Quaternion.Euler(0, 0, deg + 90);
            smokeParticles.transform.position = contact.point;
            smokeParticles.Play();
            CameraController.Shake(.005f * contact.relativeVelocity.magnitude, .05f);
        }
    }
    
        private void OnCollisionStay2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag("Slide")) return;
            ContactPoint2D contact = other.GetContact(0);
            float angleDifference = (contact.relativeVelocity.normalized - contact.normal).magnitude;
            if(!isHolding)
            {
                float deg = Mathf.Atan2(contact.normal.y, contact.normal.x) * Mathf.Rad2Deg;
                smokeParticles.transform.rotation = Quaternion.Euler(0, 0, deg + 90);
                smokeParticles.transform.position = contact.point;
                smokeParticles.Play();
                //CameraController.Shake(.005f * contact.relativeVelocity.magnitude, .05f);
            }
        }
}
