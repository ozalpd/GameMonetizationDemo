using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Obstacle Collision Detection
/// </summary>
public class ObstCollisionDetect : MonoBehaviour
{
    private AudioSource audioSource;
    private float audioDuration;
    private ObstacleController obstacle;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioDuration = audioSource != null ? audioSource.clip.length + 1f : 1f;

        obstacle = GetComponent<ObstacleController>();
        if (obstacle == null)
            obstacle = GetComponentInParent<ObstacleController>();
    }


    private void OnTriggerEnter(Collider other)
    {
        CheckCollision(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        CheckCollision(collision.gameObject);
    }

    protected virtual void CheckCollision(GameObject gameObject)
    {
        if (gameObject.tag.Equals("Player"))
        {
            if (audioSource != null)
            {
                audioSource.Play();
            }
            StartCoroutine(Release());
        }
    }

    private IEnumerator Release()
    {
        yield return new WaitForSeconds(audioDuration);
        obstacle.Release();
    }
}
