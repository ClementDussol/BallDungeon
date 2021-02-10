using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Faktori.Events {
    [System.Serializable] public class Vector3Event : UnityEvent<Vector3>{}
    [System.Serializable] public class Vector2Event : UnityEvent<Vector2>{}
    [System.Serializable] public class FloatEvent : UnityEvent<float>{}
    [System.Serializable] public class IntEvent : UnityEvent<int>{}
    [System.Serializable] public class Collider2DEvent : UnityEvent<Collider2D>{}
}