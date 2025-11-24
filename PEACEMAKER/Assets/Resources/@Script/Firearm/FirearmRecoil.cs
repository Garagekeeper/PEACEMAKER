using Resources.Script.Animation;
using Resources.Script.Controller;
using UnityEngine;

namespace Resources.Script.Firearm
{
    public class FirearmRecoil : MonoBehaviour
    {
        public FirearmController FireArm { get; private set; }
        private ProceduralAnimator ProcAnimator { get; set; }
        
        public void Init(FirearmController fireArm)
        {
            FireArm = fireArm;
            ProcAnimator = FireArm.anim.ProceduralAnimator;
        }
        public void ApplyRecoil()
        {
            // Play recoil animations if the procedural animator is assigned
            if (ProcAnimator)
            {
                FireArm.anim.RecoilAnimation?.Play(0); // Play recoil animation from the beginning
                FireArm.anim.RecoilAimAnimation?.Play(0); // Play recoil aim animation from the beginning
            }
            
            // Apply camera recoil adjustments if the character manager is assigned
            if (FireArm.Owner)
            {
                // Calculate the recoil values based on the preset and firearm attachments, considering aiming state
                //TODO 부착물에 따른 MOD 적용
                //float vRecoil = verticalRecoil * firearmAttachmentsManager.recoil;
                float vRecoil = FireArm.fireArmData.verticalRecoil;
                float hRecoil = FireArm.fireArmData.horizontalRecoil;
                float camRecoil = FireArm.fireArmData.cameraRecoil;
            
                FireArm.Owner.AddLookValue(vRecoil, hRecoil);
            
                // TODO cam 흔들리게
                // if(characterManager.cameraManager)
                // {
                //     if (characterManager.cameraManager.cameraKickAnimation)
                //     {
                //         characterManager.cameraManager.cameraKickAnimation.Play(0);
                //
                //         characterManager.cameraManager.cameraKickAnimation.weight = cameraRecoil;
                //     }
                // }
            }
        }
    }
}