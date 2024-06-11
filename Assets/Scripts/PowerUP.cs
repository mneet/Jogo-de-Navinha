using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PowerUP : MonoBehaviour {
    private enum Powers { 
        HEAL,
        BULLET,
        FIRE,
        SPEED,
        SHIELD
    }
    [SerializeField] private Powers power;
    [SerializeField] private float heal;
    [SerializeField] private float timerTotal;
    [SerializeField] private GameObject Shield;
    private float currentTimer;

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            Debug.Log("destroy");
            switch (power) {
                case Powers.HEAL: Heal(other.gameObject); break;
                case Powers.FIRE: UpgradeFire(other.gameObject); break;
                case Powers.BULLET: UpgradeBullet(other.gameObject);break;
                case Powers.SPEED: UpgradeSpeed(other.gameObject); break;
                case Powers.SHIELD: UpgradeShield(other.gameObject); break;
                    
            }
            Destroy(gameObject);
        }
    }

    private void UpgradeSpeed(GameObject other) {
        PlayerMovement movement = other.GetComponent<PlayerMovement>();
        movement.moveSpeed += 1f;
        movement.moveSpeed = Mathf.Clamp(movement.moveSpeed, 8f, 11f);
    }
    private void UpgradeShield(GameObject other) {
        other.GetComponent<HealthComponent>().CreateShield(Shield);
    }
    private void Heal(GameObject other) {
        other.GetComponent<HealthComponent>().TakeHeal(heal);
    }
    private void UpgradeBullet(GameObject other) {
        other.GetComponent<ShooterComponent>().bulletLevel++;
    }
    private void UpgradeFire(GameObject other) {
        ShooterComponent shooter = other.GetComponent<ShooterComponent>();
        shooter.fireRate -= 0.1f;
        shooter.fireRate = Mathf.Clamp(shooter.fireRate, 0.2f, 0.5f);
    }
    private void RotateBody() {
        transform.Rotate(new Vector3(0f,1f,0f) * 100f * Time.deltaTime);
    }

    private void Awake() {
        currentTimer = timerTotal;
    }

    private void Update() {
        currentTimer -= Time.deltaTime;
        if (currentTimer <= 0) {
            Destroy(gameObject);
        }
        RotateBody();

    }
}
