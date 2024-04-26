using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private Transform followTarget;
    private void Update() {
        transform.position += new Vector3(0f, 0f, moveSpeed) * Time.deltaTime;
    }
    private void LateUpdate() {
        if (transform.position.z <= -35f) {
            transform.position = new Vector3(0f, -1f, followTarget.position.z + 50f);
        }
    }
}
