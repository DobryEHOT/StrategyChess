using Game.CardSystem.MOD;
using Game.MapSystems;
using Game.MapSystems.Enums;
using Game.MapSystems.Generator;
using Game.Singleton;
using Game.Units;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Unit))]
public class Bot : MonoBehaviour
{
    //Тип бота
    //В данной реализации сильно нарушины принципы SOLID. Поэтому данный класс является "универсалом".
    //Данный бот может использовать как нейронную сеть для принятия решений, так и алгоритм
    //Данная сущность позволяет нам выбрать, что он будет использовать для принятия решений
    public BotType typeBot = BotType.NeuralNetwork;

    //Стимулы
    //Множетели, которые влияют на выдачу очков эффективности
    //Все множетели перенастраиваются в инспекторе в среде Unity
    public int WantLive = 1;
    public int WantDead = -10;
    public int WantKill = 1;
    public int WantMove = 1;
    public int WantWin = 1;
    public int WantStay = -1;
    public int WantGoToKing = 1;

    //Слой с данными для входных нейронов нейронной сети
    private float[] input = new float[6 * 6 * 4 + 6 * 4 + 1 + 3];

    //Уже инициализированная нейронная сеть, которая выдаётся Менеджером.
    public NeuralNetwork network;

    //Сохраняем показатель эффективности на время матча.
    public float position;

    private GameMap map;
    public Unit unit;
    private SpawnerToyMOD spawnerToy;
    private Bot enemuKing;

    //Переменная тригер. Нужна для определения "а выжила ли пешка?".
    public bool canSurvive = false;

    [SerializeField]
    private GameObject prefDebugTeamBlue;
    [SerializeField]
    private GameObject prefDebugTeamRed;

    public void Awake()
    {
    }

    public void Start()
    {
        //Проброс ссылки и определение типа бота по принадлежности к команде
        unit = GetComponent<Unit>();
        if (unit.senor.Team == Game.MapSystems.Enums.GameTeam.Red)
            typeBot = BotType.AlgoritmActive;

        map = Singleton<MapSystem>.MainSingleton.ChanksController.GetMap();
        spawnerToy = unit.GetCardPrefab().GetComponent<SpawnerToyMOD>();
        enemuKing = Singleton<Manager>.MainSingleton.GetEnemyKings(unit.senor.Team);

        //Добавление бота в лист ботов в Менеджере.
        //Менеджер выдаёт экземпляр нейронной сети.
        Singleton<Manager>.MainSingleton.AddBot(this);

        //(Часть визуализации) Создание цветного шарика над головой
        var deb = GameObject.Instantiate(unit.senor.Team == GameTeam.Blue ? prefDebugTeamBlue : prefDebugTeamRed, transform);

    }

    //Методы выдающие награду/наказание за определённые действия(триггеры)
    public void TakeKillPrice()
    {
        position += WantKill;
    }

    public void TakeLivePrice()
    {
        position += WantLive;
    }

    public void TakeWinPrice()
    {
        position += WantWin;
    }
    public void TakeMovePrice()
    {
        position += WantMove;
    }

    public void TakeStayPrice()
    {
        position += WantStay;
    }
    public void TakeDeadPrice()
    {
        position += WantDead;
    }
    public void TakeGoToKing()
    {
        position += WantGoToKing;
    }

    //Использование нейронной сети или алгоритма в зависимости от типа бота
    public void UseBrain()
    {
        if (typeBot == BotType.NeuralNetwork)
            NeurNet();

        if (typeBot == BotType.AlgoritmActive)
            if (!TryAttack())
                TryMove();

    }

    private void TryMove() // Метод бота "Алгоритма". Пытается сделать ход. Просто находит ближайшую клетку до вражеского кароля и встаёт на неё.
    {

        var pos = unit.StandChank.CoordinateCurrent;
        var list = new List<Chank>();
        list.Add(map[(int)pos.x + 1, (int)pos.y]);
        list.Add(map[(int)pos.x - 1, (int)pos.y]);
        list.Add(map[(int)pos.x, (int)pos.y + 1]);
        list.Add(map[(int)pos.x, (int)pos.y - 1]);

        Chank chank = null;
        if (enemuKing == null)
            enemuKing = Singleton<Manager>.MainSingleton.GetEnemyKings(unit.senor.Team);

        var kingPos = enemuKing.unit.StandChank.CoordinateCurrent;
        foreach (var ch in list)
        {
            if (ch == null)
                continue;

            if (chank == null)
                chank = ch;

            if ((chank.CoordinateCurrent - kingPos).sqrMagnitude > (ch.CoordinateCurrent - kingPos).sqrMagnitude)
                chank = ch;
        }

        if (chank == null)
            return;
        if (!chank.ChankContainer.IsFree(0))
        {
            bool y = false;
            if (Random.Range(0, 1) == 1)
            {
                if (MoveRight())
                    y = true;
            }

            if (!y)
                MoveLeft();

            return;
        }
        var chankPos = chank.CoordinateCurrent;

        MovePawnToChank(chank);
    }

    private bool TryAttack() // Метод бота "Алгоритма". Пытается сделать атаку, если есть вражеские юниты.
    {
        var pos = unit.StandChank.CoordinateCurrent;
        var list = new List<Chank>();
        list.Add(map[(int)pos.x + 1, (int)pos.y + 1]);
        list.Add(map[(int)pos.x - 1, (int)pos.y - 1]);
        list.Add(map[(int)pos.x - 1, (int)pos.y + 1]);
        list.Add(map[(int)pos.x + 1, (int)pos.y - 1]);
        foreach (var ch in list)
        {
            if (ch == null)
                continue;

            if (!ch.ChankContainer.IsFree(0))
            {
                Unit tunit;
                if (ch.ChankContainer.TryGetUnit(0, out tunit) && tunit.senor.Team != unit.senor.Team)
                {

                    AttackPawnToChank(ch);
                    return true;

                }
            }
        }

        return false;
    }


    private void NeurNet()// Метод бота "Нейронная сеть".
    {
        //Срабатывает триггер того, что бот жив.
        TakeLivePrice();

        //Высчитывание вводных данных
        CalculateInput();

        //Ввод в нейронную сеть входных данных и получение результата.
        float[] output = network.FeedForward(input);

        //Принять действия на основе "решения" нейронной сети
        //На выходе нейронной сети 9 чисел.
        MakeStep(output);
    }

    //Высчитывание вводных для нейронной сети.
    //Находит все соседние клетки в определённом диапозоне относительно позиции бота.
    private void CalculateInput()
    {
        var i = 0;
        float[,] image = new float[map.Width * 2, map.Height * 4];
        var zeroVector = unit.StandChank.CoordinateCurrent;

        var offsetX = 6;
        var offsetY = 6;
        for (int x = (int)zeroVector.x - offsetX; x < zeroVector.x + offsetX; x++)
        {
            for (int y = (int)zeroVector.y - offsetY; y < zeroVector.y + offsetY; y++)
            {
                var chank = map[x, y];
                if (chank == null)
                {
                    input[i] = 0;
                    i++;
                    continue;
                }
                var container = chank.ChankContainer;
                //var slot = container.GetUnitSlots();

                Unit tunit;
                if (container.TryGetUnit(0, out tunit))// slot.ContainsKey(0))// && slot[0] != null)
                {
                    var takeUnit = tunit;//slot[0];
                    input[i] = unit.senor.Team == takeUnit.senor.Team ? -1 : 1;

                    if (takeUnit.nameUnit.Equals("King"))
                    {
                        input[i] *= 2f;
                    }
                    if (takeUnit.Equals(unit))
                    {
                        input[i] *= 0;
                    }
                }
                else
                {
                    input[i] = 1;
                }

                i++;
            }
        }

        if (enemuKing == null)
            enemuKing = Singleton<Manager>.MainSingleton.GetEnemyKings(unit.senor.Team);

        input[input.Length - 2] = unit.nameUnit.Equals("King") ? -1 : 1;

        var vec = (enemuKing.transform.position - transform.position);
        input[input.Length - 2] = vec.x;
        input[input.Length - 1] = vec.y;
    }

    //Предпринимает действия на основе решения нейронной сети.
    //Значения сохраняются в словарь, после словарь сортируется по значениям.
    //По наибольшему значению словаря достаём из него номер(ключ) звена.
    //По номеру звена делаем действие:
    // 0 Атака лево-вперёд (по диагонали)     
    //1 Шаг вперёд
    //2 Атака право-вперёд (по диагонали)  
    //3 Шаг влево
    //4 Пропуск хода
    //5 Шаг вправо
    //6 Атака лево-назад (по диагонали)
    //7 Шаг назад
    //8 Атака право-назад (по диагонали)
    private void MakeStep(float[] output)
    {
        var diction = new Dictionary<int, float>();
        for (var y = 0; y < 9; y++)
        {
            diction.Add(y, output[y]);
        }

        var ordered = diction.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

        var first = ordered.First();

        if (first.Key == 0)
            AttackLF();
        if (first.Key == 1)
            MoveForward();
        if (first.Key == 2)
            AttackRF();
        if (first.Key == 3)
            MoveLeft();
        if (first.Key == 4)
        {
            TakeStayPrice();
            return;
        }
        if (first.Key == 5)
            MoveRight();
        if (first.Key == 6)
            AttackLB();
        if (first.Key == 7)
            MoveBack();
        if (first.Key == 8)
            AttackRB();
    }

    public bool MoveForward()
    {
        var cor = unit.StandChank.CoordinateCurrent + new Vector2(0, 1);
        return MovePawnToChank(map[(int)cor.x, (int)cor.y]);
    }

    public bool MoveLeft()
    {
        var cor = unit.StandChank.CoordinateCurrent + new Vector2(-1, 0);
        return MovePawnToChank(map[(int)cor.x, (int)cor.y]);
    }

    public bool MoveRight()
    {
        var cor = unit.StandChank.CoordinateCurrent + new Vector2(1, 0);
        return MovePawnToChank(map[(int)cor.x, (int)cor.y]);
    }

    public bool MoveBack()
    {
        var cor = unit.StandChank.CoordinateCurrent + new Vector2(0, -1);
        return MovePawnToChank(map[(int)cor.x, (int)cor.y]);
    }

    public void AttackLF()
    {
        var cor = unit.StandChank.CoordinateCurrent + new Vector2(-1, 1);
        AttackPawnToChank(map[(int)cor.x, (int)cor.y]);
    }

    public void AttackRF()
    {
        var cor = unit.StandChank.CoordinateCurrent + new Vector2(1, 1);
        AttackPawnToChank(map[(int)cor.x, (int)cor.y]);
    }
    public void AttackLB()
    {
        var cor = unit.StandChank.CoordinateCurrent + new Vector2(-1, -1);
        AttackPawnToChank(map[(int)cor.x, (int)cor.y]);
    }

    public void AttackRB()
    {
        var cor = unit.StandChank.CoordinateCurrent + new Vector2(1, -1);
        AttackPawnToChank(map[(int)cor.x, (int)cor.y]);
    }

    private void AttackPawnToChank(Chank chank)
    {
        if (chank == null)
            return;

        if (!chank.ChankContainer.IsFree(0))//UnitSlots.ContainsKey(0))
        {
            //if (chank.ChankContainer.UnitSlots[0] == null)
            //    return;

            Unit tunit;
            if (chank.ChankContainer.TryGetUnit(0, out tunit))
            {


                if (unit.senor.Team == tunit.senor.Team)
                {
                    TakeStayPrice();
                    return;
                }

                //Триггер за "рубку" пешки
                TakeKillPrice();

                if (tunit.nameUnit.Equals("King"))
                {
                    Singleton<Manager>.MainSingleton.GameToLoose(tunit.senor.Team, this);
                    Singleton<Manager>.MainSingleton.GameRestartMatch();
                    return;
                }
            }
        }
        else
        {
            return;
        }

        chank.ClearPawn(unit.ChankQueue);

        MovePawnToChank(chank);
    }

    private bool MovePawnToChank(Chank chank)
    {
        if (chank == null)
            return false;

        if (!chank.ChankContainer.IsFree(0))
        {
            TakeStayPrice();
            return false;
        }

        TakeMovePrice();
        unit.StandChank.ChankContainer.ClearAllUnitSlots();

        var pos = spawnerToy.GetDefaultPosition(chank);
        unit.transform.position = pos;
        unit.SwitchStandChank(chank);

        var raund = GetComponent<RaundTickerUnitMOD>();
        if (raund != null)
            raund.DoMoveMOD();

        return true;
    }

    //Метод, который вызывается средой Unity при уничтожении объекта (Бота)
    //При уничтожении объекта показатель эффективности сохраняется в классе нейронной сети.
    private void OnDestroy()
    {
        if (!canSurvive)
            TakeDeadPrice();

        UpdateFitness();
    }

    //Сохранение эффективности в нейронную сеть
    public void UpdateFitness()
    {
        if (typeBot != BotType.NeuralNetwork)
            return;

        network.Fitness += position / Singleton<Manager>.MainSingleton.countRandomMatch;
    }
}

public enum BotType
{
    NeuralNetwork,
    AlgoritmActive,
    AlgoritmPassive,
    Disable,
}