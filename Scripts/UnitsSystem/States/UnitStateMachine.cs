using Assets.Scripts.StateMachin;
using Game.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.UnitsSystem.States
{
    class UnitStateMachine : StateMachine
    {
        private Unit rootUnit;
        public UnitStateMachine(Unit unit)
        {
            rootUnit = unit;
            InicializeMachine();
        }

        protected override void InicializeMachine()
        {
            AddState(new IdleUnitState(rootUnit));
            AddState(new SelectUnitState(rootUnit));
            AddState(new KeepUnitState(rootUnit));
            AddState(new ActionUnitState(rootUnit));

            SwichTo<IdleUnitState>();
        }
    }
}
