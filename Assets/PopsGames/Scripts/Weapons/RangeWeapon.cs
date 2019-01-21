using Pops.Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pops.Weapons
{
    public class RangeWeapon : AbstractWeapon
    {
        public Projectile projectile;
        public float projectileSpeed = 12;
        private int _projectileId;
        [SerializeField]
        [Range(5f, 50f)]
        private float attackRange = 20f;

        [Range(5, 5000)]
        [Tooltip("Heat value that makes weapon inoperative.")]
        public int overheatVal = 100;

        [Tooltip("When Cooling Down the heat value that weapon can work.")]
        public int cDOperative = 50;

        [Range(0.1f, 10f)]
        [Tooltip("Cool down ticks per second.")]
        public float cDDuration = 5f;

        [Range(1, 500)]
        [Tooltip("Heat minus value for every CD tick.")]
        public int cDFactor = 1;

        public GameObject attackVFX;
        private int _attackVfxId;
        public Transform vfxParent;

        public Transform projectileGuide;

        protected override void Awake()
        {
            base.Awake();
            var pool = ObjectPool.GetOrInitPool(projectile.gameObject, 20);
            _projectileId = pool.OriginalObjectID;

            if (attackVFX != null)
            {
                pool = ObjectPool.GetOrInitPool(attackVFX, 20);
                _attackVfxId = pool.OriginalObjectID;
            }

            if (vfxParent == null)
                vfxParent = transform;

            if (projectileGuide == null)
                projectileGuide = transform;
        }

        protected virtual void Start()
        {
            InvokeRepeating("CoolDown", 0.1f, cDDuration);
        }

        public override float AttackRange
        {
            get { return attackRange; }
            set { attackRange = value; }
        }

        protected virtual void CoolDown()
        {
            if (Heat > 0)
                Heat -= cDFactor;
        }

        public int Heat
        {
            protected set
            {
                if (_heat != value)
                {
                    _heat = value < 0 ? 0 : value;
                    if (_heat >= overheatVal)
                    {
                        IsActive = false;
                        overheated = true;
                    }
                    else if (_heat <= cDOperative && overheated)
                    {
                        IsActive = true;
                        overheated = false;
                    }
                }
            }
            get { return _heat; }
        }
        private int _heat = 0;
        private bool overheated;

        protected override void OnAttacking(Vector3 target, float prevAttackTime)
        {
            var go = ObjectPool.GetInstance(_projectileId, projectileGuide.position, projectileGuide.rotation);
            var p = go.GetComponent<Projectile>();
            p.range = AttackRange;
            p.speed = projectileSpeed;
            p.Owner = Owner;
            Heat += p.heatFactor;

            if (attackVFX != null)
            {
                var vfxGO = ObjectPool.GetInstance(_attackVfxId,
                                                   vfxParent.position,
                                                   vfxParent.rotation);

                vfxGO.transform.parent = vfxParent;
            }
        }
    }
}