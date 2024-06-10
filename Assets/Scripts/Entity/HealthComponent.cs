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

    // Aplica dano ao objeto
    public void TakeDamage(float damage) {

        health -= damage;
        
        // Limita valor da vida
        health = Mathf.Clamp(health, 0, health);

        // Se for o player, atualiza a UI
        if (isPlayer) gameManager.UpdateHealthUI((int)health);

        

        // Se vida for igual ou menor a 0 e o objeto n�o for imortal
        if (health <= 0 && !immortal) {
            // Se for um inimigo
            if (!isPlayer) {

                // Chama particulas
                GameManager.Instance.PlayEnemieHitParticle(transform.position);

                // Atualiza pontua��o
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
            }
        }
        
    }

    // Recebe cura
    public void TakeHeal(float heal) {
        health += heal;

        // CAso seja o player, atualiza interface
        if (isPlayer) gameManager.UpdateHealthUI((int)health);
    }

    private void Awake() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        isPlayer = CompareTag("Player");
        healthMax = health;
    }

    private void Start()
    {
        //particleSys = Instantiate(hitParticles, transform.position, Quaternion.identity).GetComponent<EntityParticle>();
        //particleSys.parent = gameObject;
    }

    // Update is called once per frame
    void Update() {

    }
}
