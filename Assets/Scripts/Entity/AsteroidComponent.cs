using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AsteroidComponent : MonoBehaviour
{
    // Asteroide que deve ser spawnado ao morrer
    [SerializeField] WaveManager.AsteroidTypes asteroidToSpawn;
    // Quantidade de asteroides a ser spawnado
    [SerializeField] int asteroidAmount = 0;

    // Metodo de destruição do asteroide
    public void DestroyAsteroid()
    {
        // Caso deva spawnar um asteroide 
        if (asteroidToSpawn != null && asteroidAmount > 0)
        {
            // Spawna a quantidade de asteroides definida
            for (int i = 0; i < asteroidAmount; i++)
            {
                // Retorna asteroide da pool correspondente
                GameObject asteroid = GameManager.Instance.GetRandomEnemy(asteroidToSpawn);
                asteroid.SetActive(true);

                // Seta posição do asteroide gerado
                asteroid.transform.position = transform.position;

                // Adiciona asteroide gerado a wave
                WaveManager.Instance.currentWave.mobObjList.Add(asteroid);
            }
        }
    }
}
