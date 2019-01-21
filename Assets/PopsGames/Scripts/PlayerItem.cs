using Pops.Extensions;
using Pops.Helpers;
using UnityEngine;

namespace Pops
{
    public abstract class PlayerItem : MonoBehaviour
    {
        [Tooltip("VFX game object to display when player takes the item.")]
        public GameObject touchVFX;
        private int _touchVfxId;


        protected virtual void Awake()
        {
            if (touchVFX != null)
            {
                _touchVfxId = touchVFX.GetInstanceID();
                ObjectPool.GetOrInitPool(touchVFX);
            }
        }

        protected virtual void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag.Equals("Player"))
            {
                GiveItem();
            }
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag.Equals("Player"))
            {
                GiveItem();
            }
        }

        protected virtual void GiveItem()
        {
            if (touchVFX != null)
                ObjectPool.GetInstance(_touchVfxId, transform.position.With(y: 0.5f));

            OnGivingItem();
            ObjectPool.Release(gameObject);
        }

        protected abstract void OnGivingItem();
    }
}