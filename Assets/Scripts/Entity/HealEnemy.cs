using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealEnemy : MonoBehaviour
{
    public GameObject boss;
    [SerializeField] private float movementSpeed;
    [SerializeField] private int heal;
    [SerializeField] private Vector3 direction = new Vector3();

    private void moveTowardsBoss() {
        direction = (boss.transform.position - transform.position);
        direction.Normalize();
        transform.position += direction * (movementSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag(gameObject.tag) && other.name != gameObject.name) {
            other.GetComponent<HealthComponent>().TakeHeal(heal);
            Destroy(gameObject);
        }
    }

    private void Update() {
        moveTowardsBoss();
    }
}
