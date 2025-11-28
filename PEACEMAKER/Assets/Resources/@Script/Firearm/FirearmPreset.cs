using Resources.Script.Audio;
using UnityEngine;
using static Resources.Script.Defines;

namespace Resources.Script.Firearm
{
    [CreateAssetMenu(fileName = "New Firearm Data", menuName = "PEACEMAKER/Firearm Data")]
    public class FirearmPreset : ScriptableObject
    {
        [Header("General")]
        public string firearmName;
        public EFiringMode firingMode = EFiringMode.Auto;
        public EShotMechanism shotMechanism = EShotMechanism.HitScan;
        public GameObject defaultDecalPrefab;
        public EReloadType reloadType = EReloadType.Default; 
    
        [Header("Stats")]
        public float fireRate = 850f;
        public float range = 300f;
        public float impactForce = 10f;
        public float shotDelay = 0f;
        public float muzzleVelocity = 250f;
        public float projectileSize = 0.01f;
        public float decalSize = 1;
        public bool tracerRounds = true;

        [Header("Recoil")]
        public float horizontalRecoil = 0.7f;
        public float verticalRecoil = 0.1f;
        public float cameraRecoil = 1f;
        public float cameraShakeAmount = 0.05f;
        public float cameraShakeRoughness = 5;
        public float cameraShakeStartTime = 0.1f;
        public float cameraShakeDuration = 0.1f;
    
        [Header("Ammo")]
        public int magazineCapacity = 30;
        
        [Header("Audio")]
        //Audio
        public AudioPreset presetFireSound;
        public AudioPreset presetReloadSound;
        public AudioPreset presetReloadEmptySound;
        public AudioClip fireSound;
        public AudioClip suppressorFireSound;
        public AudioClip magInSound;
        public AudioClip magOutSound;
        
        // 탄약 타입 등 필요한 데이터 추가 가능
    }
}