using Pops;
using Pops.Controllers;
using Pops.Extensions;
using Pops.Weapons;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public abstract class AbstractPlayerController : BaseCharacterController
{
    [Range(1f, 200f)]
    public float speed = 5f;
    public MoveMechanism moveMechanism;
    [Range(0.5f, 100f)]
    public float aimRotSpeed = 10f;
    [Range(0.05f, 5f)]
    public float aimPrecision = 0.5f;

    protected Quaternion lookRotation = Quaternion.identity;

    public override bool IsAimPrecise
    {
        get
        {
            if (AimAt == null)
                return false;

            lookRotation = Quaternion.LookRotation(transform.position.Direction(AimAt.Value, true));

            return Quaternion.Angle(lookRotation, transform.rotation) < aimPrecision;
        }
    }

    protected override void OnAimedAt(Vector3 target)
    {
        lookRotation = Quaternion.LookRotation(transform.position.Direction(AimAt.Value, true));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, aimRotSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Moves player
    /// </summary>
    /// <param name="movement"></param>
    /// <param name="deltaTime">If this method called from Fixed update this should be Time.fixedDeltaTime, otherwise Time.deltaTime</param>
    public abstract void Move(Vector3 movement);


    /// <summary>
    /// Resets properties, variables and transformations.
    /// </summary>
    public virtual void ResetPlayer()
    {
        ResetValues();
    }
    /// <summary>
    /// Sets properties and variables to default values.
    /// </summary>
    public abstract void ResetValues();
}