using Resources.Script.Controller;
using UnityEngine;
using static Resources.Script.Defines;

namespace Resources.Script.Firearm
{
    public class FireArmData
    {
        public string firearmName;
        public EFiringMode firingMode = EFiringMode.Auto;
        public EShotMechanism shotMechanism = EShotMechanism.HitScan;
        public GameObject defaultDecalPrefab;
        public EReloadType reloadType; 

        public float fireRate = 850f;
        public float range = 300f;
        public float impactForce = 10f;
        public float shotDelay = 0f;
        public float muzzleVelocity = 250f;
        public float projectileSize = 0.01f;
        public float decalSize = 1;
        public bool tracerRounds = true;

        public float horizontalRecoil = 0.7f;
        public float verticalRecoil = 0.1f;
        public float cameraRecoil = 1f;
        public float cameraShakeAmount = 0.05f;
        public float cameraShakeRoughness = 5;
        public float cameraShakeStartTime = 0.1f;
        public float cameraShakeDuration = 0.1f;

        public int magazineCapacity = 30;

        public AudioClip fireSound;
        public AudioClip suppressorFireSound;
        public AudioClip magInSound;
        public AudioClip magOutSound;
        private FirearmController FireArm { get; set; }

        public void Init(FirearmController controller, FirearmPreset preset)
        {
            FireArm              = controller;
            firearmName          = preset.firearmName;
            firingMode           = preset.firingMode;
            shotMechanism        = preset.shotMechanism;
            defaultDecalPrefab   = preset.defaultDecalPrefab;
            reloadType           = preset.reloadType;

            fireRate             = preset.fireRate;
            range                = preset.range;
            impactForce          = preset.impactForce;
            shotDelay            = preset.shotDelay;
            muzzleVelocity       = preset.muzzleVelocity;
            projectileSize       = preset.projectileSize;
            decalSize            = preset.decalSize;
            tracerRounds         = preset.tracerRounds;

            horizontalRecoil     = preset.horizontalRecoil;
            verticalRecoil       = preset.verticalRecoil;
            cameraRecoil         = preset.cameraRecoil;
            cameraShakeAmount    = preset.cameraShakeAmount;
            cameraShakeRoughness = preset.cameraShakeRoughness;
            cameraShakeStartTime = preset.cameraShakeStartTime;
            cameraShakeDuration  = preset.cameraShakeDuration;

            magazineCapacity     = preset.magazineCapacity;

            fireSound            = preset.fireSound;
            suppressorFireSound  = preset.suppressorFireSound;
            magInSound           = preset.magInSound;
            magOutSound          = preset.magOutSound;
        }
    }
}