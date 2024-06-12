using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShooterComponent : MonoBehaviour {

    // Flag de controlado pelo player
    [SerializeField] bool playerControlled = false;

    // Prefab do projetil
    [SerializeField] private GameObject bulletPreFab;

    // Alvo inimigo
    private GameObject target;

    // Timers e cooldowns
    public float fireRate = 0.5f;
    private float fireCooldown = 0f;
    
    // Nível do projetil
    public int bulletLevel  = 1;

    // Istancia projetil
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

            // Audio de tiro
            AudioController.Instance.PlaySfx(0);

            // Reseta o cooldown
            fireCooldown = fireRate;
        }

    }

    // Tiro controlado pelo player
    public void PlayerShooterControl() {
        // Coleta input e executa metodo
        if (Input.GetKey(KeyCode.Space) || Input.GetMouseButton(0)) {
            ShootBullet();
        }
    }

    private void Awake() {
        // caso pertença a um inimigo
        if (!playerControlled) {
            // Define o target como o player e cooldown de tiro aleatorio
            target = GameObject.Find("Player");
            fireCooldown = Random.Range(0, fireCooldown);
        }
    }

        
    void Update() {
        // Diminui cooldown de tiro
        if (fireCooldown > 0) fireCooldown -= Time.deltaTime;

        // Chama metodos de controle de tiro 
        if (!playerControlled) { // Se não é controlado pelo player
            ShootBullet();
        }
        else { // Se é controlado pelo player
            PlayerShooterControl();
        }
    }
}
