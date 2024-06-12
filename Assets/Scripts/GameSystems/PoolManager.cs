using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    // Objeto a ser armazenado na pool
    public GameObject preFab;

    // Tamnaho inicial da pool
    public int poolSize = 0;

    // Pool
    private List<GameObject> objectPool;

    void Awake() {

        // Gera lista da pool inicial e popula com os objetos
        objectPool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++) {
            GameObject obj = Instantiate(preFab);
            obj.SetActive(false);
            objectPool.Add(obj);
        }
    }

    // Retorna um objeto da pool
    public GameObject GetPoolObject() {

        // Checa se existe algum objeto na pool desativado, caso sim o retorna
        foreach (GameObject obj in objectPool) {
            if (!obj.gameObject.activeInHierarchy) {
                return obj;
            }
        }

        // Caso não exista nenhum objeto na pool, instancia outro objeto e o adiciona na pool
        GameObject newObj = Instantiate(preFab);
        objectPool.Add(newObj);
        return newObj;
    }
}
