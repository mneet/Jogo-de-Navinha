using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityParticle : MonoBehaviour
{
    public bool destroyFlag;
    public GameObject parent = null;

    public void playParticle()
    {
        gameObject.GetComponent<ParticleSystem>().Play();
    }

    private void checkFlag()
    {
        if (destroyFlag && !gameObject.GetComponent<ParticleSystem>().isPlaying)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        checkFlag();

        if (parent != null) transform.position = parent.transform.position;
    }
}
