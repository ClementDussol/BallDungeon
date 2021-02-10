using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Faktori.Events {
    public class TriggerEvents2D : MonoBehaviour
    {
        public Collider2DEvent TriggerEnter2D = new Collider2DEvent();
        public Collider2DEvent TriggerStay2D = new Collider2DEvent();
        public Collider2DEvent TriggerExit2D = new Collider2DEvent();
        public void OnTriggerEnter2D(Collider2D collider)
        {
            Debug.Log(collider.name + " has entered " + transform.parent.name + " - " + gameObject.name);
            TriggerEnter2D.Invoke(collider);
        }
        public void OnTriggerStay2D(Collider2D collider)
        {
            TriggerStay2D.Invoke(collider);
        }
        public void OnTriggerExit2D(Collider2D collider )
        {
            Debug.Log(collider.name + " has exit " + transform.parent.name + " - " + gameObject.name);
            TriggerExit2D.Invoke(collider);
        }
    }
}
