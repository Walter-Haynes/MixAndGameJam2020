using UnityEngine;
using Generation;
using Cinemachine;

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
        Vector3 startingPosition = new Vector3(2f, 3f, zAxisValue);
        player = Instantiate(playerPrefab,startingPosition, Quaternion.identity);
        cinemachine.Follow = player.transform;
        cinemachine.LookAt = player.transform;
    }
}
