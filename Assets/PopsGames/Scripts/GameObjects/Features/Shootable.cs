using Pops.Controllers;
using Pops.Helpers;
using Pops.Weapons;
using UnityEngine;

namespace Pops.GameObjects.Features
{
    /// <summary>
    /// Makes the game object to be shootable by other game objects those are tagged as Projectile.
    /// </summary>
    public class Shootable : MonoBehaviour
    {
        [Tooltip("Point that will be added when player makes a hit to this object.")]
        public int hitPoint = 0;

        [Tooltip("Point that will be added when player kills this object.")]
        public int killPoint = 5;

        [Tooltip("Damage amount that enough to kill this object.")]
        [SerializeField]
        private float maxDamage = 20;

        [Header("Effects")]
        [Tooltip("VFX game object when a hit by a bullet etc.")]
        public GameObject impactVFX;
        private int _impactId;

        [Tooltip("VFX game object when charcter dies.")]
        public GameObject killVFX;
        private int _explosionId;

        [Header("Loot System")]
        public PlayerItem lootableItem;
        private int _lootableItemId;
        private GameObject _lootableGO;

        [Range(0f, 1f)]
        [Tooltip("Item drop possibility when object killed. Zero is never. 0.5. 50% chance. One is certain.")]
        public float lootPossibility = 0;

        private BaseCharacterController _character;

        private const string tagProjectile = "Projectile";

        private void Awake()
        {
            if (killVFX != null)
            {
                _explosionId = killVFX.GetInstanceID();
                ObjectPool.GetOrInitPool(killVFX);
            }

            if (impactVFX != null)
            {
                _impactId = impactVFX.GetInstanceID();
                ObjectPool.GetOrInitPool(impactVFX);
            }

            if (lootableItem != null)
            {
                _lootableGO = lootableItem.gameObject;
                _lootableItemId = _lootableGO.GetInstanceID();
                ObjectPool.GetOrInitPool(_lootableGO);
            }

            _character = GetComponent<BaseCharacterController>();
            if (_character != null)
                _character.OnDying += OnCharacterDying;
        }

        private void OnCollisionEnter(Collision collision)
        {
            CheckIsHitByProjectile(collision.gameObject, collision.contacts[0].point);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            CheckIsHitByProjectile(collision.gameObject, collision.contacts[0].point);
        }

        private void OnDisable()
        {
            _damage = 0;
        }

        private void OnCharacterDying(BaseCharacterController character)
        {
            DropItem();
            GameManager.Score += killPoint;
        }


        public float Damage
        {
            get { return _character != null ? _character.Damage : _damage; }
            set
            {
                if (!Mathf.Approximately(_damage, value))
                {
                    if (_character == null)
                        _damage = value;
                    else
                        _character.Damage = value;
                }
            }
        }
        protected float _damage = 0;

        public float MaxDamage
        {
            get { return _character != null ? _character.maxDamage : maxDamage; }
            set
            {
                if (_character == null)
                    maxDamage = value;
                else
                    _character.maxDamage = value;
            }
        }

        protected virtual void Die(Vector3 damagePoint)
        {
            DropItem();
            GameManager.Score += killPoint;

            if (killVFX != null)
            {
                ObjectPool.GetInstance(_explosionId, damagePoint);
            }

            ObjectPool.Release(gameObject);
        }

        public virtual void DropItem()
        {
            if (lootableItem == null || Mathf.Approximately(lootPossibility, 0))
                return;

            if (Random.Range(0f, 1f) < lootPossibility)
                ObjectPool.GetInstance(_lootableItemId, transform.position, transform.rotation);
        }

        protected virtual void CheckIsHitByProjectile(GameObject collider, Vector3 contactPoint)
        {
            if (!collider.gameObject.tag.Equals(tagProjectile))
                return;

            if (hitPoint > 0)
                GameManager.Score += hitPoint;

            if (Damage < MaxDamage)
            {
                if (impactVFX != null)
                    ObjectPool.GetInstance(_impactId, contactPoint);
            }

            var p = collider.GetComponent<Projectile>();
            if (p != null)
            {
                Damage += p.damageFactor;
                if (_character != null && Damage < MaxDamage)
                    _character.HitBy(p.Owner);

                p.Release();
            }
            else
            {
                Debug.LogWarning("Hit by " + collider.name + " which is tagged as '" + tagProjectile + "' but no Projectile component found!");
                Damage += 2f;
                ObjectPool.Release(collider.gameObject);
            }

            if (_character == null)      //If the GameObject does not have
            {                            //a BaseCharacterController component,
                if (Damage >= maxDamage) //this instance must
                    Die(contactPoint);   //handle to die.
            }
        }
    }
}