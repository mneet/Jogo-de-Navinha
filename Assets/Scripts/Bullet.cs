using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    [SerializeField] string targetTag;
    [SerializeField] GameObject fragmentPreFab;

    public int bulletLevel = 0;
    public float speed = 15.0f;
    public Vector3 direction = Vector3.forward;
    public float damage = 2f;


    private void CheckOutOfScreen() {
        if (transform.position.z < -30f || transform.position.z >  30f || transform.position.x < -30f || transform.position.x > 30f) {
            Destroy(gameObject);
        }
    }

    private void ExplodeBullet() {
        GameObject bullet1 = Instantiate(fragmentPreFab, transform.position, transform.rotation);
        GameObject bullet2 = Instantiate(fragmentPreFab, transform.position, transform.rotation);
        GameObject bullet3 = Instantiate(fragmentPreFab, transform.position, transform.rotation);

        bullet1.GetComponent<Bullet>().direction = Vector3.forward;
        bullet1.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        bullet2.GetComponent<Bullet>().direction = Vector3.right;
        bullet2.transform.rotation = Quaternion.LookRotation(Vector3.right, Vector3.up);
        bullet3.GetComponent<Bullet>().direction = Vector3.left;
        bullet3.transform.rotation = Quaternion.LookRotation(Vector3.left, Vector3.up);

    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag(targetTag)) {
            HealthComponent health = other.GetComponent<HealthComponent>();
            if (health != null) {
                health.TakeDamage(damage);
            }
            if (bulletLevel > 3) ExplodeBullet();
            Destroy(gameObject);
        }
        
    }

    void Update() {
        transform.position += (direction * speed) * Time.deltaTime;
        CheckOutOfScreen();
    }

}
