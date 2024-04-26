using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class HealthComponent : MonoBehaviour
{
    [SerializeField] private float health = 10f;
    [SerializeField] private bool immortal = false;

   
    private GameManager gameManager;
    private bool isPlayer;
    public void TakeDamage(float damage) {
        health -= damage;
        health = Mathf.Clamp(health, 0, health);
        if (isPlayer) gameManager.UpdateHealthUI((int)health);
        if (health <= 0 && !immortal) {
            if (!isPlayer) {
                MovementComponent movement = GetComponent<MovementComponent>();
                
                health = 2;
                gameManager.ScorePoint(1);

                int dice = Random.Range(0, 100);
                Vector3 pos = transform.position;
                if (dice < 20) gameManager.SpawnPowerUp(pos);

                movement.RandomizeMovementDirection();
            }
            else {           
                Destroy(gameObject);             
            }
        }
        
    }

    public void TakeHeal(float heal) {
        health += heal;
        if (isPlayer) gameManager.UpdateHealthUI((int)health);
    }

    private void Awake() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        isPlayer = CompareTag("Player");
    }

    private void Start() {
        //if (isPlayer) gameManager.UpdateHealthUI((int)health);
    }

    // Update is called once per frame
    void Update() {

    }
}
