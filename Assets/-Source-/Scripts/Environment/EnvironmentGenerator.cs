using System.Collections;
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
            [SerializeField, Tooltip("How many prefabs tiles to have on a scene at one time")]
                private int totalTileCount = 10;
            [SerializeField, Tooltip("How many gameobjects to spawn per prefab")]
                private int totalPrefabCount = 4;
        
        // Currently spawned tiles Tuple
        // Gameobject tile, the kind of tile, and it's index in the pooler list
        private Queue<(GameObject, ENVIRONMENT_TYPE, int)> currentTiles;
        // Current environment
        private ENVIRONMENT_TYPE currentEnvironmentType;
        // Current tracked position on the right end of the tiles
        private Vector3 currentRightPosition = Vector3.zero;
        // List of all tile types and their pools
        private Dictionary<ENVIRONMENT_TYPE, List<Pooler>> tilePools;
        // Total length of all the spawned tiles
        private float currentTileLength = 0f;

        public void InitializeGrid() {
            currentTiles = new Queue<(GameObject, ENVIRONMENT_TYPE, int)>();
            InstantiateObjects();
            GenerateGrid();
        }

        // TODO: top level
        private void InstantiateObjects() {
            tilePools = new Dictionary<ENVIRONMENT_TYPE, List<Pooler>>();
            for (int i = 0; i < environments.Count; ++i) {
                Pooler pooler = gameObject.AddComponent<Pooler>();
                pooler.InitializePooler(environments[i].prefab, true, totalPrefabCount);
                tilePools[environments[i].type] = new List<Pooler>();
                tilePools[environments[i].type].Add(pooler);
            }
        }

        private void GenerateGrid() {
            currentEnvironmentType = (ENVIRONMENT_TYPE) GetRandomNumber(tilePools.Count);
            for (int i = 0; i < totalTileCount; ++i) {
                GenerateNextTile();
            }
        }

        private int GetRandomNumber(int range) {
            return Random.Range(0, range);
        }

        private void SetNewTileTotalLength(float length) {
            currentTileLength += length;
        }

        /// <summary>
        /// Spawn a random new tile with the current ENVIRONMENT_TYPE
        /// </summary>
        public void GenerateNextTile() {
            List<Pooler> startingPools = tilePools[currentEnvironmentType];
            int randomNumber = GetRandomNumber(startingPools.Count);
            GameObject env = startingPools[randomNumber].GetObject();
            env.transform.position = currentRightPosition;
            env.SetActive(true);
            BoxCollider box = env.GetComponent<BoxCollider>();
            currentRightPosition.x += box.bounds.size.x;
            currentTiles.Enqueue((env, currentEnvironmentType, randomNumber));
            SetNewTileTotalLength(box.bounds.size.x);
        }
        
        /// <summary>
        /// Called when the player advances forward enough
        /// Will put the last layer (on the leftmost side) in the pool
        /// and will spawn a new one on the far right.
        /// TODO: assumes player keeps moving to the right consistently
        /// </summary>
        private void UpdateTiles() {
            (GameObject, ENVIRONMENT_TYPE, int) tile = currentTiles.Dequeue();
            SetNewTileTotalLength(-1 * tile.Item1.GetComponent<BoxCollider>().bounds.size.x);
            tilePools[tile.Item2][tile.Item3].ReturnObject(tile.Item1);
            GenerateNextTile();
        }

        // TODO: implement
        public void SwitchEnvironment() {
            currentEnvironmentType = (ENVIRONMENT_TYPE) GetRandomNumber(tilePools.Count);
        }

        public void CheckPlayerPosition(float currentPlayerXPosition) {
            // If player is in the middle of the length of all the tiles then spawn the next one
            // TODO: can store first item instead of checking each time
            Vector3 firstTile = currentTiles.Peek().Item1.transform.position; // should use start of collider
            float length = (currentRightPosition.x - firstTile.x);
            if (length > 0 && currentPlayerXPosition >= (length / 2f) + firstTile.x) {
                UpdateTiles();
            }
        }
    }

    public enum ENVIRONMENT_TYPE {
        Japan
    }
}