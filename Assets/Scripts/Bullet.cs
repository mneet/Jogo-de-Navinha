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

    // Cehca se projetil saiu dos limites da tela
    private void CheckOutOfScreen() {
        // Caso tenha saido dos limites, autodestroi
        if (transform.position.z < -30f || transform.position.z >  30f || transform.position.x < -30f || transform.position.x > 30f) {
            Destroy(gameObject);
        }
    }

    // Efeito de explosão, cria 3 projeteis extras em 3 direções diferentes
    private void ExplodeBullet() {
        // Intancia projeteis
        GameObject bullet1 = Instantiate(fragmentPreFab, transform.position, transform.rotation);
        GameObject bullet2 = Instantiate(fragmentPreFab, transform.position, transform.rotation);
        GameObject bullet3 = Instantiate(fragmentPreFab, transform.position, transform.rotation);

        // Define direção dos projeteis
        bullet1.GetComponent<Bullet>().direction = Vector3.forward;
        bullet1.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        bullet2.GetComponent<Bullet>().direction = Vector3.right;
        bullet2.transform.rotation = Quaternion.LookRotation(Vector3.right, Vector3.up);
        bullet3.GetComponent<Bullet>().direction = Vector3.left;
        bullet3.transform.rotation = Quaternion.LookRotation(Vector3.left, Vector3.up);

    }

    // Checa por colisão com alvo
    private void OnTriggerEnter(Collider other) {
        // Caso colida com o alvo
        if (other.gameObject.CompareTag(targetTag)) {
            // Aplica dano
            HealthComponent health = other.GetComponent<HealthComponent>();
            if (health != null) {
                health.TakeDamage(damage);
            }

            // Ativa efeito caso level seja suficiente
            if (bulletLevel > 3) ExplodeBullet();

            // Se destroi apos colisão
            Destroy(gameObject);
        }
        
    }

    void Update() {
        // Aplica movimento
        transform.position += (direction * speed) * Time.deltaTime;

        // Checa por limites da tela
        CheckOutOfScreen();
    }

}
