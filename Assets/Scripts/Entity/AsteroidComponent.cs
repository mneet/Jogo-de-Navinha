using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AsteroidComponent : MonoBehaviour
{
    [SerializeField] int scorePoints = 0;
    [SerializeField] GameObject asteroidToSpawn = null;
    [SerializeField] int asteroidAmount = 0;

    public void DestroyAsteroid()
    {
        if (asteroidToSpawn != null && asteroidAmount > 0)
        {
            for (int i = 0; i < asteroidAmount; i++)
            {
                GameObject asteroid = Instantiate(asteroidToSpawn, transform.position, Quaternion.identity);
                asteroid.transform.position = transform.position;

                WaveManager.Instance.currentWave.mobObjList.Add(asteroid);
                //MovementComponent asteroidMovement = asteroid.GetComponent<MovementComponent>();
            }
        }
    }
}
