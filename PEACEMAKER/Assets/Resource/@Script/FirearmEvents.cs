using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Resource.Script
{
    public class FirearmEvents
    {
        [Tooltip("Action to be invoked when firing is starting, passing the position, rotation, and direction of the projectile. If 'NumberOfShots' is 2, this event will be invoked once the fire action is triggred once.")]
        public UnityEvent<Vector3, Quaternion, Vector3> onFireDemand;
        [Tooltip("Action to be invoked when firing is completed once, passing the position, rotation, and direction of the projectile. If 'NumberOfShots' is 2, this event will be invoked twice each time the firearm is fired.")]
        public UnityEvent<Vector3, Quaternion, Vector3> onFireDone;
        [Tooltip("Action to be invoked when post firing events are done once e.g firing sound and effects")]
        public UnityEvent onFireApplied;
        [Tooltip("Action to be invoked when reloading is starting."), FormerlySerializedAs("OnReload")]
        public UnityEvent onReloadStarting;
        [Tooltip("Action to be invoked when reloading is started. This is invoked when IsReloading state changes.")]
        public UnityEvent onReloadStart;
        [Tooltip("Action to be invoked when reloading is completed.")]
        public UnityEvent onReloadComplete;
        [Tooltip("Action to be invoked when reloading is applied once.")]
        public UnityEvent onReloadApplied;
        [Tooltip("Action to be invoked when relpading is cancled.")]
        public UnityEvent onReloadCancel;
        [Tooltip("Action to be invoked when FireMode state is changed.")]
        public UnityEvent onFireModeChange;

        ////TODO: Add these events
        ////OnAim
        ////OnDropped
        ////OnItemPickedUp
        ////OnDryFire
        ////OnOutOfAmmo
        ////OnOutOfAmmoType
    }
}