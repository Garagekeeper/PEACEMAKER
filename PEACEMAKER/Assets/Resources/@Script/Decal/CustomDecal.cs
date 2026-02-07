using System;
using UnityEngine;

namespace Resources.Script.Decal
{
    [AddComponentMenu("PEACEMAKER/Effects/Custom Decal")]
    public class CustomDecal : MonoBehaviour
    {
        public GameObject decalVFX;
        public float lifeTime = 60;

        private void Awake()
        {
            //StartCoroutine()
        }
    }
    
}