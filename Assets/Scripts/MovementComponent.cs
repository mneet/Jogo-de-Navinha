using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MovementComponent : MonoBehaviour
{

    [SerializeField] private float maxX = 14f;
    [SerializeField] private float minX = -14f;
    [SerializeField] private float maxZ = 7.2f;
    [SerializeField] private float minZ = -7.7f;
    public float movementSpeed = 7f;
    [SerializeField] private bool rotate = false;
    [SerializeField] private float rotateSpeed = 5f;
    [SerializeField] private Vector3 rotateDirection;
    private GameObject target;

    public enum MovementBehaviour {
        RANDOM,
        TARGET,
        DOWN
    }
    [SerializeField] private MovementBehaviour movementBehaviour;
    private enum MovementDirection {
        RIGHT,
        LEFT,
        DOWN
    }
    [SerializeField] private MovementDirection movementDirection;
    public Vector2 movementDirectionVector;
    public void RandomizeMovementDirection() {
        switch (movementBehaviour) {
            case MovementBehaviour.DOWN:
                RandomizeDownDirection();
                break;
            case MovementBehaviour.RANDOM:
                RandomizeDirection();
                break;
            case MovementBehaviour.TARGET:
                RandomizeTargetDirection();
                break;
        }
    }
    private void RandomizeDirection() {
        MovementDirection direction = GetRandomDirection();
        movementDirection = direction;
        movementDirectionVector = new Vector2(0, 0);

        Vector2 newPosition = new Vector2(0, 0);
        switch (movementDirection) {
            case MovementDirection.RIGHT:
                movementDirectionVector.x = 1;
                newPosition.x = UnityEngine.Random.Range(minX - 4, minX);
                newPosition.y = UnityEngine.Random.Range(minZ + 4, maxZ - 4);
                break;
            case MovementDirection.LEFT:
                movementDirectionVector.x = -1;
                newPosition.x = UnityEngine.Random.Range(maxX, maxX + 4);
                newPosition.y = UnityEngine.Random.Range(minZ + 4, maxZ - 4);
                break;
            case MovementDirection.DOWN:
                movementDirectionVector.y = -1;
                newPosition.x = UnityEngine.Random.Range(minX + 4, maxX - 4);
                newPosition.y = UnityEngine.Random.Range(maxZ, maxZ + 4);
                break;
        }
        ApplyPosition(newPosition);
    }
    private void RandomizeTargetDirection() {
        RandomizeDirection();
        if (target != null) {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            movementDirectionVector.x = direction.x;
            movementDirectionVector.y = direction.z;
        }
    }
    private void RandomizeDownDirection() {
        MovementDirection direction = MovementDirection.DOWN;
        movementDirection = direction;
        movementDirectionVector = new Vector2(0, 0);

        Vector2 newPosition = new Vector2(0, 0);
        movementDirectionVector.y = -1;
        newPosition.x = UnityEngine.Random.Range(minX + 2, maxX - 2);
        newPosition.y = UnityEngine.Random.Range(maxZ, maxZ + 10);

        ApplyPosition(newPosition);
    }
    private void ApplyPosition(Vector2 newPosition) {

        Vector3 objectPosition = new Vector3();
        objectPosition.x = newPosition.x;
        objectPosition.z = newPosition.y;
        transform.position = objectPosition;
    }
    private MovementDirection GetRandomDirection() {
        // Obtém todos os valores do enum como um array
        Array values = Enum.GetValues(typeof(MovementDirection));

        // Gera um índice aleatório entre 0 e o comprimento do array
        int randomIndex = UnityEngine.Random.Range(0, values.Length);

        // Retorna o valor correspondente ao índice aleatório
        return (MovementDirection)values.GetValue(randomIndex);
    }
    private void BasicStraightMovement() {
        Vector3 movDir = new Vector3(0, 0, 0);

        movDir = new Vector3(movementDirectionVector.x, 0, movementDirectionVector.y);

        transform.position += movDir * movementSpeed * Time.deltaTime;
    }
    private void CheckOutOfBorder() {
        bool xLimit = transform.position.x < minX - 12 || transform.position.x > maxX + 12;
        bool zLimit = transform.position.z < minZ - 12 || transform.position.z > maxZ + 12;

        if (xLimit || zLimit) RandomizeMovementDirection();
    }
    private void RotateBody() {
        transform.Rotate(rotateDirection * rotateSpeed * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Player")){
            HealthComponent health = other.GetComponent<HealthComponent>();
            if (health != null) {
                health.TakeDamage(1);
            }
            RandomizeMovementDirection();
        }
    }
    private void Awake() {
        target = GameObject.Find("Player");
        RandomizeMovementDirection();
    }
    void Update() {
        BasicStraightMovement();
        CheckOutOfBorder();

        if (rotate) RotateBody();
    }
}


