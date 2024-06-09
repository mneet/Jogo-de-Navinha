using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShooterComponent : MonoBehaviour {
    [SerializeField] bool playerControlled = false;
    [SerializeField] private GameObject bulletPreFab;
    private GameObject target;

    public float fireRate = 0.5f;
    private float fireCooldown = 0f;
    public int bulletLevel  = 1;

    public void ShootBullet() {

        // Se cooldown estiver zerado
        if (fireCooldown <= 0f) {

            // Coleta posição e rotação do objeto
            Vector3 position = transform.position;
            Quaternion rotation = transform.rotation;

            // Define quantidade de projeteis com base no level 
            int bulletAmnt = bulletLevel;

            // Limita a quantidade de projeteis dentro de um range
            bulletAmnt = Mathf.Clamp(bulletAmnt, 1, 3);

            // Loop de repetição com base na quantidade de projeteis
            for (var i = 0; i < bulletAmnt; i++) {

                // Nova variavel de posição
                Vector3 newPos = position; 

                // Separação entre os projeteis
                float sep = 0.8f;
                // Tamanho horizontal dos projetos alinhados
                float width = sep * bulletAmnt;

                // Posição do projeto sendo instanciado
                newPos.x = position.x - width / 2 + sep * i;

                // Intancia projetil  e define level do mesmo
                GameObject bullet = Instantiate(bulletPreFab, newPos, rotation);
                bullet.GetComponent<Bullet>().bulletLevel = bulletLevel;

                // Rotaciona projetil com base no objeto que o criou
                bullet.GetComponent<Bullet>().direction = transform.forward;
            }
            
            // Reseta o cooldown
            fireCooldown = fireRate;
        }

    }

    public void PlayerShooterControl() {
        // Coleta input e executa metodo
        if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) {
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
