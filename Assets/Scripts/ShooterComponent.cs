using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterComponent : MonoBehaviour {
    [SerializeField] bool playerControlled = false;
    [SerializeField] private GameObject bulletPreFab;
    private GameObject target;

    public float fireRate = 0.5f;
    private float fireCooldown = 0f;
    public int bulletLevel  = 1;

    public void ShootBullet() {

        if (fireCooldown <= 0f) {
            Vector3 position = transform.position;
            Quaternion rotation = transform.rotation;
            int bulletAmnt = bulletLevel;
            bulletAmnt = Mathf.Clamp(bulletAmnt, 1, 3);
            for (var i = 0; i < bulletAmnt; i++) {
                Vector3 newPos = position; 
                float sep = 0.8f;
                float width = sep * bulletAmnt;
                newPos.x = position.x - width / 2 + sep * i;
                GameObject bullet = Instantiate(bulletPreFab, newPos, rotation);
                bullet.GetComponent<Bullet>().bulletLevel = bulletLevel;
            }
            
            fireCooldown = fireRate;
        }

    }
    public void SetBulletDirection(GameObject bullet) {

        Vector2 parentDirection = gameObject.GetComponent<MovementComponent>().movementDirectionVector;
        Vector3 newDirection = new Vector3(parentDirection.x, 0, parentDirection.y);
        bullet.GetComponent<Bullet>().direction = newDirection;

        Quaternion rotation = Quaternion.LookRotation(newDirection, Vector3.up);
        bullet.transform.rotation = rotation;

    }

    public void PlayerShooterControl() {
        if (Input.GetKey(KeyCode.Space)) {
            ShootBullet();
        }
    }

    private void Awake() {
        if (!playerControlled) {
            target = GameObject.Find("Player");
            fireCooldown = Random.Range(0, fireCooldown);
        }
    }
    // Update is called once per frame
    void Update() {
        if (fireCooldown > 0) fireCooldown -= Time.deltaTime;
        if (!playerControlled) {
            ShootBullet();
        }
        else {
            PlayerShooterControl();
        }
    }
}
