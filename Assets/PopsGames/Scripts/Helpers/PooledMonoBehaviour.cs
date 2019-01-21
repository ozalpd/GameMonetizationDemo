using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pops.Helpers
{
    public abstract class PooledMonoBehaviour : MonoBehaviour
    {
        public ObjectPool ObjectPool
        {
            get { return _objectPool; }
            set
            {
                if (_objectPool != null)
                    Debug.LogError("OriginalObjectID can be set once!");
                else
                    _objectPool = value;
            }
        }
        private ObjectPool _objectPool;

        /// <summary>
        /// InstanceID of the original GameObject which probably is a prefab.
        /// </summary>
        public int OriginalObjectID
        {
            get { return ObjectPool.OriginalObjectID; }
        }


        public IEnumerable<GameObject> Others
        {
            get
            {
                if (_others == null || Time.time > _othersTime + 0.1f)//Do not query more than 10 times in a second
                {
                    _others = ObjectPool
                                .GetActives()
                                .Where(o => o != gameObject);
                    _othersTime = Time.time;
                }

                return _others;
            }
        }
        private IEnumerable<GameObject> _others;
        private float _othersTime = -1;
    }
}