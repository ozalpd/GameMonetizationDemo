using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pops.Helpers
{
    /// <summary>
    /// Keeps a number of instantiated game objects ready to use in memory and recyles them to keep memory unfragmanted.
    /// </summary>
    public class ObjectPool : MonoBehaviour
    {
        /// <summary>
        /// The pool of objects
        /// </summary>
        private List<GameObject> _objectList;

        /// <summary>
        /// The original GameObject
        /// </summary>
        public GameObject OriginalGO { get; private set; }

        /// <summary>
        /// InstanceID of the original GameObject which probably is a prefab.
        /// </summary>
        public int OriginalObjectID { get; private set; }

        /// <summary>
        /// Number of the instantiated game objects
        /// </summary>
        public int PoolSize { get; private set; }

        /// <summary>
        /// Keeps the object pool instances
        /// </summary>
        private static Dictionary<int, ObjectPool> _pools = new Dictionary<int, ObjectPool>();

        #region public instance methods
        /// <summary>
        /// Clears all active game objects in this pool instance.
        /// </summary>
        public void Clear()
        {
            var actives = _objectList.Where(o => o.activeSelf);
            foreach (var go in actives)
            {
                go.SetActive(false);
            }
        }

        public IEnumerable<GameObject> GetActives()
        {
            if (_actives == null || Time.time > _activesQueryTime + 0.05f) //Do not query more than 20 times in a second
            {
                _actives = _objectList.Where(o => o.activeSelf);
                _activesQueryTime = Time.time;
            }
            return _actives;
        }
        private IEnumerable<GameObject> _actives;
        private float _activesQueryTime = -1;

        #endregion

        /// <summary>
        /// Clears all active game objects in the specified pool instance.
        /// </summary>
        /// <param name="originalId">InstanceID of the original GameObject.</param>
        public static void ClearPool(int originalId)
        {
            if (_pools.ContainsKey(originalId) && _pools[originalId] != null)
            {
                _pools[originalId].Clear();
            }
        }

        /// <summary>
        /// Clears all active game objects in all pool instances.
        /// </summary>
        public static void ClearAllPools()
        {
            foreach (var p in _pools.Values)
            {
                if (p != null)
                    p.Clear();
            }
        }


        /// <summary>
        /// Finds an ObjectPool instance from a static dictionary. If no instance found initiliazes a new instance.
        /// </summary>
        /// <param name="original">The original GameObject that is needed to be in ObjectPool instance</param>
        /// <param name="poolSize">Number of the instantiated game objects in the new instance. If there is already an instance of ObjectPool this param has no effect.</param>
        /// <returns>ObjectPool that initiliazed or found in Dictionary</returns>
        public static ObjectPool GetOrInitPool(GameObject original, int poolSize = 200)
        {
            int id = original.GetInstanceID();
            ObjectPool pool = _pools.ContainsKey(id) ? _pools[id] : null;
            if (pool == null)
            {
                var poolGO = new GameObject("ObjectPool: " + original.name);
                pool = poolGO.AddComponent<ObjectPool>();
                pool.OriginalGO = original;
                pool.OriginalObjectID = id;
                pool.PoolSize = poolSize;
                pool.Init();
            }

            return pool;
        }

        private void Init()
        {
            _objectList = new List<GameObject>();
            for (int i = 0; i < PoolSize; i++)
            {
                GameObject go = Instantiate(OriginalGO);
                AddGameObjectToPool(go);
            }

            if (_pools.ContainsKey(OriginalObjectID))
                _pools[OriginalObjectID] = this;
            else
                _pools.Add(OriginalObjectID, this);
        }

        private GameObject AddGameObjectToPool(GameObject go, bool setActive = false)
        {
            if (!setActive)
                go.SetActive(false);
            go.transform.SetParent(transform);
            _objectList.Add(go);

            var pooledObjects = go.GetComponents<PooledMonoBehaviour>();
            foreach (var p in pooledObjects)
            {
                p.ObjectPool = this;
            }

            return go;
        }

        private GameObject GetOneInstanceFromPool(Vector3 position = default(Vector3), Quaternion rotation = default(Quaternion), bool setActive = true)
        {
            var go = _objectList.FirstOrDefault(o => !o.activeSelf);
            if (go == null)
            {
                go = Instantiate(OriginalGO, position, rotation);
                AddGameObjectToPool(go, setActive);
                PoolSize++;
            }
            else
            {
                go.transform.position = position;
                go.transform.rotation = rotation;
                go.SetActive(setActive);
            }

            return go;
        }

        /// <summary>
        /// Intended to be used like Object.Instantiate method.
        /// </summary>
        /// <param name="originalId">Hash ID of an existing object that you want to activate a copy of it</param>
        /// <param name="position">Position for the activated object.</param>
        /// <param name="rotation">Orientation for the activated object.</param>
        /// <returns></returns>
        public static GameObject GetInstance(int originalId, Vector3 position = default(Vector3), Quaternion rotation = default(Quaternion), bool setActive = true)
        {
            return _pools[originalId].GetOneInstanceFromPool(position, rotation, setActive);
        }

        /// <summary>
        /// Intended to be used like Object.Instantiate method.
        /// </summary>
        /// <param name="original">An existing object that you want to activate a copy of it.</param>
        /// <param name="position">Position for the activated object.</param>
        /// <param name="rotation">Orientation for the activated object.</param>
        /// <param name="poolSize">If an ObjectPool instance is not found, this will be poolSize of the new instance</param>
        /// <returns></returns>
        public static GameObject GetInstance(GameObject original, Vector3 position = default(Vector3), Quaternion rotation = default(Quaternion), bool setActive = true, int poolSize = 200)
        {
            ObjectPool pool = GetOrInitPool(original);
            return pool.GetOneInstanceFromPool(position, rotation, setActive);
        }

        /// <summary>
        /// Releases the specified obj.
        /// </summary>
        /// <param name="obj">GameObject to be released</param>
        public static void Release(GameObject obj)
        {
            obj.SetActive(false);
        }
    }
}