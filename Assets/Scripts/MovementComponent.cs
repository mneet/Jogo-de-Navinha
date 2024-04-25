using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementComponent : MonoBehaviour
{
    [SerializeField] bool rotateBehaviour = false;
    [SerializeField] Vector3 rotateDirection;
    [SerializeField] float rotationSpeed;
    [SerializeField] float moveSpeed;

    private enum MovementDirection {
        HORIZONTAL,
        VERTICAL,
        RANDOM
    }
    [SerializeField] private MovementDirection movementDirection;

    // Update is called once per frame
    void Update()
    {
        
    }
}
