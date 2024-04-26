using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float maxX;
    [SerializeField] private float minX;
    [SerializeField] private float maxZ;
    [SerializeField] private float minZ;


    public float moveSpeed = 8f;

    private void HandleMovement() {

        Vector3 movementVec = new Vector3();
        Vector3 position = transform.position;

        movementVec.x = Input.GetAxis("Horizontal");
        movementVec.z = Input.GetAxis("Vertical");

        if (movementVec.magnitude > 1) {
            movementVec.Normalize();
        }

        position += movementVec * moveSpeed * Time.deltaTime;

        // Limitar movimento dentro da camera
        float xMinLimit = minX;
        float xMaxLimit = maxX;
        float zMinLimit = minZ;
        float zMaxLimit = maxZ;

        position.x = Mathf.Clamp(position.x, xMinLimit, xMaxLimit);
        position.z = Mathf.Clamp(position.z, zMinLimit, zMaxLimit);

        transform.position = position;
    }
    // Update is called once per frame
    void Update() {
        HandleMovement();
    }
}
