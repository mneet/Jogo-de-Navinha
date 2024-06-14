using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class HealthComponent : MonoBehaviour
{
    // Vida atual e maxima
    public float health = 10f;
    public float healthMax = 10f;

    // Quantidade de pontos ao ser destruido
    public int scorePoints = 1;

    [SerializeField] private bool immortal = false;

    // Sistema de particulas do Player
    [SerializeField] private GameObject hitParticles;
    private EntityParticle particleSys;
   
    private GameManager gameManager;

    // Flags se é controlado pelo player e shield ativo
    private bool isPlayer;
    private bool shieldActive = false;

    // Parent do shield
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

        // Audio de hit
        AudioController.Instance.PlaySfx(1);

        // Se vida for igual ou menor a 0 e o objeto não for imortal
        if (health <= 0 && !immortal) {
            // Se for um inimigo
            if (!isPlayer) {

                // Chama particulas
                GameManager.Instance.PlayEnemieHitParticle(transform.position);

                // Atualiza pontuação
                gameManager.ScorePoint(scorePoints);

                // Dropa powerup caso o dado esteja dentro do range
                int dice = Random.Range(0, 100);
                Vector3 pos = transform.position;
                if (dice < 20) gameManager.SpawnPowerUp(pos);

                AsteroidComponent asteroid =  gameObject.GetComponent<AsteroidComponent>();
                if (asteroid != null) asteroid.DestroyAsteroid();

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
        health = Mathf.Min(health, healthMax);

        // CAso seja o player, atualiza interface
        if (isPlayer) gameManager.UpdateHealthUI((int)health);
    }

    // Cria shield em volta do player
    public void CreateShield(GameObject shield) {
        // Checa se ja não existe um shield ativo
        if (!shieldActive) {
            GameObject shieldObj = Instantiate(shield, this.transform);
            shieldObj.GetComponent<HealthComponent>().parent = this;
        }
    }

    private void Awake() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        isPlayer = CompareTag("Player") || CompareTag("PlayerShield");
        healthMax = health;
    }

    private void Start()
    {
        // Instancia sistema de particulas apenas apra o player
        if (isPlayer && parent == null) {
            GameObject ptSys = Instantiate(hitParticles, transform.position, Quaternion.identity);
            particleSys = ptSys.GetComponent<EntityParticle>();
            particleSys.parent = gameObject;
            ptSys.SetActive(true);
        }
    }
}
