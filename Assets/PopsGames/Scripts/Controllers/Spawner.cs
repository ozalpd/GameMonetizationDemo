using Pops.Extensions;
using Pops.Helpers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pops.Controllers
{
    public class Spawner : MonoBehaviour
    {
        [Tooltip("Wait before start spawning.")]
        public float delayIn = 3;

        [Tooltip("If Spawner has an animation, delay for animation out.")]
        public float animOutDelay = 3;

        [Header("Spawn Objects")]
        [Tooltip("If this is unchecked spawns references sequentally.")]
        public bool spawnRandomRef = true;

        [Tooltip("Number of objects to be spawn in each wave.")]
        public int spawnNumber = 20;
        private int remain;

        [Tooltip("Number of spawn waves.")]
        public int waveNumber = 10;

        [Tooltip("Wait between waves.")]
        [Range(1f, 180f)]
        public float waveDelay = 5f;

        [Tooltip("Shortest wait between two spawns")]
        [Range(0.05f, 180f)]
        public float minSpawnDelay = 0.25f;

        [Tooltip("Longest wait between two spawns")]
        [Range(0.05f, 180f)]
        public float maxSpawnDelay = 2f;

        [Tooltip("Minimum spawn distance to player.")]
        [Range(1f, 20f)]
        public float minSpawnDistToPlayer = 4f;

        [Space]
        [Tooltip("Reference objects to be spawn")]
        public GameObject[] references;
        private int[] referId;

        [Header("Spawn Positions")]
        [Tooltip("Method of choosing Spawn Positions.")]
        public SpawnPointChoice spawnPosChoice = SpawnPointChoice.Randomly;
        public Transform[] spawnPositions;


        [Header("Player")]
        [Tooltip("The player")]
        public AbstractPlayerController player;
        private Animator animator;
        private int boolSpawningId;

        protected virtual void Awake()
        {
            referId = new int[references.Length];
            for (int i = 0; i < references.Length; i++)
            {
                var pool = ObjectPool.GetOrInitPool(references[i]);
                referId[i] = pool.OriginalObjectID;
            }

            animator = GetComponent<Animator>();
            boolSpawningId = Animator.StringToHash("Spawning");
        }

        private IEnumerator Start()
        {
            if (animator != null)
            {
                animator.SetBool(boolSpawningId, true);
            }
            yield return new WaitForSeconds(delayIn);

            if (references != null && references.Length > 0)
            {
                for (int i = 0; i < waveNumber; i++)
                {
                    Debug.LogFormat("Beginning spawn wave {0} of {1}...", i, waveNumber);
                    remain = spawnNumber;
                    while (remain > 0)
                    {
                        SpawnGameObject(referId[GetNewRefIndex()]);
                        remain -= 1;
                        yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));
                    }

                    yield return new WaitForSeconds(waveDelay);
                }
            }
            else
            {
                Debug.LogError("There is no reference prefabs attached!");
            }

            if (animator != null)
            {
                yield return new WaitForSeconds(animOutDelay);
                animator.SetBool(boolSpawningId, false);
            }

            OnSpawnsFinished();
        }

        protected virtual void OnSpawnsFinished()
        {
            gameObject.SetActive(false);
        }

        protected virtual GameObject SpawnGameObject(int referenceId)
        {
            var go = ObjectPool.GetInstance(referenceId, PickSpawnPoint(spawnPosChoice, minSpawnDistToPlayer).position, new Quaternion(0, 0, 0, 0));

            return go;
        }

        protected Transform PickSpawnPoint(SpawnPointChoice pointChoice, float minDistToPlayer, bool skipPrevOne = true)
        {
            if (spawnPositions == null || spawnPositions.Length < 1)
            {
                //If there is no spawn point,
                return transform; //the spawn point is spawner itself
            }
            else if (spawnPositions.Length == 1)
            {
                //If we only have one spawn point, no need to choose
                return spawnPositions[0];
            }

            if (!skipPrevOne)
                prevOne = null;


            var spawnPosInRange = player == null ? spawnPositions.ToList()
                                                 : spawnPositions.Where(p => p.DistanceTo(player.transform.position) > minDistToPlayer)
                                                                 .ToList();

            if (spawnPosInRange.Count == 0 && minDistToPlayer > 0.5f)
            {
                Debug.LogError(string.Format("There is no spawn point out of {0} distance to player! Trying with its 75% value...", minDistToPlayer));
                return PickSpawnPoint(pointChoice, minDistToPlayer * 0.75f);
            }
            else if (spawnPosInRange.Count == 1)
            {
                return spawnPosInRange.FirstOrDefault();
            }
            else if (spawnPosInRange.Count == 0)
            {
                Debug.LogErrorFormat("There is no spawn point out of even {0} distance to player!", minDistToPlayer);
                spawnPosInRange = spawnPositions.ToList();
            }

            Transform pickedOne;

            switch (pointChoice)
            {
                case SpawnPointChoice.Randomly:
                    pickedOne = spawnPosInRange[Random.Range(0, spawnPositions.Length - 1)];
                    if (pickedOne == prevOne || pickTry > 30)
                    {
                        pickTry++;
                        pickedOne = PickSpawnPoint(SpawnPointChoice.Randomly, minDistToPlayer);
                    }
                    break;

                case SpawnPointChoice.Sequentally:
                    pickedOne = GetNextSpawnPoint();
                    while (pickedOne.DistanceTo(player.transform.position) < minDistToPlayer && pickTry < 30)
                    {
                        pickTry++;
                        pickedOne = GetNextSpawnPoint();
                    }
                    break;

                case SpawnPointChoice.AwayFromPlayer:
                    if (player != null)
                        pickedOne = spawnPositions
                                .Where(p => p != prevOne)
                                .OrderByDescending(p => p.DistanceTo(player.transform.position))
                                .First();
                    else
                        pickedOne = PickSpawnPoint(SpawnPointChoice.Randomly, minDistToPlayer);
                    break;

                case SpawnPointChoice.CloseToPlayer:
                    if (player != null)
                        pickedOne = spawnPosInRange
                                .Where(p => p != prevOne)
                                .OrderBy(p => p.DistanceTo(player.transform.position))
                                .First();
                    else
                        pickedOne = PickSpawnPoint(SpawnPointChoice.Randomly, minDistToPlayer);
                    break;

                default:
                    pickedOne = transform;
                    break;
            }

            pickTry = 0;
            prevOne = pickedOne;
            return pickedOne;
        }
        private Transform prevOne;
        private int pickTry = 0;

        protected Transform GetNextSpawnPoint()
        {
            spawnIndex = spawnIndex.HasValue && spawnIndex.Value < spawnPositions.Length - 1 ? spawnIndex.Value + 1 : 0;
            return spawnPositions[spawnIndex.Value];
        }
        private int? spawnIndex;


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
}