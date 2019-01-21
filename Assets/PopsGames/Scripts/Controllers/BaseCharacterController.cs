using Pops.Helpers;
using Pops.Weapons;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pops.Controllers
{
    public abstract class BaseCharacterController : MonoBehaviour
    {
        public AbstractWeapon defaultWeapon;
        public Transform weaponPosition;

        [Tooltip("Damage amount that enough to kill this character.")]
        public float maxDamage = 20;

        [Tooltip("VFX game object when character dies.")]
        public GameObject dieVFX;
        private int _dieVfxId;

        protected Animator animator;
        protected List<AbstractWeapon> weapons;

        public delegate void DamageChange(float damage, BaseCharacterController character);
        public delegate void CharacterEvent(BaseCharacterController character);

        protected virtual void Awake()
        {
            if (weaponPosition == null)
                weaponPosition = transform.Find("WeaponPosition");

            if (weaponPosition == null)//if we didn't find it above
                weaponPosition = transform;

            animator = GetComponent<Animator>();
            if (animator == null)  //might be in a child object
                animator = GetComponentInChildren<Animator>();

            weapons = new List<AbstractWeapon>();

            if (dieVFX != null)
            {
                _dieVfxId = dieVFX.GetInstanceID();
                ObjectPool.GetOrInitPool(dieVFX);
            }
        }

        protected virtual void Start()
        {
            if (defaultWeapon != null)
                SwitchWeapon(defaultWeapon);
        }

        protected virtual void OnDisable()
        {
            CancelInvoke("Attack");
            _isAttacking = false;
            _damage = 0;
            _aimAt = null;
        }

        protected virtual void OnDestroy()
        {
            CancelInvoke("Attack");
        }

        public Vector3? AimAt
        {
            get { return _aimAt; }
            set
            {
                if (_aimAt != value)
                {
                    _aimAt = value;
                    if (_aimAt.HasValue)
                        OnAimedAt(_aimAt.Value);
                    else
                        OnAimStopped();
                }
            }
        }
        private Vector3? _aimAt = null;
        protected virtual void OnAimedAt(Vector3 target) { }
        protected virtual void OnAimStopped() { }
        public abstract bool IsAimPrecise { get; }

        public virtual bool Attacking
        {
            get { return _isAttacking; }
            set
            {
                if (_isAttacking != value)
                {
                    _isAttacking = value;

                    if (_isAttacking)
                        OnAttacking();
                    else
                        OnAttackStopped();
                }
            }
        }
        private bool _isAttacking = false;
        protected virtual void OnAttacking()
        {
            if (Weapon == null)
                return;

            if (Weapon.attackSpeed > 0)
            {
                InvokeRepeating("Attack", Weapon.AttackTimeRemaining, Weapon.AttackInterval);
            }
            else
            {
                Attack();
            }
        }

        protected virtual void OnAttackStopped() { CancelInvoke("Attack"); }

        /// <summary>
        /// Simply Weapon's range.
        /// </summary>
        public virtual float AttackRange
        {
            get
            {
                return Weapon != null ? Weapon.AttackRange : 0f;
            }
        }

        public virtual bool CanAttack
        {
            get { return Weapon != null && Weapon.CanAttack; }
        }

        protected virtual void Attack()
        {
            if (CanAttack)
            {
                Weapon.Attack(AimAt ?? Weapon.transform.forward);
            }
        }

        public float Damage
        {
            get { return _damage; }
            set
            {
                if (!Mathf.Approximately(_damage, value))
                {
                    _damage = value;
                    if (DamageChanged != null)
                        DamageChanged(_damage, this);
                    if (Damage >= maxDamage)
                    {
                        if (OnDying != null)
                            OnDying(this);
                        if (CharacterDying != null)
                            CharacterDying(this);
                        Die();
                    }
                }
            }
        }
        protected float _damage = 0;
        public static event DamageChange DamageChanged;
        public static event CharacterEvent CharacterDying;
        public event CharacterEvent OnDying;

        protected virtual void Die()
        {
            if (dieVFX != null)
            {
                ObjectPool.GetInstance(_dieVfxId, transform.position);
            }
            ObjectPool.Release(gameObject);
        }

        public abstract void HitBy(BaseCharacterController attacker);

        public virtual bool Shielding
        {
            get { return _shielding; }
            set
            {
                if (_shielding != value)
                {
                    _shielding = value;

                    if (_shielding)
                        OnShielding();
                    else
                        OnShieldingStopped();
                }
            }
        }
        private bool _shielding;
        protected virtual void OnShielding() { }
        protected virtual void OnShieldingStopped() { }


        public virtual AbstractWeapon Weapon
        {
            get { return _weapon; }
            protected set
            {
                if (_weapon != null)
                    _weapon.StatusChanged -= Weapon_StatusChanged;
                _weapon = value;
                _weapon.Owner = this;
                _weapon.StatusChanged += Weapon_StatusChanged;
            }
        }
        AbstractWeapon _weapon;

        private void Weapon_StatusChanged(AbstractWeapon sender, bool isActive)
        {
            if (Attacking && isActive)
            {
                OnAttacking();
            }
            else if(Attacking)
            {
                OnAttackStopped();
            }
        }


        public virtual void SwitchWeapon(AbstractWeapon weapon)
        {
            if (Weapon != null && Weapon.name.Equals(weapon.name))
                return;

            if (weaponPosition == null)
                weaponPosition = transform;

            bool isAttacking = Attacking;
            if (Weapon != null && Attacking)
                Attacking = false;

            var inListWeapon = weapons.FirstOrDefault(w => w.name.Equals(weapon.name));
            if (inListWeapon != null)
            {
                inListWeapon.gameObject.SetActive(true);
                Weapon = inListWeapon;
            }
            else
            {
                var weaponGO = Instantiate(weapon.gameObject, weaponPosition);
                weaponGO.transform.localRotation = Quaternion.identity;
                weaponGO.transform.localPosition = Vector3.zero;

                Weapon = weaponGO.GetComponent<AbstractWeapon>();
            }

            //override this method if we need not to continue to attack after switching
            Attacking = isAttacking;
        }
    }
}