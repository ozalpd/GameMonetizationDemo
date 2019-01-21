using Pops;
using Pops.Controllers;
using Pops.Extensions;
using Pops.Weapons;
using UnityEngine;

public class SimplePlayerController : AbstractPlayerController
{
    public GameObject aimPointer;
    public Transform projectileGuide;

    [Range(0.01f, 2.5f)]
    [Tooltip("Speed of rotation")]
    public float rotationalSpeed = 0.25f;

    public float acceleration = 2f;
    private float fixedAcceleration;
    private float fixedSpeed;

    private Vector3 rotating = Vector3.zero;
    private Vector3 prevMovement = Vector3.zero;
    private float deltaTime;

    [Range(0.2f, 1f)]
    [Tooltip("Speed multiplier when rotating or walking backward.")]
    public float rotateSpeedFactor = 0.75f;

    protected new Rigidbody rigidbody;

    protected override void Awake()
    {
        if (defaultWeapon is RangeWeapon && projectileGuide != null)
            ((RangeWeapon)defaultWeapon).projectileGuide = projectileGuide;

        base.Awake();

        rigidbody = GetComponent<Rigidbody>();
        if (rigidbody == null && moveMechanism.UsesRigidbody())
            Debug.LogWarning("A Rigidbody component is not found. Player must have a Rigidbody component.");

        if (aimPointer != null)
            aimPointer.SetActive(false);
    }

    public bool IsWalking
    {
        get { return _isWalking; }
        set
        {
            if (_isWalking != value)
            {
                _isWalking = value;
                if (animator != null)
                    animator.SetBool("isWalking", _isWalking);
            }
        }
    }
    private bool _isWalking;


    public override void HitBy(BaseCharacterController attacker)
    {
        //TODO: put a clue to UI that player is under attack 
    }

    public override void Move(Vector3 movement)
    {
        IsWalking = movement.magnitude > 0;
        fixedAcceleration = IsWalking ? acceleration * 3 : acceleration;

        if (IsWalking && transform.IsBehind(transform.position + movement))//if walking backward or sides
        {
            movement = movement * rotateSpeedFactor;
            fixedSpeed = -1;
        }
        else if (IsWalking)
        {
            fixedSpeed = 1;
        }
        else
        {
            fixedSpeed = 0;
        }

        if (AimAt.HasValue || IsWalking)
            lookRotation = Quaternion.LookRotation(
                            transform.position.Direction(AimAt ?? (transform.position + movement), true));

        if (moveMechanism.UsesRigidbody())
        {
            deltaTime = Time.fixedDeltaTime;
        }
        else
        {
            deltaTime = Time.deltaTime;
            movement = prevMovement.Lerp(movement, Mathf.Clamp(deltaTime * fixedAcceleration, 0f, 1f));
        }

        switch (moveMechanism)
        {
            case MoveMechanism.SetPosition:
                transform.position += deltaTime * movement;
                prevMovement = movement;
                break;

            case MoveMechanism.SetPositionAndAccelerate:
                rotating = lookRotation.eulerAngles - transform.rotation.eulerAngles;
                prevMovement = movement.Lerp(movement * rotateSpeedFactor, Mathf.Clamp(rotating.magnitude * 0.0025f, 0f, 1f));
                transform.position += deltaTime * prevMovement;
                break;

            case MoveMechanism.SetVelocity:
                rigidbody.velocity = rigidbody.velocity.Lerp(movement, Mathf.Clamp(deltaTime * fixedAcceleration, 0f, 1f));
                break;

            case MoveMechanism.AddForce:
                rigidbody.AddForceAtPosition(movement, transform.position);
                break;

            default:
                break;
        }

        fixedSpeed = fixedSpeed * movement.magnitude;
        if (animator != null)
            animator.SetFloat("speed", fixedSpeed);

        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Mathf.Clamp(20 * deltaTime * rotationalSpeed, 0f, 1f));
    }

    protected override void OnAimedAt(Vector3 target)
    {
        if (aimPointer != null)
            aimPointer.SetActive(true);

        base.OnAimedAt(target);
    }

    protected override void OnAimStopped()
    {
        if (aimPointer != null)
            aimPointer.SetActive(false);
    }

    public override void ResetValues()
    {
        //TODO: reset values
    }

    public override void SwitchWeapon(AbstractWeapon weapon)
    {
        base.SwitchWeapon(weapon);
        if (weapon is RangeWeapon && projectileGuide != null)
                ((RangeWeapon)weapon).projectileGuide = projectileGuide;
    }
}
