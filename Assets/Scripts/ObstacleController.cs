using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    public PlayerController Player { get; set; }
    public bool Releasing { get; set; }

    public void Release()
    {
        gameObject.SetActive(false);
    }
}
