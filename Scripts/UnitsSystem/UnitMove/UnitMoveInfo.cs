using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Units.MOD
{ 
    [Serializable]
    public struct UnitMoveInfo
    {
        public Dictionary<bool, bool> map;
    }

}
