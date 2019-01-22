using Pops.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 500f;
    public float horztSpeed = 20f;

    private Rigidbody rbody;
    private Camera cameraMain;

    private void Start()
    {
        rbody = GetComponent<Rigidbody>();
        cameraMain = Camera.main;
        if (cameraMain == null)
        {
            Debug.LogError("Main camera could not find! Please set tag of a camera as 'MainCamera'.");
        }
    }

    private void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
        inputH = Input.GetAxis("Horizontal");
        xPos = Mathf.Clamp(transform.position.x + inputH, -2.5f, 2.5f);
#endif
        transform.position = Vector3.Lerp(transform.position, transform.position.With(x: xPos), horztSpeed);
        rbody.velocity = Vector3.forward * speed * Time.deltaTime;
    }
    private float inputH, xPos;

    //public float DistanceTo(Vector3 destination, bool ignoreY = false)
    //{
    //    return transform.DistanceTo(destination, ignoreY);
    //}
}
