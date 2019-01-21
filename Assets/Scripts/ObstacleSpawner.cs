using Pops.Extensions;
using Pops.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Tooltip("The player")]
    public Transform player;

    [Tooltip("Reference objects to be spawn")]
    public GameObject[] references;
    private int[] referId;

    [Tooltip("If this is unchecked spawns references sequentally.")]
    public bool spawnRandomRef = true;

    [Header("Distance metrics")]
    [Tooltip("Minimum gap between obstacles")]
    [Range(1f, 100f)]
    public float minGap = 10f;
    [Tooltip("Maximum gap between obstacles")]
    [Range(1f, 100f)]
    public float maxGap = 30f;

    public float horizonDistance = 120f;

    private Vector3 spawnPosition;

    private void Awake()
    {
        referId = new int[references.Length];
        if (references != null && references.Length > 0)
        {
            for (int i = 0; i < references.Length; i++)
            {
                var pool = ObjectPool.GetOrInitPool(references[i]);
                referId[i] = pool.OriginalObjectID;
            }
        }
        else
        {
            Debug.LogError("There is no reference prefabs attached to ObstacleSpawner!");
        }

        if (player != null)
        {
            spawnPosition = Vector3.zero.With(z: player.position.z + horizonDistance);
        }
        else
        {
            Debug.LogError("There is no player attached to ObstacleSpawner!");
        }
    }

    private void Update()
    {
        if (references != null && references.Length > 0
            && player.DistanceTo(spawnPosition, true) < horizonDistance)
        {
            spawnPosition = spawnPosition.With(z: spawnPosition.z + RndGap());
            ObjectPool.GetInstance(referId[GetNewRefIndex()], spawnPosition, Quaternion.identity);
        }
    }

    private float RndGap()
    {
        return minGap < maxGap ? Random.Range(minGap, maxGap) : minGap;
    }

    protected int GetNewRefIndex()
    {
        if (spawnRandomRef)
        {
            return Random.Range(0, references.Length - 1);
        }
        else
        {
            refIndex = refIndex.HasValue && refIndex.Value < references.Length - 1 ? refIndex.Value + 1 : 0;
            return refIndex.Value;
        }
    }
    private int? refIndex;
}
