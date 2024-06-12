using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    // Alvo
    [SerializeField] string targetTag;

    // Prefab dos fragmentos
    [SerializeField] GameObject fragmentPreFab;

    // N�vel do projetil
    public int bulletLevel = 0;

    // Velocidade de movimento
    public float speed = 15.0f;
    // Dire��o 
    public Vector3 direction = Vector3.forward;

    // Dano
    public float damage = 2f;

    // Checa se projetil saiu dos limites da tela
    private void CheckOutOfScreen() {
        // Caso tenha saido dos limites, autodestroi
        if (transform.position.z < -30f || transform.position.z >  30f || transform.position.x < -30f || transform.position.x > 30f) {
            Destroy(gameObject);
        }
    }

    // Efeito de explos�o, cria 3 projeteis extras em 3 dire��es diferentes
    private void ExplodeBullet() {
        // Intancia projeteis
        GameObject bullet1 = Instantiate(fragmentPreFab, transform.position, transform.rotation);
        GameObject bullet2 = Instantiate(fragmentPreFab, transform.position, transform.rotation);
        GameObject bullet3 = Instantiate(fragmentPreFab, transform.position, transform.rotation);

        // Define dire��o dos projeteis
        bullet1.GetComponent<Bullet>().direction = Vector3.forward;
        bullet1.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        bullet2.GetComponent<Bullet>().direction = Vector3.right;
        bullet2.transform.rotation = Quaternion.LookRotation(Vector3.right, Vector3.up);
        bullet3.GetComponent<Bullet>().direction = Vector3.left;
        bullet3.transform.rotation = Quaternion.LookRotation(Vector3.left, Vector3.up);

    }

    // Checa por colis�o com alvo
    private void OnTriggerEnter(Collider other) {
        // Caso colida com o alvo
        if (other.gameObject.CompareTag(targetTag) || (targetTag == "Player" && other.gameObject.CompareTag("PlayerShield"))) {
            // Aplica dano
            HealthComponent health = other.GetComponent<HealthComponent>();
            if (health != null) {
                health.TakeDamage(damage);
            }

            // Ativa efeito caso level seja suficiente
            if (bulletLevel > 3) ExplodeBullet();

            // Se destroi apos colis�o
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
