using Pops.Controllers;
using UnityEngine;

public class Reward : MonoBehaviour
{
    public int rewardPoint = 5;

    private void OnTriggerEnter(Collider other)
    {
        CheckCollision(other.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        CheckCollision(collision.gameObject);
    }

    private void CheckCollision(GameObject gameObject)
    {
        if (gameObject.tag.Equals("Player"))
            GameManager.Score += rewardPoint;
    }
}
