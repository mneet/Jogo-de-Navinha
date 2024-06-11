using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityParticle : MonoBehaviour
{
    public bool destroyFlag;
    public GameObject parent = null;

    public void playParticle()
    {
        Debug.Log("Playing");
        gameObject.GetComponent<ParticleSystem>().Play();
    }

    private void checkFlag()
    {
        if (!gameObject.GetComponent<ParticleSystem>().isPlaying && parent == null)
        {
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        checkFlag();
        if (parent != null) {
            transform.position = parent.transform.position;
        }
    }
}
