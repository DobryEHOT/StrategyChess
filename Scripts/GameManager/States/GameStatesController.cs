using Assets.Scripts.StateMachin;
using Game.Singleton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GameManager.States
{
    public class GameStatesController : Singleton<GameStatesController>
    {
        public GameState Active { get => machine.ActiveState as GameState; }

        private StateMachine machine;

        public void SetMachine(StateMachine machine)
        {
            this.machine = machine;
        }

        public void EndGame()
        {
            machine.SwichTo<EndGameState>();
        }

        private void Awake()
        {
            Inicialize(this);
        }

        private void FixedUpdate()
        {
            if (machine != null)
                machine.DoUpdate();
        }
    }
}
