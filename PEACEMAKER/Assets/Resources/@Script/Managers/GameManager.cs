using Resources.Script.Creatures;
using static Resources.Script.Utilities;

namespace Resources.Script.Managers
{
    public class GameManager
    {
        public bool IsPaused { get; set; } = false;
        public bool ProcAnimIsActive {get; private set;} = false;

        public int CurrentFirearmNum { get; set; } = -1;

        public Player MainPlayer { get; set; }
        
        public float MouseSensitivity { get; set; } = 1f;

        public void Init()
        {
            ProcAnimIsActive = true;
            HeadManager.Game.IsPaused = false;
            UnlockCursor();
            CurrentFirearmNum = -1;
            MainPlayer = null;
            //MouseSensitivity = 1f;
        }
    }
}