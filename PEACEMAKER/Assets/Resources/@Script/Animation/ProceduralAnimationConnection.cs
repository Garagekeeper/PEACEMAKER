using System;
using UnityEngine.Serialization;

namespace Resources.Script.Animation
{
    [Serializable]
    public class ProceduralAnimationConnection
    {
        [FormerlySerializedAs("type")]
        public ProceduralAnimationConnectionType type = ProceduralAnimationConnectionType.InfluenceIfTargetPlaying;
        [FormerlySerializedAs("clip")]
        public ProceduralAnimation target;
    }
    
    public enum ProceduralAnimationConnectionType
    {
        None = 0,
        PauseIfTargetIdle = 1,
        PauseIfTargetPlaying = 2,
        InfluenceIfTargetIdle = 3,
        InfluenceIfTargetPlaying = 4,
    }
}