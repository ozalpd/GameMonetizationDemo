using Pops.Controllers;
using Pops.Helpers;
using System.Collections;
using UnityEngine;

namespace Pops.Weapons
{
    public class Projectile : MonoBehaviour
    {
        [Range(0.25f, 50f)]
        public float speed = 1f;
        [Tooltip("Releases the projectile on any collision detection")]
        public bool releaseOnAnyHit = true;

        [System.NonSerialized]
        public float range = 12;
        protected float distance = 0;

        [Range(1, 100)]
        public int damageFactor = 5;

        [Range(1, 100)]
        public int heatFactor = 5;

        [Tooltip("VFX game object when hit to wall etc.")]
        public GameObject impactVFX;
        private int _impactId;


        private MeshRenderer meshRenderer;
        protected AudioSource audioSource;
        protected RangeWeapon shooter;

        private float audioDuration;
        private float enableTime;
        private bool releasing = false;

        //protected Rigidbody rigidBody;

        //protected ParticleSystem vFXInstance;

        private void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            audioSource = GetComponent<AudioSource>();
            audioDuration = audioSource != null ? audioSource.clip.length : 0;

            if (impactVFX != null)
            {
                _impactId = impactVFX.GetInstanceID();
                ObjectPool.GetOrInitPool(impactVFX);
            }

            //rigidBody = GetComponent<Rigidbody>();
            //rigidBody.detectCollisions = false;

            //vFXInstance = Instantiate(explosionVFX);
            //vFXInstance.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            if (audioSource != null)
                audioSource.Play();
            if (meshRenderer != null)
                meshRenderer.enabled = true;

            distance = 0;
            releasing = false;
            enableTime = Time.time;
        }

        private void FixedUpdate()
        {
            if (releasing)
                return;

            transform.Translate(0, 0, speed * Time.fixedDeltaTime, transform);
            distance += speed * Time.fixedDeltaTime;

            if (distance > range)
                ObjectPool.Release(gameObject);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag.Equals("Wall"))
            {

                if (impactVFX != null)
                {
                    //var rotat = collision.transform.Direction(transform.position);
                    //rotat.y -= 90;
                    ObjectPool.GetInstance(_impactId, collision.contacts[0].point);
                }

                Release();
            }
            else if (releaseOnAnyHit)
            {
                Release();
            }
        }


        /// <summary>
        /// Owner or attacker character of this Projectile
        /// </summary>
        public BaseCharacterController Owner { get; set; }

        public void Release()
        {
            releasing = true;
            StartCoroutine(Release(audioDuration - Time.time + enableTime));
            if (meshRenderer != null)
                meshRenderer.enabled = false;
        }

        private IEnumerator Release(float after)
        {
            yield return new WaitForSeconds(after);
            gameObject.SetActive(false);
        }
    }
}