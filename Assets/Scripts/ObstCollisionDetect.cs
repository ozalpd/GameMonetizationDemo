using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Obstacle Collision Detection
/// </summary>
public abstract class ObstCollisionDetect : MonoBehaviour
{
    public float releaseLatency = 2f;

    private AudioSource audioSource;
    private float audioDuration;
    private ObstacleController obstacle;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioDuration = audioSource != null ? audioSource.clip.length + releaseLatency : releaseLatency;

        obstacle = GetComponent<ObstacleController>();
        if (obstacle == null)
            obstacle = GetComponentInParent<ObstacleController>();
    }


    protected virtual void OnTriggerEnter(Collider other)
    {
        CheckCollision(other.gameObject);
    }

    protected virtual void OnCollisionEnter(Collision collision)
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

            OnPlayerCollision(gameObject.GetComponent<PlayerController>());
            StartCoroutine(Release());
        }
    }
    public abstract void OnPlayerCollision(PlayerController player);

    protected IEnumerator Release()
    {
        yield return new WaitForSeconds(audioDuration);
        obstacle.Release();
    }
}
