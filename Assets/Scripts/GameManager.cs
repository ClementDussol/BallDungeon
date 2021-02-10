using System.Collections;
using System.Collections.Generic;
using Faktori;
using Faktori.CameraTools;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    
    private GameObject _playerInstance;
    private GameObject _currentCheckpoint;
    void Start()
    {
        GameEvents.OnGameStarted.AddListener(() =>
        {
            _playerInstance = SpawnPlayer(Vector3.zero);
        });
        
        GameEvents.OnPlayerDied.AddListener(() =>
        {
            TimeController.SetTimeScale(0f);
            CameraShake.Shake(.25f, .5f);

            SceneManager.LoadScene(0);
        });
        
        GameEvents.OnGameStarted.Invoke();
    }

    GameObject SpawnPlayer(Vector3 position)
    {
        return Instantiate(playerPrefab, position, Quaternion.identity);
    }
}
