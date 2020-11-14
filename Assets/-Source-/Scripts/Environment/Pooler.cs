using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Object pooler for GameObjects
/// </summary>
public class Pooler : MonoBehaviour
{

	[Header("Pool Settings")]

    [SerializeField, Tooltip("Prefab to instantiate in pool")]
    private GameObject prefab;

    [SerializeField, Tooltip("Size of pool")]
    private int poolSize;

    [SerializeField, Tooltip("Can the pool size grow?")]
    private bool expandable;

    private List<GameObject> freeList;
    private List<GameObject> usedList;
	
    // Keeps track of size of list
	private int totalFree = 0;

    public void InitializePooler(GameObject prefab, bool expandable, int poolSize) {
        this.prefab = prefab;
        this.expandable = expandable;
        this.poolSize = poolSize;
		// Initialize lists
        freeList = new List<GameObject>();
        usedList = new List<GameObject>();

		// Instantiate Objects
        for (int i = 0; i < poolSize; ++i) {
            GenerateNewObject();
        }
    }

    /// <summary>
    /// Get an object from the pool
    /// </summary>
    /// <returns>Requested GameObject</returns>
    public GameObject GetObject() {
        if (totalFree == 0 && !expandable) return null;
        else if (totalFree == 0) GenerateNewObject();

        GameObject g = freeList[totalFree - 1];
        freeList.RemoveAt(totalFree - 1);
        usedList.Add(g);
		totalFree--;
        return g;
    }

    /// <summary>
    /// Return an object to the pool
    /// </summary>
    /// <param name="obj">Gameobject to return</param>
    public void ReturnObject(GameObject obj) {
        Debug.Assert(usedList.Contains(obj));
        obj.SetActive(false);
        usedList.Remove(obj);
        freeList.Add(obj);
		totalFree++;
    }

    /// <summary>
    /// Instantiate GameObject Prefab
    /// </summary>
    private void GenerateNewObject() {
        GameObject g = Instantiate(prefab);
        g.transform.parent = transform;
        g.SetActive(false);
        freeList.Add(g);
		totalFree++;
    }
}
