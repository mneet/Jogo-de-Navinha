using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterComponent : MonoBehaviour {
    [SerializeField] bool playerControlled = false;
    [SerializeField] private GameObject bulletPreFab;
    private GameObject target;

    [SerializeField] private float fireRate = 0.5f;
    private float fireCooldown = 0f;

    public void ShootBullet() {

        fireCooldown -= Time.deltaTime;

        if (fireCooldown <= 0f) {
            Vector3 position = transform.position;
            Quaternion rotation = transform.rotation;

            GameObject bullet = Instantiate(bulletPreFab, position, rotation);
            bullet.GetComponent<Bullet>().parent = gameObject;
            bullet.GetComponent<Bullet>().parentTag = tag;
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
        if (!playerControlled) {
            ShootBullet();
        }
        else {
            PlayerShooterControl();
        }
    }
}
