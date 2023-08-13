using Game.MapSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.UnitsSystem.Interfaces
{
    interface IUnitAction
    {
        public void DoBeforeAction();
        public void DoUpdateAction(Chank chank);
        public void DoAfterAction(Chank chank);
    }
}
