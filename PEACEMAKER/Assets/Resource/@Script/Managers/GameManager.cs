namespace Resource.Script.Managers
{
    public class GameManager : IManagerBase
    {
        public bool IsPaused { get; private set; } = false;
        public bool ProcAnimIsActive {get; private set;} = true;
        
        public void OnUpdate()
        {
            
        }
    }
}