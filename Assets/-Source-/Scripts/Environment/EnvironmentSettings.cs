using UnityEngine;

namespace Generation {
    [CreateAssetMenu(menuName= "Environment")]
    public class EnvironmentSettings : ScriptableObject
    {
        public GameObject prefab;
        public ENVIRONMENT_TYPE type;
    }
}