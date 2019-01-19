using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public GameObject target;
    public float speed = 100f;
    private Vector3 offset;

    private void Start()
    {
        if (target != null)
        {
            offset = transform.position - target.transform.position;
        }
        else
        {
            Debug.LogError("Target was not set! Please a the target GameObject for to follow.");
        }
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            transform.position = Vector3.Lerp(transform.position, offset + target.transform.position, speed * Time.deltaTime);
        }
    }
}
