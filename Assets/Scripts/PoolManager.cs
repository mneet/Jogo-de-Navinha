using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public GameObject preFab;
    public int poolSize = 0;

    private List<GameObject> objectPool;

    void Awake() {
        objectPool = new List<GameObject>();

        for (int i = 0; i < poolSize; i++) {
            GameObject obj = Instantiate(preFab);
            obj.SetActive(false);
            objectPool.Add(obj);
        }
    }

    public GameObject GetPoolObject() {
        foreach (GameObject obj in objectPool) {
            if (!obj.gameObject.activeInHierarchy) {
                return obj;
            }
        }

        GameObject newObj = Instantiate(preFab);
        newObj.SetActive(true);
        objectPool.Add(newObj);
        return newObj;
    }
}
