using UnityEngine;
using Generation;
using Cinemachine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public BeatDetection beatDetection;
    public EnvironmentGenerator environmentGenerator;
    public GameObject playerPrefab;
    public CinemachineVirtualCamera cinemachine;

    private float zAxisValue = -12f;
    private GameObject player;

    private void Awake() {
        environmentGenerator.InitializeGrid();
        Vector3 startingPosition = new Vector3(2f, 6f, zAxisValue);
        player = Instantiate(playerPrefab,startingPosition, Quaternion.identity);
        cinemachine.Follow = player.transform;
        cinemachine.LookAt = player.transform;
        StartCoroutine(CheckPlayerPosition());
    }

    // private void OnEnable() {
    //     playerMove.playerMovedDelegate += CheckTiles;
    // }

    // private void OnDisable() {
    //     playerMove.playerMovedDelegate -= CheckTiles;
    // }

    // TODO, do on player move event
    IEnumerator CheckPlayerPosition() {
        WaitForSeconds seconds = new WaitForSeconds(0.5f);
        while (true) {
            environmentGenerator.CheckPlayerPosition(player.transform.position.x);
            yield return seconds;
        }
    }

    /// <summary>
    /// Restart the level
    /// </summary>
    public void Restart() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
