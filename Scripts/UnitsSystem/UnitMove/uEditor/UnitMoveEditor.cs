
#if UNITY_EDITOR

namespace Game.Units.MOD
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using UnityEditor;
    using UnityEditor.AnimatedValues;
    using UnityEngine;
    public class UnitMoveEditor : UnityEditor.EditorWindow
    {
        static TypeMoveUnit[,] bo = new TypeMoveUnit[9, 9];
        static bool incialize = false;
        static Texture2D textUnit;
        static Texture2D textFree;
        static Texture2D textMove;
        static Texture2D textAttack;
        static Texture2D textAttackMove;
        static GUIStyle styleUnit;
        static GUIStyle styleFree;
        static GUIStyle styleMove;
        static GUIStyle styleAttack;
        static GUIStyle styleAttackMove;
        static GUIStyle styleNames;
        static GUIStyle styleJSON;
        static Dictionary<TypeMoveUnit, GUIStyle> durInfo = new Dictionary<TypeMoveUnit, GUIStyle>();
        static FileStream file;
        static string JSON;
        static UnityEngine.TextAsset oFile;
        static UnitMoveEditor window;

        UnityEditor.AnimatedValues.AnimBool m_ShowExtraFields;
        Vector2 scroll = new Vector2(0, 0);
        Vector2 scrollJSON = new Vector2(0, 0);
        List<ActionMask> actionMasks = new List<ActionMask>();
        string nameUnit;
        string fileName = "FileName";

        void OnEnable()
        {
            if (incialize)
                return;

            m_ShowExtraFields = new AnimBool(true);
            m_ShowExtraFields.valueChanged.AddListener(Repaint);

        }

        [UnityEditor.MenuItem("ChessConstruct/ChessMaskActionConstructor")]
        static void ChessConstuctOpen()
        {
            window = (UnitMoveEditor)EditorWindow.GetWindow(typeof(UnitMoveEditor), true, "Chess mask action constructor");
            window.Show();
        }

        void OnGUI()
        {
            if (!incialize)
            {
                Inicial();
                return;
            }

            scroll = EditorGUILayout.BeginScrollView(scroll);
            EditorGUILayout.BeginHorizontal();
            oFile = (TextAsset)EditorGUILayout.ObjectField(oFile, typeof(TextAsset), true);
            if (GUILayout.Button("Open File") && oFile != null)
            {
                fileName = oFile.name;
                actionMasks = OpenJSONfile(oFile.text);
                oFile = null;
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add Unit"))
            {
                var aM = new ActionMask();
                aM.UnitName = nameUnit;
                if (!actionMasks.Contains(aM))
                    actionMasks.Add(aM);
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginVertical(styleAttackMove);
            for (var i = 0; i < actionMasks.Count; i++)
            {
                EditorGUILayout.LabelField("---------------------------------------------------------------------------------------------------------");
                EditorGUILayout.BeginHorizontal(styleAttackMove);
                actionMasks[i].showMods = EditorGUILayout.Toggle(actionMasks[i].showMods);
                actionMasks[i].UnitName = EditorGUILayout.TextArea(actionMasks[i].UnitName);
                EditorGUILayout.LabelField("Unit name", styleNames);
                if (GUILayout.Button("Add MOD"))
                {
                    var aM = new UnitModInfo();
                    aM.Name = nameUnit;
                    if (!actionMasks[i].mods.Contains(aM))
                        actionMasks[i].mods.Add(aM);
                }

                if (GUILayout.Button("X", styleAttack))
                {
                    actionMasks.Remove(actionMasks[i]);
                    continue;
                }

                EditorGUILayout.EndHorizontal();
                if (!actionMasks[i].showMods)
                    continue;

                EditorGUILayout.BeginVertical(styleAttackMove);
                var mods = actionMasks[i].mods;
                for (var el = 0; el < mods.Count; el++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Space(30);
                    mods[el].showMask = EditorGUILayout.Toggle(mods[el].showMask);
                    mods[el].Name = EditorGUILayout.TextArea(mods[el].Name);
                    EditorGUILayout.LabelField("MOD name", styleNames);
                    if (GUILayout.Button("X", styleAttack))
                    {
                        mods.Remove(mods[el]);
                        continue;
                    }

                    EditorGUILayout.EndHorizontal();
                    if (mods[el].showMask)
                    {
                        var re = ReSerializeMap(mods[el].mask);
                        re = MapActions(re);
                        mods[el].mapMask = re;
                        mods[el].mask = SerializeMap(re);
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();

            scrollJSON = EditorGUILayout.BeginScrollView(scrollJSON, styleJSON);
            JSON = GetJSON();
            EditorGUILayout.TextArea(JSON, styleJSON);
            EditorGUILayout.EndScrollView();

            ShowSave();
        }

        private static void Inicial()
        {
            styleUnit = new GUIStyle("Button");
            styleFree = new GUIStyle("Button");
            styleMove = new GUIStyle("Button");
            styleAttack = new GUIStyle("Button");
            styleAttackMove = new GUIStyle("Box");
            styleNames = new GUIStyle("Box");
            styleJSON = new GUIStyle("textField");

            incialize = true;

            textUnit = new Texture2D(1, 1);
            textUnit.SetPixel(0, 0, Color.black);

            textFree = new Texture2D(1, 1);
            textFree.SetPixel(0, 0, Color.white);

            textMove = new Texture2D(1, 1, TextureFormat.RGBA64, true);
            textMove.SetPixel(1, 1, Color.HSVToRGB(0, 0, 40));

            textAttack = new Texture2D(1, 1);
            textAttack.SetPixel(0, 0, Color.red);

            textAttackMove = new Texture2D(1, 1);
            textAttackMove.SetPixel(0, 0, Color.yellow);

            styleUnit.normal.textColor = Color.black;
            styleFree.normal.textColor = Color.white;
            styleMove.normal.textColor = Color.blue;

            styleAttack.fixedWidth = 40;
            styleAttack.normal.textColor = Color.red;

            styleFree.fixedWidth = 40;
            styleMove.fixedWidth = 40;
            styleUnit.fixedWidth = 40;
            styleNames.fixedWidth = 80;

            styleJSON.stretchHeight = false;

            styleAttackMove.normal.textColor = Color.HSVToRGB(0, 0, 46);
            styleAttackMove.focused = styleAttackMove.normal;
            styleAttackMove.active = styleAttackMove.normal;
            styleAttackMove.hover = styleAttackMove.normal;
            styleAttackMove.hover = styleAttackMove.normal;

            styleAttackMove.onNormal = styleAttackMove.onNormal;
            styleAttackMove.onActive = styleAttackMove.onNormal;
            styleAttackMove.onFocused = styleAttackMove.onNormal;
            styleAttackMove.onHover = styleAttackMove.onNormal;

            durInfo.Add(TypeMoveUnit.UnitPos, styleUnit);
            durInfo.Add(TypeMoveUnit.Free, styleFree);
            durInfo.Add(TypeMoveUnit.Move, styleMove);
            durInfo.Add(TypeMoveUnit.Attack, styleAttack);
            durInfo.Add(TypeMoveUnit.MoveAndAttack, styleAttackMove);
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
                        mod = null;
                    }
                    b.Clear();
                }
                if (litra.Equals('}'))
                {
                    if (isMod && mod != null)
                    {
                        mod.mask = b.ToString();
                        mod = null;
                    }
                    isMod = false;
                    isAct = true;
                }
            }
            return lis;
        }

        private static TypeMoveUnit[,] MapActions(TypeMoveUnit[,] bo)
        {
            bo[4, 4] = TypeMoveUnit.UnitPos;
            for (var y = 0; y < 9; y++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space(30, true);

                for (var x = 0; x < 9; x++)
                {
                    var i = ((int)bo[x, y] + 1);
                    if (i > 1)
                        i = 0;

                    var nextType = (TypeMoveUnit)i;

                    if (GUILayout.Button(((int)bo[x, y]).ToString(), durInfo[bo[x, y]]))
                        bo[x, y] = nextType;

                }

                EditorGUILayout.EndHorizontal();
            }

            return bo;
        }

        private static void Resett()
        {
            if (GUILayout.Button("Reset"))
            {
                for (var y = 0; y < 9; y++)
                    for (var x = 0; x < 9; x++)
                        bo[x, y] = TypeMoveUnit.Free;

                bo[4, 4] = TypeMoveUnit.UnitPos;
            }
        }

        private static void ShowMetaInfo()
        {
            GUILayout.Space(20);
            foreach (var el in durInfo)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Button(((int)el.Key).ToString(), el.Value);
                EditorGUILayout.LabelField($"{(int)el.Key} -> {el.Key}", durInfo[el.Key]);
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(20);
        }

        private void ShowSave()
        {
            fileName = EditorGUILayout.TextArea(fileName);

            if (GUILayout.Button("Save"))
                File.WriteAllText(Application.dataPath + @"\Scripts\UnitsSystem\UnitMove\uEditor\" + fileName + ".json", JSON);
        }

        private static string SerializeMap(TypeMoveUnit[,] bo)
        {
            StringBuilder b = new StringBuilder();

            foreach (var el in bo)
            {
                b.Append(((int)el));
            }

            return b.ToString();
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

        private string GetJSON()
        {
            StringBuilder b = new StringBuilder();
            b.Append("{");
            foreach (var e in actionMasks)
            {
                b.Append($"\n     {e.UnitName}:\n     {{");
                foreach (var m in e.mods)
                {
                    b.Append($"\n           {m.Name}:{m.mask}");

                    if (e.mods.Count - 1 != e.mods.IndexOf(m))
                        b.Append($",");
                }

                b.Append($"\n     }}");
                if (actionMasks.Count - 1 != actionMasks.IndexOf(e))
                    b.Append($",");
            }

            b.Append("\n}");
            return b.ToString();
        }
    }
}
#endif