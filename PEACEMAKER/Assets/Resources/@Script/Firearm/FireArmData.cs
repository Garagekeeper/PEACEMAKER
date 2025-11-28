using Resources.Script.Controller;
using UnityEngine;
using static Resources.Script.Defines;

namespace Resources.Script.Firearm
{
    public class FireArmData
    {
        public string firearmName;
        public EFiringMode firingMode;
        public EShotMechanism shotMechanism;
        public GameObject defaultDecalPrefab;
        public EReloadType reloadType; 

        public float fireRate;
        public float range;
        public float impactForce;
        public float shotDelay;
        public float muzzleVelocity;
        public float projectileSize;
        public float decalSize;
        public bool tracerRounds;

        public float horizontalRecoil;
        public float verticalRecoil;
        public float cameraRecoil;
        public float cameraShakeAmount;
        public float cameraShakeRoughness;
        public float cameraShakeStartTime;
        public float cameraShakeDuration;

        public int magazineCapacity;

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

            fireSound            = preset.presetFireSound.audioClip;
            magInSound           = preset.presetReloadSound.audioClip;
            magOutSound          = preset.presetReloadEmptySound.audioClip;
        }
    }
}