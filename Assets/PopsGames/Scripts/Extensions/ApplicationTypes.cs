using Pops.Controllers;
using UnityEngine;

namespace Pops.Extensions
{
    public static class ApplicationTypes
    {
        public static bool UsesRigidbody(this MoveMechanism moveMechanism)
        {
            return moveMechanism == MoveMechanism.AddForce
                || moveMechanism == MoveMechanism.SetVelocity;
        }
    }
}