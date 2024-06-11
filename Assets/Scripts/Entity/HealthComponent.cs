using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class HealthComponent : MonoBehaviour
{
    public float health = 10f;
    public float healthMax = 10f;

    [SerializeField] private bool immortal = false;
    [SerializeField] private GameObject hitParticles;
    private EntityParticle particleSys;
   
    private GameManager gameManager;
    private bool isPlayer;
    private bool shieldActive = false;
    private HealthComponent parent;

    // Aplica dano ao objeto
    public void TakeDamage(float damage) {

        health -= damage;
        
        // Limita valor da vida
        health = Mathf.Clamp(health, 0, health);

        // Se for o player, atualiza a UI
        if (isPlayer && CompareTag("Player")) {
            gameManager.UpdateHealthUI((int)health);
            particleSys.playParticle();
        }

        // Se vida for igual ou menor a 0 e o objeto não for imortal
        if (health <= 0 && !immortal) {
            // Se for um inimigo
            if (!isPlayer) {

                // Chama particulas
                GameManager.Instance.PlayEnemieHitParticle(transform.position);

                // Atualiza pontuação
                gameManager.ScorePoint(1);

                // Dropa powerup caso o dado esteja dentro do range
                int dice = Random.Range(0, 100);
                Vector3 pos = transform.position;
                if (dice < 20) gameManager.SpawnPowerUp(pos);

                gameObject.GetComponent<AsteroidComponent>().DestroyAsteroid();
                WaveManager.Instance.CheckWaveList(gameObject);
                health = healthMax;
                gameObject.SetActive(false);
                
            }
            else {       
                Destroy(gameObject);
                if (parent != null) {
                    parent.shieldActive = false;
                }
                else {
                    particleSys.destroyFlag = true;
                }              
            }
        }
        
    }

    // Recebe cura
    public void TakeHeal(float heal) {
        health += heal;

        // CAso seja o player, atualiza interface
        if (isPlayer) gameManager.UpdateHealthUI((int)health);
    }

    public void CreateShield(GameObject shield) {
        if (!shieldActive) {
            GameObject shieldObj = Instantiate(shield, this.transform);
            shieldObj.GetComponent<HealthComponent>().parent = this;
        }
    }

    private void Awake() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        isPlayer = CompareTag("Player") || CompareTag("PlayerShield");
        healthMax = health;


        Debug.Log("Object created");
    }

    private void Start()
    {
        if (isPlayer && parent == null) {
            GameObject ptSys = Instantiate(hitParticles, transform.position, Quaternion.identity);
            particleSys = ptSys.GetComponent<EntityParticle>();
            particleSys.parent = gameObject;
            ptSys.SetActive(true);
        }
    }
}
