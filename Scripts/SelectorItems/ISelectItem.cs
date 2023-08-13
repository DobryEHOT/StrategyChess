using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.SelectorSpace
{
    interface ISelectItem
    {
        void OnSelectItem(SelectorItems selector);
        void OnAimSelectItem(SelectorItems selector);
        void OnUnAimSelectItem(SelectorItems selector);
        void OnUnselectItem(SelectorItems selector);
    }
}
