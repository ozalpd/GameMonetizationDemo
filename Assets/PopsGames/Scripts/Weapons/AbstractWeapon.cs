using Pops.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pops.Weapons
{
    public abstract class AbstractWeapon : MonoBehaviour
    {
        public new string name;
        [Range(0f, 10f)]
        [Tooltip("Attack per second. If set to zero can not attack automatically.")]
        public float attackSpeed = 1;
        private float _lastAttackTime;

        public abstract float AttackRange { get; set; }

        [Tooltip("Ratio of Attack Distance to Attack Range. Intended for NPCs.")]
        [Range(0.1f, 1f)]
        public float attackDistRate = 0.5f;

        public delegate void WeaponStatusChange(AbstractWeapon sender, bool isActive);

        protected virtual void Awake()
        {
            if (string.IsNullOrEmpty(name))
                name = gameObject.name;
        }


        public void Attack(Vector3 target)
        {
            if (CanAttack)
            {
                OnAttacking(target, _lastAttackTime);
                _lastAttackTime = Time.time;
            }
        }

        /// <summary>
        /// Actual attack method which should be implemented
        /// </summary>
        /// <param name="target">Target vector</param>
        /// <param name="prevAttackTime">Previous attack time recorded from Time.time</param>
        protected abstract void OnAttacking(Vector3 target, float prevAttackTime);

        /// <summary>
        /// Read only property, calculated by using Attack Distance Rate. Intended for NPCs.
        /// </summary>
        public float AttackDistance { get { return AttackRange * attackDistRate; } }

        /// <summary>
        /// Intervals between attacks, calculated from attackSpeed
        /// </summary>
        public float AttackInterval
        {
            get
            {
                return attackSpeed > 0 ? 1 / attackSpeed : 0;
            }
        }

        /// <summary>
        /// Time remaining for ability to a new attack in seconds
        /// </summary>
        public float AttackTimeRemaining
        {
            get
            {
                return attackSpeed > 0 ? AttackInterval + _lastAttackTime - Time.time : 0;
            }
        }

        /// <summary>
        /// Can weapon attack, Attack method checks this
        /// </summary>
        public virtual bool CanAttack
        {
            get
            {
                return IsActive && !(AttackTimeRemaining > AttackInterval * 0.1f);
            }
        }

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    if (StatusChanged != null)
                        StatusChanged(this, _isActive);
                }
            }
        }
        private bool _isActive = true;
        public event WeaponStatusChange StatusChanged;

        public bool IsAutomatic
        {
            get { return attackSpeed > 0; }
        }


        /// <summary>
        /// Owner character of this weapon
        /// </summary>
        public BaseCharacterController Owner { get; set; }
    }
}