using Game.Singleton;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using Game.Units.MOD;

namespace Game.Units
{
    public class UnitMaskActions : Singleton<UnitMaskActions>
    {
        [SerializeField] private TextAsset masks;
        private List<ActionMask> actionMasks = new List<ActionMask>();

        private void Awake()
        {
            Inicialize(this);
            OpenFile();
        }

        private void OpenFile() => actionMasks = OpenJSONfile(masks.text);

        public ActionMask GetActionMask(string name)
        {
            foreach (var mask in actionMasks)
                if (mask.UnitName.Equals(name))
                    return mask;

            throw new Exception($"'{name}' actionMask does not exist");
        }

        private List<ActionMask> OpenJSONfile(string json)
        {
            StringBuilder b = new StringBuilder();
            List<ActionMask> lis = new List<ActionMask>();
            var isAct = true;
            var isMod = false;
            ActionMask act = null;
            UnitModInfo mod = null;
            foreach (var litra in json)
            {
                if (!litra.Equals('\n') && !litra.Equals(' ') && !litra.Equals('}')
                    && !litra.Equals('{') && !litra.Equals(':') && !litra.Equals(','))
                {
                    b.Append(litra);
                }
                if (litra.Equals(':'))
                {
                    if (isAct && !isMod)
                    {
                        act = new ActionMask();
                        act.UnitName = b.ToString();

                        isMod = true;
                        isAct = false;

                        lis.Add(act);
                    }
                    else
                    {
                        mod = new UnitModInfo();
                        mod.Name = b.ToString();
                        act.mods.Add(mod);
                    }

                    b.Clear();
                }
                if (litra.Equals(','))
                {

                    if (isMod && mod != null)
                    {
                        mod.mask = b.ToString();
                        mod.mapMask = ReSerializeMap(mod.mask);
                        mod = null;
                    }
                    b.Clear();
                }
                if (litra.Equals('}'))
                {
                    if (isMod && mod != null)
                    {
                        mod.mask = b.ToString();
                        mod.mapMask = ReSerializeMap(mod.mask);
                        mod = null;
                    }

                    isMod = false;
                    isAct = true;
                }
            }

            return lis;
        }

        private static TypeMoveUnit[,] ReSerializeMap(string map)
        {
            TypeMoveUnit[,] bo = new TypeMoveUnit[9, 9];

            if (bo.Length != map.Length)
                return null;

            int i = 0;
            for (var y = 0; y < 9; y++)
            {
                for (var x = 0; x < 9; x++)
                {
                    bo[x, y] = (TypeMoveUnit)(int.Parse(map[i].ToString()));
                    i++;
                }
            }

            return bo;
        }
    }
}
