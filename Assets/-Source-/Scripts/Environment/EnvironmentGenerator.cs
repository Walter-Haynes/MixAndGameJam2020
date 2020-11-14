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
        
        // Currently spawned tiles
        private List<GameObject> currentTiles;
        // Current environment
        private ENVIRONMENT_TYPE currentEnvironmentType;
        // Current tracked position on the left end of the tiles
        private Vector3 currentLeftPosition = Vector3.zero;
        // List of all tile types and their pools
        private Dictionary<ENVIRONMENT_TYPE, List<Pooler>> tilePools;

        public void InitializeGrid() {
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

        public void GenerateNextTile() {
            List<Pooler> startingPools = tilePools[currentEnvironmentType];
            int randomNumber = GetRandomNumber(startingPools.Count);
            GameObject env = startingPools[randomNumber].GetObject();
            env.transform.position = currentLeftPosition;
            env.SetActive(true);
            currentLeftPosition.x += env.GetComponent<BoxCollider>().bounds.size.x;
        }

        public void SwitchEnvironment() {
            currentEnvironmentType = (ENVIRONMENT_TYPE) GetRandomNumber(tilePools.Count);
        }
    }

    public enum ENVIRONMENT_TYPE {
        Japan
    }
}