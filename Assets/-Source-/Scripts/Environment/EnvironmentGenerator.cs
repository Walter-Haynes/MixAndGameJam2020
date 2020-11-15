using System.Collections.Generic;
using UnityEngine;

namespace Generation
{
    /// <summary>
    /// Manage the procedural generation of the environment
    /// </summary>
    public class EnvironmentGenerator : MonoBehaviour
    {
        [Header("Settings to generate the environment")]
            [SerializeField]
                private List<EnvironmentSettings> environments;
            [SerializeField]
                private List<GameObject> obstacles;
            [SerializeField, Tooltip("How many prefabs tiles to have on a scene at one time")]
                private int totalTileCount = 10;
            [SerializeField, Tooltip("How many gameobjects to spawn per prefab")]
                private int totalPrefabCount = 5;
            [SerializeField, Tooltip("How far the top floor should be")]
                private float topFloorOffset = 25f;
            [SerializeField, Tooltip("How often to spawn an obstacle")]
                private float obstacleSpawnPercentage = 70f;
            [SerializeField, Tooltip("What z axis value should the prefabs spawn")]
                private float zOffset = -12f;
        
        #region Bottom

        // Currently spawned tiles Tuple
        // Gameobject tile, the kind of tile, and it's index in the pooler list, and it's an obstacle type tile
        private Queue<(GameObject, ENVIRONMENT_TYPE, int)> currentTilesBottom;
        private Queue<(GameObject, ENVIRONMENT_TYPE, int)> currentTilesTop;
        private Queue<GameObject> currentObstacles;
        // Current environment
        private ENVIRONMENT_TYPE currentEnvironmentType;
        // Current tracked position on the right end of the tiles
        private Vector3 currentRightPositionBottom = Vector3.zero;
        private Vector3 currentRightPositionTop = Vector3.zero;
        // List of all tile types and their pools
        private Dictionary<ENVIRONMENT_TYPE, List<Pooler>> tilePools;
        private Dictionary <string, bool> isObstacleTile; // I know this could be done better but time
        // List of all obstacles and their pools
        private Dictionary<string, Pooler> obstaclePools;
        // Total length of all the spawned tiles
        private float currentTileLengthBottom = 0f;
        private float currentTileLengthTop = 0f;

        // Used to track empty prefabs
        private string previousGeneratedObject;

        #endregion Bottom

        public void InitializeGrid() {
            currentTilesBottom = new Queue<(GameObject, ENVIRONMENT_TYPE, int)>();
            currentTilesTop = new Queue<(GameObject, ENVIRONMENT_TYPE, int)>();
            currentObstacles = new Queue<GameObject>();
            currentRightPositionTop.y = topFloorOffset;
            tilePools = new Dictionary<ENVIRONMENT_TYPE, List<Pooler>>();
            obstaclePools = new Dictionary<string, Pooler>();
            isObstacleTile = new Dictionary<string, bool>();
            InstantiateObjects();
            GenerateGrid();
        }

        private void InstantiateObjects() {
            // Tiles
            for (int i = 0; i < environments.Count; ++i) {
                Pooler pooler = gameObject.AddComponent<Pooler>();
                pooler.InitializePooler(environments[i].prefab, true, totalPrefabCount);
                List<Pooler> poolerList;
                tilePools.TryGetValue(environments[i].type, out poolerList);
                if (poolerList == null)
                    tilePools[environments[i].type] = new List<Pooler>();
                tilePools[environments[i].type].Add(pooler);
                isObstacleTile[environments[i].prefab.name] = environments[i].isObstacleTypeTile;
            }
            // Obstacles
            for (int i = 0; i < obstacles.Count; ++i) {
                Pooler pooler = gameObject.AddComponent<Pooler>();
                pooler.InitializePooler(obstacles[i], true, totalPrefabCount);
                obstaclePools[obstacles[i].name] = pooler;
            }
        }

        private void GenerateGrid() {
            currentEnvironmentType = (ENVIRONMENT_TYPE) GetRandomNumber(tilePools.Count);
            // Bottom floor generation
            for (int i = 0; i < totalTileCount; ++i) {
                GenerateNextTile(true);
                GenerateNextTile(false);
            }
            // // Top floor generation
            // for (int i = 0; i < totalTileCount; ++i) {
            //     GenerateNextTile(false);
            // }
        }

        private int GetRandomNumber(int range) {
            return Random.Range(0, range);
        }

        private float GetRandomInRange(float start, float end) {
            return Random.Range(start, end);
        }

        private void SetNewTileTotalLength(float length, bool bottom) {
            if (bottom)
                currentTileLengthBottom += length;
            else
                currentTileLengthTop += length;
        }

        /// <summary>
        /// Spawn a random new tile with the current ENVIRONMENT_TYPE
        /// </summary>
        public void GenerateNextTile(bool bottom) {
            List<Pooler> startingPools = tilePools[currentEnvironmentType];
            int randomNumber = GetRandomNumber(startingPools.Count);
            // Debug.Log(randomNumber + "  " + startingPools.Count);
            GameObject env = startingPools[randomNumber].GetObject();
            float xPosition = 0f;
            // I know this is not the neatest
            if (bottom) {
                Vector3 position = currentRightPositionBottom;
                position.z = zOffset;
                env.transform.position = position;
                env.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                env.SetActive(true);
                BoxCollider box = env.GetComponent<BoxCollider>();
                xPosition = currentRightPositionBottom.x + (box.bounds.size.x / 2);
                currentRightPositionBottom.x += box.bounds.size.x;
                currentTilesBottom.Enqueue((env, currentEnvironmentType, randomNumber));
                SetNewTileTotalLength(box.bounds.size.x, true);
            }
            else {
                // While previous was obstacle and current is obstacle
                while (isObstacleTile.ContainsKey(env.name) && isObstacleTile.ContainsKey(previousGeneratedObject)) {
                    env = startingPools[randomNumber].GetObject();
                }
                Vector3 position = currentRightPositionTop;
                position.z = zOffset;
                env.transform.position = position;
                env.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
                env.SetActive(true);
                BoxCollider box = env.GetComponent<BoxCollider>();
                xPosition = currentRightPositionTop.x + (box.bounds.size.x / 2);
                currentRightPositionTop.x += box.bounds.size.x;
                currentTilesTop.Enqueue((env, currentEnvironmentType, randomNumber));
                SetNewTileTotalLength(box.bounds.size.x, false);
            }
            previousGeneratedObject = env.name;
            // Spawn an obstacle for that tile
            SpawnObstacles(xPosition);
        }
        
        /// <summary>
        /// Called when the player advances forward enough
        /// Will put the last layer (on the leftmost side) in the pool
        /// and will spawn a new one on the far right.
        /// TODO: assumes player keeps moving to the right consistently
        /// </summary>
        private void UpdateTiles() {
            // Bottom
            (GameObject, ENVIRONMENT_TYPE, int) tileBottom = currentTilesBottom.Dequeue();
            SetNewTileTotalLength(-1 * tileBottom.Item1.GetComponent<BoxCollider>().bounds.size.x, true);
            tilePools[tileBottom.Item2][tileBottom.Item3].ReturnObject(tileBottom.Item1);
            GenerateNextTile(true);
            // Top
            (GameObject, ENVIRONMENT_TYPE, int) tileTop = currentTilesTop.Dequeue();
            SetNewTileTotalLength(-1 * tileTop.Item1.GetComponent<BoxCollider>().bounds.size.x, false);
            tilePools[tileTop.Item2][tileTop.Item3].ReturnObject(tileTop.Item1);
            GenerateNextTile(false);
            // Obstacles
            GameObject obstacle = currentObstacles.Dequeue();
            if (obstacle == null) return;
            string name = obstacle.name.Replace("(Clone)", "");
            obstaclePools[name].ReturnObject(obstacle);
        }

        // TODO: implement
        public void SwitchEnvironment() {
            currentEnvironmentType = (ENVIRONMENT_TYPE) GetRandomNumber(tilePools.Count);
        }

        public void CheckPlayerPosition(float currentPlayerXPosition) {
            // If player is in the middle of the length of all the tiles then spawn the next one
            // TODO: can store first tile on creation instead of checking each time
            Vector3 firstTile = currentTilesBottom.Peek().Item1.transform.position; // should use start of collider
            float length = (currentRightPositionBottom.x - firstTile.x);
            if (length > 0 && currentPlayerXPosition >= (length / 2f) + firstTile.x) {
                UpdateTiles();
            }
        }

        private void SpawnObstacles(float currentTileMiddlePosition) {
            // TODO: Should do 0 - 1 and make obstacleSpawnPercentage a decimal value
            if (GetRandomNumber(100) > obstacleSpawnPercentage) {
                int index = GetRandomNumber(obstacles.Count);
                string name = obstacles[index].name;
                GameObject obstacle = obstaclePools[name].GetObject();
                obstacle.transform.position = new Vector3(GetRandomInRange(currentTileMiddlePosition - 2f, currentTileMiddlePosition + 2f),
                                                          GetRandomInRange(6f, topFloorOffset - 6f),
                                                          zOffset);
                if (GetRandomNumber(2) == 1)
                    obstacle.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
                obstacle.SetActive(true);
                currentObstacles.Enqueue(obstacle);
            }
            else {
                currentObstacles.Enqueue(null);
            }
        }
    }

    public enum ENVIRONMENT_TYPE {
        Japan
    }
}