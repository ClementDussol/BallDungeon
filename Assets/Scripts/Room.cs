using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Faktori.CameraTools;
using Faktori.Events;

public class RoomCameraTrigger : MonoBehaviour
{
    private TriggerEvents2D _triggerEvents;

    void Awake()
    {
        _triggerEvents = GetComponentInChildren<TriggerEvents2D>();
    }

    void Start()
    {

    }
}
