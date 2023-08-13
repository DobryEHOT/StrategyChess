using Game.CardSystem;
using Game.CardSystem.HandManipulation;
using Game.MapSystems;
using Game.Singleton;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Global
{
    public class GlobalManager : Singleton<GlobalManager>
    {
        private LevelsStratageActions stratageActions;
        public int Tome { get; private set; } = 0;
        public int Level { get; private set; } = 0;

        void Awake()
        {
            DontDestroyOnLoad(this);
            stratageActions = new LevelsStratageActions();
            Inicialize();
        }

        public void SetTomeAndLevel(int tomeID, int levelID)
        {
            Tome = tomeID;
            Level = levelID;
        }

        public LevelStratage GetLevelStratage()
        {
            return stratageActions.GetLevelStratage(Tome, Level);
        }

        public void SaveActiveLvlAccess(bool access)
        {
            Singleton<SaveSystem>.MainSingleton.TrySetAccessLevel(Tome, Level + 1, access);
        }
    }

    public class LevelsStratageActions
    {
        private List<List<LevelStratage>> tomes = new List<List<LevelStratage>>();
        public LevelsStratageActions()
        {
            tomes.Add(new List<LevelStratage>());
            AddStratage(1, new Level_Tutorial_0());
            AddStratage(1, new Level_Field_1());
            AddStratage(1, new Level_Field_2());
            AddStratage(1, new Level_Field_3());
            AddStratage(1, new Level_Field_4());
            AddStratage(1, new Level_Field_5());
        }

        public LevelStratage GetLevelStratage(int tome, int levelID)
        {
            return tomes[tome][levelID];
        }

        private void AddStratage(int tome, LevelStratage s)
        {
            if (tomes.Count - 1 < tome)
            {
                while (tomes.Count - 1 < tome)
                {
                    tomes.Add(new List<LevelStratage>());
                }
            }
            tomes[tome].Add(s);
        }
    }

    public abstract class LevelStratage
    {
        public abstract List<string> CardOnLevel { get; set; }
        public abstract List<string[]> BotDecCard { get; set; }
        public virtual Objective GameObjective { get; set; } = new KingObjective();
        public abstract float GenerateLevelItems(MapSystem mapSys, Player player);

        protected int countFixedUpdateWait = 4;

        protected void UseCard(Chank chank, Player player, string nameCard = "Default")
        {
            if (chank == null || player == null)
                return;

            var card = player.PlayerHand.TakeCard(nameCard);
            player.StartCoroutine(WaitTime(() => player.PlayerHand.UseCard(card, chank), 0.1f));
        }

        protected void UseCards(List<CardInfo> cards, Player player) => player.StartCoroutine(UseCardsMass(cards, player));

        private IEnumerator UseCardsMass(List<CardInfo> cards, Player player)
        {
            var waiter = new WaitForFixedUpdate();

            foreach (var card in cards)
            {
                for (var i = 0; i < countFixedUpdateWait; i++)
                    yield return waiter;

                if (card.chank != null && player != null)
                    UseCard(card.chank, player, card.NameCard);
            }

        }

        protected IEnumerator WaitTime(Action action, float time)
        {
            yield return new WaitForSeconds(time);
            action();
        }
    }

    public class Level_Tutorial_0 : LevelStratage
    {
        public override List<string> CardOnLevel { get; set; } = new List<string>();
        public override List<string[]> BotDecCard { get; set; } = new List<string[]>();

        public override float GenerateLevelItems(MapSystem mapSys, Player player)
        {
            return 3f;

        }
    }

    public class Level_Field_1 : LevelStratage
    {
        public override List<string> CardOnLevel { get; set; }
            = new List<string>()
            {
                "Soldier",
                "Soldier",
                "Soldier",
                "Soldier",
            //    "Scout",
                "RearGuard",
                "Knight",
            //    "Queen",
                "Spearman",
            //    "Archer",
            //    "ShieldWarrior",
            //    "SabotageMan",
            //    "Occupier",

                "Village",
                "Village",
                "Village",
                "Village",
                "Village",
                "Village",
                "Village",
                "Mansion",
                "Mansion",
            //    "Dirt",
                "Forest",
                "Stair",
                "Stair",
                "Stair",
            //    "MilletField",
            //    "Mill",
                "Mine"
            };
        public override List<string[]> BotDecCard { get; set; }
            = new List<string[]>()
            {
                new[] { "Soldier" },
                //new[] { "Soldier" },
                new[] { "Soldier", "Soldier"},
                new[] { "Spearman", "Village", "Ore" },
                new[] { "Knight", "Soldier", "Ore" },
                //new[] { "Soldier", "Soldier","Soldier"},
                new[] { "Spearman"},
                new[] { "Knight"},
                //new[] { "Knight"},
            };

        public override float GenerateLevelItems(MapSystem mapSys, Player player)
        {
            var map = mapSys.ChanksController.GetMap();
            var x = -3;
            var y = -7;

            var listMap = new List<CardInfo>()
            {
                new CardInfo("Castle_Tower"    , map[x + 5, y + 12]),
                new CardInfo("Castle_HouseCore", map[x + 5, y + 13]),
                new CardInfo("Castle_House"    , map[x + 5, y + 14]),
                new CardInfo("Castle_Wall"     , map[x + 4, y + 14]),
                new CardInfo("Castle_Tower"    , map[x + 3, y + 14]),
                new CardInfo("Castle_Wall"     , map[x + 3, y + 13]),
                new CardInfo("Castle_Tower"    , map[x + 3, y + 12]),
                new CardInfo("Castle_Wall"     , map[x + 4, y + 12]),
                new CardInfo("Stair", map[x + 4, y + 13]),
                new CardInfo("King", map[x + 4, y + 13]),
                new CardInfo("Soldier"    , map[x + 3, y + 12]),
                new CardInfo("Soldier"    , map[x + 3, y + 14]),
                new CardInfo("Soldier"    , map[x + 5, y + 12]),
                new CardInfo("Knight", map[x + 7, y + 13]),
            };

            UseCards(listMap, player);

            return listMap.Count * countFixedUpdateWait * Time.fixedDeltaTime + countFixedUpdateWait * Time.fixedDeltaTime;
        }
    }

    public class Level_Field_2 : LevelStratage
    {
        public override List<string> CardOnLevel { get; set; } = new List<string>()
            {
                "Soldier",
                "Soldier",
                "Spearman",
                "Knight",
                "Scout",
                "Scout",
              //  "RearGuard",
              //  "Knight",
              //  "Queen",
              //  "Spearman",
              //  "Archer",
                "ShieldWarrior",
              //  "SabotageMan",
              //  "Occupier",

                "Village",
                "Village",
                "Village",
                "Village",
                "Village",
                "Village",
                "Mansion",
         //       "Dirt",
        //        "Forest",
        //        "Stair",
        //        "MilletField",
        //        "Mill",
        //        "Mine"
            };
        public override List<string[]> BotDecCard { get; set; }
            = new List<string[]>()
            {
                new[] { "Soldier" },
                new[] { "Soldier", "Soldier"},
                new[] { "Spearman", "Village", "Ore" },
                new[] { "Knight", "Soldier", "Ore" },
                new[] { "Soldier", "Soldier","Soldier"},
                new[] { "Soldier" },
                new[] { "Soldier" },
                new[] { "Soldier" },
                new[] { "Knight" },
                new[] { "Knight" },
            };

        public override float GenerateLevelItems(MapSystem mapSys, Player player)
        {
            var map = mapSys.ChanksController.GetMap();
            var x = 0;
            var y = -7;

            var listMap = new List<CardInfo>()
            {
                new CardInfo( "Forest"            , map[x + 0, y + 4]     ),
                new CardInfo( "Forest"            , map[x + 0, y + 6]     ),
                new CardInfo( "Forest"            , map[x + 0, y + 7]     ),
                new CardInfo( "Forest"            , map[x + 0, y + 9]     ),
                new CardInfo( "Forest"            , map[x + 0, y + 11]    ),
                new CardInfo( "Forest"            , map[x + 0, y + 12]    ),
                new CardInfo( "Forest"            , map[x + 0, y + 13]    ),
                new CardInfo( "Forest"            , map[x + 0, y + 14]    ),

                new CardInfo( "Forest"            , map[x + 1, y + 5]     ),
                new CardInfo( "Forest"            , map[x + 1, y + 7]     ),
                new CardInfo( "Forest"            , map[x + 1, y + 8]     ),
                new CardInfo( "Forest"            , map[x + 1, y + 9]     ),
                new CardInfo( "MountainLow"       , map[x + 1, y + 11]    ),
                new CardInfo( "MountainLow"       , map[x + 1, y + 12]    ),
                new CardInfo( "Forest"            , map[x + 1, y + 13]    ),
                new CardInfo( "Forest"            , map[x + 1, y + 14]    ),

                new CardInfo( "Forest"            , map[x + 2, y + 6]     ),
                new CardInfo( "Forest"            , map[x + 2, y + 7]     ),
                new CardInfo( "Forest"            , map[x + 2, y + 8]     ),
                new CardInfo( "Forest"            , map[x + 2, y + 9]     ),
                new CardInfo( "MountainLow"       , map[x + 2, y + 11]    ),
                new CardInfo( "MountainLow"       , map[x + 2, y + 12]    ),
                new CardInfo( "Forest"            , map[x + 2, y + 13]    ),

                new CardInfo( "Forest"            , map[x + 3, y + 5]     ),
                new CardInfo( "Forest"            , map[x + 3, y + 6]     ),
                new CardInfo( "Forest"            , map[x + 3, y + 7]     ),
                new CardInfo( "Forest"            , map[x + 3, y + 8]     ),
                new CardInfo( "Forest"            , map[x + 3, y + 10]    ),
                new CardInfo( "MountainLow"       , map[x + 3, y + 12]    ),
                new CardInfo( "Forest"            , map[x + 3, y + 13]    ),
                new CardInfo( "Forest"            , map[x + 3, y + 14]    ),

                new CardInfo( "Forest"            , map[x + 4, y + 6]     ),
                new CardInfo( "Forest"            , map[x + 4, y + 7]     ),
                new CardInfo( "Forest"            , map[x + 4, y + 8]     ),
                new CardInfo( "Forest"            , map[x + 4, y + 9]     ),
                new CardInfo( "Forest"            , map[x + 4, y + 10]    ),
                new CardInfo( "Forest"            , map[x + 4, y + 11]    ),
                new CardInfo( "Forest"            , map[x + 4, y + 12]    ),
                new CardInfo( "Forest"            , map[x + 4, y + 13]    ),
                new CardInfo( "Forest"            , map[x + 4, y + 15]    ),

                new CardInfo( "Forest"            , map[x + 5, y + 6]     ),
                new CardInfo( "Forest"            , map[x + 5, y + 7]     ),
                new CardInfo( "Forest"            , map[x + 5, y + 8]     ),
                new CardInfo( "Forest"            , map[x + 5, y + 11]    ),
                new CardInfo( "Forest"            , map[x + 5, y + 12]    ),
                new CardInfo( "Forest"            , map[x + 5, y + 13]    ),
                new CardInfo( "Forest"            , map[x + 5, y + 14]    ),

                new CardInfo( "Forest"            , map[x + 6, y + 5]     ),
                new CardInfo( "Forest"            , map[x + 6, y + 6]     ),
                new CardInfo( "Forest"            , map[x + 6, y + 7]     ),
                new CardInfo( "Forest"            , map[x + 6, y + 8]     ),
                new CardInfo( "Forest"            , map[x + 6, y + 9]     ),
                new CardInfo( "MountainLow"       , map[x + 6, y + 11]    ),
                new CardInfo( "Forest"            , map[x + 6, y + 13]    ),
                new CardInfo( "Forest"            , map[x + 6, y + 14]    ),

                new CardInfo( "Forest"            , map[x + 7, y + 6]     ),
                new CardInfo( "Forest"            , map[x + 7, y + 7]     ),
                new CardInfo( "Forest"            , map[x + 7, y + 9]     ),
                new CardInfo( "Forest"            , map[x + 7, y + 10]    ),
                new CardInfo( "Forest"            , map[x + 7, y + 11]    ),
                new CardInfo( "Forest"            , map[x + 7, y + 12]    ),
                new CardInfo( "Forest"            , map[x + 7, y + 13]    ),
                new CardInfo( "Forest"            , map[x + 7, y + 14]    ),

                new CardInfo( "King"              , map[x + 2, y + 14]    ),
                new CardInfo( "Village"           , map[x + 2, y + 14]    ),
                new CardInfo( "Archer"            , map[x + 1, y + 11]    ),
                new CardInfo( "Archer"            , map[x + 3, y + 11]    ),
                new CardInfo( "Soldier"           , map[x + 2, y + 12]    ),
                new CardInfo( "Archer"            , map[x + 6, y + 11]    ),
            };
            UseCards(listMap, player);

            return listMap.Count * countFixedUpdateWait * Time.fixedDeltaTime + countFixedUpdateWait * Time.fixedDeltaTime;
        }
    }

    public class Level_Field_3 : LevelStratage
    {
        public override List<string> CardOnLevel { get; set; } = new List<string>()
            {
                "Soldier",
                "Soldier",
                //"Scout",
                "RearGuard",
                "Knight",
                "Knight",
               // "Queen",
                "Spearman",
                "Archer",
                "ShieldWarrior",
               // "ShieldWarrior",
               // "SabotageMan",
                "Occupier",

                "Village",
                "Village",
                "Village",
                "Village",
                "Village",
                "Village",
                "Mansion",
                "Dirt",
                "Forest",
              //  "Stair",
                "MilletField",
              //  "Mill",
                "Mine",
                "Mine"
            };
        public override List<string[]> BotDecCard { get; set; }
            = new List<string[]>()
            {
                new[] { "Dirt" },

                new[] { "Soldier" },
                new[] { "Soldier", "Soldier"},
                new[] { "Spearman", "Village", "Dirt" },
                new[] { "Knight", "Soldier" },
                new[] { "ShieldWarrior", "ShieldWarrior", "Soldier"},

                new[] { "Spearman" },
                new[] { "Spearman" },

            };

        public override float GenerateLevelItems(MapSystem mapSys, Player player)
        {
            var map = mapSys.ChanksController.GetMap();
            var x = 0;
            var y = -5;

            var listMap = new List<CardInfo>()
            {
                new CardInfo("Forest"             , map[x + 0, y + 4] ),
                new CardInfo("Forest"             , map[x + 0, y + 6] ),
                new CardInfo("Forest"             , map[x + 0, y + 7] ),
                new CardInfo("Ore"                , map[x + 0, y + 9] ),

                new CardInfo( "Mountain"          , map[x + 0, y + 11]),
                new CardInfo( "Mountain"          , map[x + 0, y + 12]),
                new CardInfo( "Forest"            , map[x + 0, y + 13]),
                //new CardInfo( "Ore"               , map[x + 0, y + 14]),

                new CardInfo("Forest"             , map[x + 1, y + 5] ),
                new CardInfo("MountainLow"        , map[x + 1, y + 7] ),
                new CardInfo("MountainLow"        , map[x + 1, y + 8] ),
                new CardInfo("Forest"             , map[x + 1, y + 9] ),
                new CardInfo( "MountainLow"       , map[x + 1, y + 11]),
                new CardInfo( "MountainHight"     , map[x + 1, y + 12]),
                new CardInfo( "Mountain"          , map[x + 1, y + 13]),
                new CardInfo( "Forest"            , map[x + 1, y + 14]),

                new CardInfo("Forest"             , map[x + 2, y + 6] ),
                new CardInfo("Forest"             , map[x + 2, y + 7] ),
                new CardInfo("Forest"             , map[x + 2, y + 8] ),
                new CardInfo("Forest"             , map[x + 2, y + 9] ),
                new CardInfo( "MountainHight"     , map[x + 2, y + 11]),
                new CardInfo( "MountainHight"     , map[x + 2, y + 12]),
                new CardInfo( "Forest"            , map[x + 2, y + 13]),


                new CardInfo("Forest"             , map[x + 3, y + 5] ),
                new CardInfo("Forest"             , map[x + 3, y + 6] ),
                new CardInfo("Forest"             , map[x + 3, y + 7] ),
                new CardInfo( "MountainHight"     , map[x + 3, y + 10]),
                new CardInfo( "MountainLow"       , map[x + 3, y + 11]),
                new CardInfo( "MountainLow"       , map[x + 3, y + 12]),
                new CardInfo( "Forest"            , map[x + 3, y + 13]),
                new CardInfo( "Forest"            , map[x + 3, y + 14]),


                new CardInfo("Forest"             , map[x + 4, y + 6] ),
                new CardInfo("Forest"             , map[x + 4, y + 7] ),
                new CardInfo("MountainLow"        , map[x + 4, y + 8] ),
                new CardInfo("MountainLow"        , map[x + 4, y + 9] ),
                new CardInfo( "MountainLow"       , map[x + 4, y + 10]),
                new CardInfo( "Forest"            , map[x + 4, y + 11]),
                new CardInfo( "Forest"            , map[x + 4, y + 12]),
                new CardInfo( "Forest"            , map[x + 4, y + 13]),
                new CardInfo( "Forest"            , map[x + 4, y + 15]),


                new CardInfo("Forest"             , map[x + 6, y + 5] ),
                new CardInfo("Forest"             , map[x + 6, y + 6] ),
                new CardInfo("Forest"             , map[x + 6, y + 7] ),
                new CardInfo("MountainLow"        , map[x + 6, y + 8] ),
                new CardInfo("Mountain"           , map[x + 6, y + 9] ),
                new CardInfo( "MountainHight"     , map[x + 6, y + 11]),
                new CardInfo( "Forest"            , map[x + 6, y + 13]),
                new CardInfo( "Forest"            , map[x + 6, y + 14]),


                new CardInfo("MountainHight"      , map[x + 7, y + 9] ),
                new CardInfo("Forest"             , map[x + 7, y + 7] ),
                new CardInfo( "Mountain"          , map[x + 7, y + 10]),
                new CardInfo( "Mountain"          , map[x + 7, y + 11]),
                new CardInfo( "Mountain"          , map[x + 7, y + 12]),
                new CardInfo( "Forest"            , map[x + 7, y + 13]),
                new CardInfo( "Forest"            , map[x + 7, y + 14]),


                new CardInfo( "King"              , map[x + 5, y + 12]),
                new CardInfo( "Village"           , map[x + 5, y + 12]),
                new CardInfo( "Archer"            , map[x + 1, y + 11]),
                new CardInfo( "Soldier"           , map[x + 0, y + 12]),


                new CardInfo( "Archer"            , map[x + 6, y + 11]),


                new CardInfo("Castle_Tower"       , map[x + 4, y + 8] ),
                new CardInfo("Castle_Tower"       , map[x + 6, y + 8] ),
                new CardInfo("Castle_Wall"        , map[x + 4, y + 9] ),


                new CardInfo( "Castle_Tower"      , map[x + 4, y + 10]),


                new CardInfo( "Castle_HouseCore"  , map[x + 3, y + 11]),
                new CardInfo("Forest"             , map[x + 7, y + 9] ),
                new CardInfo( "Archer"            , map[x + 4, y + 8] ),
            };

            UseCards(listMap, player);
            return listMap.Count * countFixedUpdateWait * Time.fixedDeltaTime + countFixedUpdateWait * Time.fixedDeltaTime;
        }
    }

    public class Level_Field_4 : LevelStratage
    {
        public override List<string> CardOnLevel { get; set; } = new List<string>()
            {
                "Soldier",
                "Soldier",
                "Soldier",
                //"Scout",
                //"RearGuard",
                "Knight",
                "Knight",
               // "Queen",
                "Spearman",
                "Archer",
                "ShieldWarrior",
                "SabotageMan",
                "Occupier",

                "Village",
                "Village",
                "Village",
                "Village",
                "Mansion",
               // "Dirt",
                "Forest",
                "Stair",
                "Stair",
                "MilletField",
                "Mill",
                "Mine",
                "Ore"
            };
        public override List<string[]> BotDecCard { get; set; }
            = new List<string[]>()
            {
                new[] { "Soldier" },
                new[] { "Soldier", "Soldier"},
                new[] { "Spearman", "Village", "Soldier"},
                new[] { "Knight", "Soldier", "Soldier"},
                new[] { "Soldier", "Soldier","Soldier"},

                new[] { "Queen" },
                new[] { "Knight" },
                new[] { "Knight" },
                new[] { "Knight" },


            };

        public override float GenerateLevelItems(MapSystem mapSys, Player player)
        {
            var map = mapSys.ChanksController.GetMap();
            var x = -3;
            var y = -7;

            var x1 = -2;
            var y1 = -8;

            var x2 = 0;
            var y2 = -13;

            var listMap = new List<CardInfo>()
            {
                new CardInfo("Castle_Tower"    , map[x + 5, y + 12]),
                new CardInfo("Castle_HouseCore", map[x + 5, y + 13]),
                new CardInfo("Castle_House"    , map[x + 5, y + 14]),
                new CardInfo("Castle_Wall"     , map[x + 4, y + 14]),
                new CardInfo("Castle_Tower"    , map[x + 3, y + 14]),
                new CardInfo("Castle_Wall"     , map[x + 3, y + 13]),
                new CardInfo("Castle_Tower"    , map[x + 3, y + 12]),
                new CardInfo("Castle_Wall"     , map[x + 4, y + 12]),
                new CardInfo("Stair", map[x + 4, y + 13]),

                new CardInfo("River"     , map[x1 + 2, y1 + 11]),
                new CardInfo("River"     , map[x1 + 3, y1 + 11]),
                new CardInfo("River"     , map[x1 + 4, y1 + 11]),
                new CardInfo("River"     , map[x1 + 5, y1 + 11]),
                new CardInfo("River"     , map[x1 + 6, y1 + 11]),
                new CardInfo("River"     , map[x1 + 8, y1 + 11]),
                new CardInfo("River"     , map[x1 + 9, y1 + 11]),

                new CardInfo("Village"     , map[x1 + 2 , y1 + 11+ 1]),
                new CardInfo("Village"     , map[x1 + 3 , y1 + 11+ 1]),
                new CardInfo("Village"     , map[x1 + 4 , y1 + 11+ 1]),
                new CardInfo("Forest"     , map[x1 + 5 , y1 + 11+ 1]),
                new CardInfo("Village"     , map[x1 + 6 , y1 + 11+ 1]),

                new CardInfo("Village"     , map[x1 + 7, y1 + 14]),
                new CardInfo("Village"     , map[x1 + 8, y1 + 13]),
                new CardInfo("Mansion"     , map[x1 + 9, y1 + 13]),

                new CardInfo("Mill"     , map[x1 + 8, y1 + 14]),
                new CardInfo("MilletField"     , map[x1 + 7, y1 + 13]),
                new CardInfo("MilletField"     , map[x1 + 8, y1 + 12]),

                new CardInfo("Forest"                 , map[x2 + 0, y2 + 4] ),
                new CardInfo("Forest"                 , map[x2 + 0, y2 + 6] ),
                new CardInfo("Forest"                 , map[x2 + 0, y2 + 7] ),
                new CardInfo("Forest"                 , map[x2 + 0, y2 + 9] ),
                new CardInfo("Mountain"               , map[x2 + 0, y2 + 11]),
                new CardInfo("Mountain"               , map[x2 + 0, y2 + 12]),
                new CardInfo("Forest"                 , map[x2 + 0, y2 + 13]),
                //new CardInfo("Forest"                 , map[x2 + 0, y2 + 14]),
                                                            
                new CardInfo("Forest"                 , map[x2 + 1, y2 + 5] ),
                new CardInfo("MountainLow"            , map[x2 + 1, y2 + 7] ),
                new CardInfo("MountainLow"            , map[x2 + 1, y2 + 8] ),
                new CardInfo("Forest"                 , map[x2 + 1, y2 + 9] ),
                new CardInfo("MountainLow"            , map[x2 + 1, y2 + 11]),
                new CardInfo("MountainHight"          , map[x2 + 1, y2 + 12]),
                new CardInfo("MountainLow"               , map[x2 + 1, y2 + 13]),
                //new CardInfo("Forest"                 , map[x2 + 1, y2 + 14]),
                                                          
                new CardInfo("Forest"                 , map[x2 + 2, y2 + 6] ),
                new CardInfo("Forest"                 , map[x2 + 2, y2 + 7] ),
                new CardInfo("Forest"                 , map[x2 + 2, y2 + 8] ),
                new CardInfo("Forest"                 , map[x2 + 2, y2 + 9] ),
                new CardInfo("MountainHight"          , map[x2 + 2, y2 + 11]),
                new CardInfo("MountainHight"          , map[x2 + 2, y2 + 12]),
                new CardInfo("Forest"                 , map[x2 + 2, y2 + 13]),

                new CardInfo("Forest"                 , map[x2 + 3, y2 + 5] ),
                new CardInfo("Forest"                 , map[x2 + 3, y2 + 6] ),
                new CardInfo("Forest"                 , map[x2 + 3, y2 + 7] ),
                new CardInfo("Forest"                 , map[x2 + 3, y2 + 8] ),
                new CardInfo("MountainHight"          , map[x2 + 3, y2 + 10]),
                new CardInfo("MountainLow"            , map[x2 + 3, y2 + 11]),
                new CardInfo("MountainLow"            , map[x2 + 3, y2 + 12]),
                new CardInfo("Forest"                 , map[x2 + 3, y2 + 13]),
                //new CardInfo("Forest"                 , map[x2 + 3, y2 + 14]),
                                                          
                new CardInfo("Forest"                 , map[x2 + 4, y2 + 6] ),
                new CardInfo("Forest"                 , map[x2 + 4, y2 + 7] ),
                new CardInfo("MountainLow"            , map[x2 + 4, y2 + 8] ),
                new CardInfo("MountainLow"            , map[x2 + 4, y2 + 9] ),
                new CardInfo("MountainLow"            , map[x2 + 4, y2 + 10]),
                new CardInfo("Forest"                 , map[x2 + 4, y2 + 11]),
                new CardInfo("Forest"                 , map[x2 + 4, y2 + 12]),
                new CardInfo("Forest"                 , map[x2 + 4, y2 + 13]),
                //new CardInfo("Forest"                 , map[x2 + 4, y2 + 15]),
                                                        
                new CardInfo("Forest"                 , map[x2 + 6, y2 + 5] ),
                new CardInfo("Forest"                 , map[x2 + 6, y2 + 6] ),
                new CardInfo("Forest"                 , map[x2 + 6, y2 + 7] ),
                new CardInfo("MountainLow"            , map[x2 + 6, y2 + 8] ),
                new CardInfo("Mountain"               , map[x2 + 6, y2 + 9] ),
                new CardInfo("MountainHight"          , map[x2 + 6, y2 + 11]),
                new CardInfo("Forest"                 , map[x2 + 6, y2 + 13]),
                //new CardInfo("Forest"                 , map[x2 + 6, y2 + 14]),
                                                           
                new CardInfo("MountainHight"          , map[x2 + 7, y2 + 9] ),
                new CardInfo("Forest"                 , map[x2 + 7, y2 + 7] ),
                new CardInfo("Mountain"               , map[x2 + 7, y2 + 10]),
                new CardInfo("Mountain"               , map[x2 + 7, y2 + 11]),
                new CardInfo("Mountain"               , map[x2 + 7, y2 + 12]),
                new CardInfo("Forest"                 , map[x2 + 7, y2 + 13]),
                //new CardInfo("Forest"                 , map[x2 + 7, y2 + 14]),

                new CardInfo("King"                   , map[x + 4, y + 13]),
                new CardInfo("Archer"                   , map[x + 5, y + 12]),
                new CardInfo("Knight"                   , map[x1 + 7, y1 + 14]),
                new CardInfo("RearGuard"                   ,  map[x1 + 5, y1 + 12]),

            };

            UseCards(listMap, player);
            return listMap.Count * countFixedUpdateWait * Time.fixedDeltaTime + countFixedUpdateWait * Time.fixedDeltaTime;
        }
    }

    public class Level_Field_5 : LevelStratage
    {
        public override List<string> CardOnLevel { get; set; } = new List<string>()
            {
                "Soldier",
                "Soldier",
                "Soldier",
                "Scout",
                "RearGuard",
                "Knight",
                "Knight",
                "Knight",
                "Knight",
                "Queen",
                "Spearman",
                "Spearman",
                "Spearman",
                "Archer",
                "Archer",
                "ShieldWarrior",
                "ShieldWarrior",
                "SabotageMan",
                "Occupier",

                //"Village",
                //"Mansion",
                //"Dirt",
                "Forest",
                "Stair",
                "Stair",
                //"MilletField",
                //"Mill",
                "Mine",
                "Ore"
            };
        public override List<string[]> BotDecCard { get; set; }
            = new List<string[]>()
            {
                new[] { "Soldier" },
                new[] { "Soldier", "Soldier"},
                new[] { "Spearman" },
                new[] { "Knight", "Soldier"},
                new[] { "Soldier", "Soldier","Soldier"},
                new[] { "Knight","Spearman" },

                new[] { "ShieldWarrior" },
                new[] { "ShieldWarrior" },
                new[] { "Archer","Archer" },
                new[] { "Knight", "Knight", "Knight"},
                new[] { "Archer" },
                new[] { "Queen" },

                new[] { "Soldier" },

                new[] { "Dirt" },
                new[] { "Dirt" },
                new[] { "Dirt" },
                new[] { "Dirt" },
                new[] { "Dirt" },

            };

        public override float GenerateLevelItems(MapSystem mapSys, Player player)
        {
            var map = mapSys.ChanksController.GetMap();
            var x = -2;
            var y = -5;

            var x1 = -2;
            var y1 = -9;

            var listMap = new List<CardInfo>()
            {
                new CardInfo( "Castle_Tower"                      , map[x + 5 - 1, y + 12 - 2]),
                new CardInfo( "Castle_Wall"                       , map[x + 5 - 1, y + 13 - 2]),
                new CardInfo( "Castle_Tower"                      , map[x + 5 - 1, y + 14 - 2]),
                new CardInfo( "Castle_Wall"                       , map[x + 3 - 1, y + 13 - 2]),
                new CardInfo( "Castle_Tower"                      , map[x + 3 - 1, y + 12 - 2]),
                new CardInfo( "Castle_Wall"                       , map[x + 4 - 1, y + 12 - 2]),
                new CardInfo( "Castle_HouseCore"                  , map[x + 4 - 1, y + 14 - 2]),
                new CardInfo( "Castle_House"                      , map[x + 3 - 1, y + 14 - 2]),

                new CardInfo( "Stair"                             , map[x + 4 - 1, y + 13 - 2]),

                new CardInfo( "CityBuild"                         , map[x + 2, y + 9] ),
                new CardInfo( "CityBuild"                         , map[x + 3, y + 9] ),
                new CardInfo( "CityBuild"                         , map[x + 4, y + 9] ),
                new CardInfo( "CityBuild"                         , map[x + 5, y + 9] ),
                new CardInfo( "CityBuild"                         , map[x + 7, y + 9] ),
                new CardInfo( "CityBuild"                         , map[x + 8, y + 9] ),
                new CardInfo( "CityBuild"                         , map[x + 9, y + 9] ),

                //new CardInfo( "CityBuild"                         , map[x + 2, y + 10]),
                //new CardInfo( "CityBuild"                         , map[x + 3, y + 10]),
                //new CardInfo( "CityBuild"                         , map[x + 4, y + 10]),
                new CardInfo( "CityBuild"                         , map[x + 5, y + 10]),
                new CardInfo( "CityBuild"                         , map[x + 7, y + 10]),
                new CardInfo( "Market"                            , map[x + 8, y + 10]),
                new CardInfo( "CityBuild"                         , map[x + 9, y + 10]),

                //new CardInfo( "Mansion"                           , map[x + 2, y + 11]),
                //new CardInfo( "Mansion"                           , map[x + 3, y + 11]),
                //new CardInfo( "Mansion"                           , map[x + 4, y + 11]),
                new CardInfo( "Mansion"                           , map[x + 5, y + 11]),
                new CardInfo( "Mansion"                           , map[x + 7, y + 11]),
                new CardInfo( "Mansion"                           , map[x + 8, y + 11]),
                new CardInfo( "Mansion"                           , map[x + 9, y + 11]),


                new CardInfo( "Castle_Wall"                       , map[x + 2, y + 8] ),
                new CardInfo( "Castle_Tower"                      , map[x + 3, y + 8] ),
                new CardInfo( "Castle_Wall"                       , map[x + 4, y + 8] ),
                new CardInfo( "Castle_Tower"                      , map[x + 5, y + 8] ),
                new CardInfo( "Castle_Tower"                      , map[x + 7, y + 8] ),
                new CardInfo( "Castle_Wall"                       , map[x + 8, y + 8] ),
                new CardInfo( "Castle_Tower"                      , map[x + 9, y + 8] ),

                new CardInfo("River", map[x1 + 2, y1 + 11]),
                new CardInfo("River", map[x1 + 3, y1 + 11]),
                new CardInfo("River", map[x1 + 4, y1 + 11]),
                new CardInfo("River", map[x1 + 5, y1 + 11]),
                new CardInfo("River", map[x1 + 7, y1 + 11]),
                new CardInfo("River", map[x1 + 8, y1 + 11]),
                new CardInfo("River", map[x1 + 9, y1 + 11]),

                new CardInfo("MillWater", map[x1 + 8, y1 + 10]),
                new CardInfo("MilletField", map[x1 + 8, y1 + 9]),
                new CardInfo("Village", map[x1 + 7, y1 + 9]),
                new CardInfo("Mansion", map[x1 + 2, y1 + 9]),
                new CardInfo("Village", map[x1 + 3, y1 + 9]),
                new CardInfo("Village", map[x1 + 2, y1 + 10]),
                new CardInfo("Forest", map[x1 + 3, y1 + 10]),

                new CardInfo( "King"                      , map[x + 4 - 1, y + 13 - 2]),
                new CardInfo( "Archer"                      , map[x + 5 - 1, y + 12 - 2]),
                new CardInfo( "Archer"                      , map[x + 7, y + 8] ),
            };

            UseCards(listMap, player);
            return listMap.Count * countFixedUpdateWait * Time.fixedDeltaTime + countFixedUpdateWait * Time.fixedDeltaTime;
        }
    }

    public struct CardInfo
    {
        public string NameCard;
        public Chank chank;

        public CardInfo(string name, Chank chank)
        {
            this.NameCard = name;
            this.chank = chank;
        }
    }
}
