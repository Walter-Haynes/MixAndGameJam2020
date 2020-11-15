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
                private int totalPrefabCount = 5;
            [SerializeField, Tooltip("How far the top floor should be")]
                private float topFloorOffset = 100f;
        
        #region Bottom

        // Currently spawned tiles Tuple
        // Gameobject tile, the kind of tile, and it's index in the pooler list
        private Queue<(GameObject, ENVIRONMENT_TYPE, int)> currentTilesBottom;
        private Queue<(GameObject, ENVIRONMENT_TYPE, int)> currentTilesTop;
        // Current environment
        private ENVIRONMENT_TYPE currentEnvironmentType;
        // Current tracked position on the right end of the tiles
        private Vector3 currentRightPositionBottom = Vector3.zero;
        private Vector3 currentRightPositionTop = Vector3.zero;
        // List of all tile types and their pools
        private Dictionary<ENVIRONMENT_TYPE, List<Pooler>> tilePools;
        // Total length of all the spawned tiles
        private float currentTileLengthBottom = 0f;
        private float currentTileLengthTop = 0f;

        #endregion Bottom

        public void InitializeGrid() {
            currentTilesBottom = new Queue<(GameObject, ENVIRONMENT_TYPE, int)>();
            currentTilesTop = new Queue<(GameObject, ENVIRONMENT_TYPE, int)>();
            currentRightPositionTop.y = topFloorOffset;
            InstantiateObjects();
            GenerateGrid();
        }

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
            // Bottom floor generation
            for (int i = 0; i < totalTileCount; ++i) {
                GenerateNextTile(true);
            }
            // Top floor generation
            for (int i = 0; i < totalTileCount; ++i) {
                GenerateNextTile(false);
            }
        }

        private int GetRandomNumber(int range) {
            return Random.Range(0, range);
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
            GameObject env = startingPools[randomNumber].GetObject();
            // I know this is not the neatest
            if (bottom) {
                env.transform.position = currentRightPositionBottom;
                env.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                env.SetActive(true);
                BoxCollider box = env.GetComponent<BoxCollider>();
                currentRightPositionBottom.x += box.bounds.size.x;
                currentTilesBottom.Enqueue((env, currentEnvironmentType, randomNumber));
                SetNewTileTotalLength(box.bounds.size.x, true);
            }
            else {
                env.transform.position = currentRightPositionTop;
                env.transform.rotation = Quaternion.Euler(0f, 0f, 180f);
                env.SetActive(true);
                BoxCollider box = env.GetComponent<BoxCollider>();
                currentRightPositionTop.x += box.bounds.size.x;
                currentTilesTop.Enqueue((env, currentEnvironmentType, randomNumber));
                SetNewTileTotalLength(box.bounds.size.x, false);
            }
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
        }

        // TODO: implement
        public void SwitchEnvironment() {
            currentEnvironmentType = (ENVIRONMENT_TYPE) GetRandomNumber(tilePools.Count);
        }

        public void CheckPlayerPosition(float currentPlayerXPosition) {
            // If player is in the middle of the length of all the tiles then spawn the next one
            // TODO: can store first item instead of checking each time
            Vector3 firstTile = currentTilesBottom.Peek().Item1.transform.position; // should use start of collider
            float length = (currentRightPositionBottom.x - firstTile.x);
            if (length > 0 && currentPlayerXPosition >= (length / 2f) + firstTile.x) {
                UpdateTiles();
            }
        }
    }

    public enum ENVIRONMENT_TYPE {
        Japan
    }
}