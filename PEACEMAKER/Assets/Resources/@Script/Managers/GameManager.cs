using Resources.Script.Controller;

namespace Resources.Script.Managers
{
    public class GameManager : IManagerBase
    {
        public bool IsPaused { get; private set; } = false;
        public bool ProcAnimIsActive {get; private set;} = true;
        
        public PlayerController Player { get; set; }
        
        public void OnUpdate()
        {
            
        }
    }
}