using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AsteroidComponent : MonoBehaviour
{
    [SerializeField] int scorePoints = 0;
    [SerializeField] WaveManager.AsteroidTypes asteroidToSpawn;
    [SerializeField] int asteroidAmount = 0;

    public void DestroyAsteroid()
    {
        if (asteroidToSpawn != null && asteroidAmount > 0)
        {

            for (int i = 0; i < asteroidAmount; i++)
            {
                GameObject asteroid = GameManager.Instance.GetRandomEnemy(asteroidToSpawn);
                asteroid.SetActive(true);
                asteroid.transform.position = transform.position;

                WaveManager.Instance.currentWave.mobObjList.Add(asteroid);
                //MovementComponent asteroidMovement = asteroid.GetComponent<MovementComponent>();
            }
        }
    }
}
