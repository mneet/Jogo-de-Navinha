using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MovementComponent : MonoBehaviour
{
    // Limites da tela
    [SerializeField] private float maxX = 14f;
    [SerializeField] private float minX = -14f;
    [SerializeField] private float maxZ = 7.2f;
    [SerializeField] private float minZ = -7.7f;

    // Velocidade de movimento
    public float movementSpeed = 7f;

    // Flag se deve rotacionar
    [SerializeField] private bool rotate = false;
    // Velocidade de rotação
    [SerializeField] private float rotateSpeed = 5f;
    // Eixos que deve rotacionar
    [SerializeField] private Vector3 rotateDirection;

    // ZIGZAG
    private float invertTimerMax = 1.2f;
    private float invertTimer = 1.2f;

    // Enumerador com possibilidades de direções de movimento
    private enum MovementDirection {
        RIGHT = 0,
        LEFT = 1,
        UP = 2,
        DOWN = 3,
        LENGTH = 4
    }
    [SerializeField] private MovementDirection movementDirection;
    public Vector2 movementDirectionVector;

    // Padrões de movimentação
    private enum MovementPatterns {
        STRAIGHT = 0,
        ZIGZAG = 1,
        LENGTH = 2
    }
    [SerializeField] private MovementPatterns movementPattern;

    // Returna um valor aleatorio do enumerador MovementDirection
    private MovementDirection GetRandomDirection() {

        // Gera um índice aleatório entre 0 e o comprimento do array
        int randomIndex = UnityEngine.Random.Range(0, (int)MovementDirection.LENGTH);

        // Retorna o valor correspondente ao índice aleatório
        return (MovementDirection)randomIndex;
    }

    // Define direção de movimento aleatoria para o objeto
    public void RandomizeDirection() {

        // Chama metodo para gerar direção aleatoria
        movementDirection = GetRandomDirection();
        movementDirectionVector = new Vector2(0, 0);
        
        // Variavel da nova posição do objeto
        Vector2 newPosition = new Vector2(0, 0);

        // Gera uma posição aleatoria com base na direção gerada
        // Define vetor de direção para calculo de movimentação
        switch (movementDirection) {
            case MovementDirection.RIGHT:
                movementDirectionVector.x = 1;
                movementDirectionVector.y = UnityEngine.Random.Range(0f, 1f);
                newPosition.x = UnityEngine.Random.Range(minX - 2, minX);
                newPosition.y = UnityEngine.Random.Range(minZ + 2, maxZ - 2);
                break;
            case MovementDirection.LEFT:
                movementDirectionVector.x = -1;
                movementDirectionVector.y = UnityEngine.Random.Range(0f, 1f);
                newPosition.x = UnityEngine.Random.Range(maxX, maxX + 2);
                newPosition.y = UnityEngine.Random.Range(minZ + 2, maxZ - 2);
                break;
            case MovementDirection.DOWN:
                movementDirectionVector.y = -1;
                movementDirectionVector.x = UnityEngine.Random.Range(-1f, 0f);
                newPosition.x = UnityEngine.Random.Range(minX + 2, maxX - 2);
                newPosition.y = UnityEngine.Random.Range(maxZ, maxZ + 2);
                break;
            case MovementDirection.UP:
                movementDirectionVector.y = 1;
                movementDirectionVector.x = UnityEngine.Random.Range(-1f, 0f);
                newPosition.x = UnityEngine.Random.Range(minX + 2, maxX - 2);
                newPosition.y = UnityEngine.Random.Range(minZ + 2, minZ - 2);
                break;
        }
        movementDirectionVector.Normalize();
        
        // Aplica posição ao objeto
        ApplyPosition(newPosition);
    }
    
    // Aplica posição recebida ao objeto
    private void ApplyPosition(Vector2 newPosition) {

        // Converte Vector2 recebido para Vector3 e aplica ao objeto
        Vector3 objectPosition = new Vector3();
        objectPosition.x = newPosition.x;
        objectPosition.z = newPosition.y;
        transform.position = objectPosition;
    }
    
    // Movimentação em linha reta 
    private void BasicStraightMovement() {
        Vector3 movDir = new Vector3(0, 0, 0);

        movDir = new Vector3(movementDirectionVector.x, 0, movementDirectionVector.y);

        transform.position += movDir * movementSpeed * Time.deltaTime;
    }
    
    // Movimentação em ZigZag
    private void ZigZagMovement() {

        invertTimer -= Time.deltaTime;
        Vector3 movDir = new Vector3(movementDirectionVector.x, 0, movementDirectionVector.y);
        if (invertTimer <= 0) {
            switch (movementDirection) {
                case MovementDirection.UP:
                case MovementDirection.DOWN:
                    movementDirectionVector.x *= -1f;
                    break;
                case MovementDirection.LEFT:
                case MovementDirection.RIGHT:
                    movementDirectionVector.y *= -1f;
                    break;
            }
            invertTimer = invertTimerMax;
        }

        transform.position += movDir * movementSpeed * Time.deltaTime;
    }

    // Checa se objeto ultrapassou os limites da tela
    private void CheckOutOfBorder() {
        bool xLimit = transform.position.x < minX - 12 || transform.position.x > maxX + 12;
        bool zLimit = transform.position.z < minZ - 12 || transform.position.z > maxZ + 12;

        // Caso tenha ultrapassado os limties, gera nova posição e direção aleatoria
        if (xLimit || zLimit) RandomizeDirection();
    }
   
    // Rotaciona o objeto em torno do eixo definido
    private void RotateBody() {
        transform.Rotate(rotateDirection * rotateSpeed * Time.deltaTime);
    }
   
    // Checa por colisão do Player com o objeto
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("PlayerShield")){
            // Caso tenha colidido, aplica dano ao Player
            HealthComponent health = other.GetComponent<HealthComponent>();
            if (health != null) {
                health.TakeDamage(1);
            }
        }
    }
    
    private void Awake() {
        // Define posição e direção aleatoria ao iniciar
        RandomizeDirection();
    }
    
    void Update() {

        // Executa metodo de movimento com base do padrão selecionado
        switch (movementPattern) {
            case MovementPatterns.STRAIGHT:
                BasicStraightMovement();
                break;
            case MovementPatterns.ZIGZAG:
                ZigZagMovement(); 
                break;
        }
        // Checa se o objeto saiu dos limites da tela
        CheckOutOfBorder();

        // Rotaciona objeto caso ativado
        if (rotate) RotateBody();
    }
}


