using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Faktori.Events;
using UnityEngine.Events;

public static class GameEvents
{
    public static UnityEvent OnGameStarted = new UnityEvent();
    public static UnityEvent OnPlayerDied = new UnityEvent();
    
    
    
    public static Vector2Event OnPlayerStartAiming = new Vector2Event();
    public static Vector2Event OnPlayerAim = new Vector2Event();
    public static Vector2Event OnPlayerReleaseAiming = new Vector2Event();
    public static Vector2Event OnCharacterDash = new Vector2Event();
}
