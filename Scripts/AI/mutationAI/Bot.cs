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
    //��� ����
    //� ������ ���������� ������ �������� �������� SOLID. ������� ������ ����� �������� "�����������".
    //������ ��� ����� ������������ ��� ��������� ���� ��� �������� �������, ��� � ��������
    //������ �������� ��������� ��� �������, ��� �� ����� ������������ ��� �������� �������
    public BotType typeBot = BotType.NeuralNetwork;

    //�������
    //���������, ������� ������ �� ������ ����� �������������
    //��� ��������� ����������������� � ���������� � ����� Unity
    public int WantLive = 1;
    public int WantDead = -10;
    public int WantKill = 1;
    public int WantMove = 1;
    public int WantWin = 1;
    public int WantStay = -1;
    public int WantGoToKing = 1;

    //���� � ������� ��� ������� �������� ��������� ����
    private float[] input = new float[6 * 6 * 4 + 6 * 4 + 1 + 3];

    //��� ������������������ ��������� ����, ������� ������� ����������.
    public NeuralNetwork network;

    //��������� ���������� ������������� �� ����� �����.
    public float position;

    private GameMap map;
    public Unit unit;
    private SpawnerToyMOD spawnerToy;
    private Bot enemuKing;

    //���������� ������. ����� ��� ����������� "� ������ �� �����?".
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
        //������� ������ � ����������� ���� ���� �� �������������� � �������
        unit = GetComponent<Unit>();
        if (unit.senor.Team == Game.MapSystems.Enums.GameTeam.Red)
            typeBot = BotType.AlgoritmActive;

        map = Singleton<MapSystem>.MainSingleton.ChanksController.GetMap();
        spawnerToy = unit.GetCardPrefab().GetComponent<SpawnerToyMOD>();
        enemuKing = Singleton<Manager>.MainSingleton.GetEnemyKings(unit.senor.Team);

        //���������� ���� � ���� ����� � ���������.
        //�������� ����� ��������� ��������� ����.
        Singleton<Manager>.MainSingleton.AddBot(this);

        //(����� ������������) �������� �������� ������ ��� �������
        var deb = GameObject.Instantiate(unit.senor.Team == GameTeam.Blue ? prefDebugTeamBlue : prefDebugTeamRed, transform);

    }

    //������ �������� �������/��������� �� ����������� ��������(��������)
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

    //������������� ��������� ���� ��� ��������� � ����������� �� ���� ����
    public void UseBrain()
    {
        if (typeBot == BotType.NeuralNetwork)
            NeurNet();

        if (typeBot == BotType.AlgoritmActive)
            if (!TryAttack())
                TryMove();

    }

    private void TryMove() // ����� ���� "���������". �������� ������� ���. ������ ������� ��������� ������ �� ���������� ������ � ����� �� ��.
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

    private bool TryAttack() // ����� ���� "���������". �������� ������� �����, ���� ���� ��������� �����.
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


    private void NeurNet()// ����� ���� "��������� ����".
    {
        //����������� ������� ����, ��� ��� ���.
        TakeLivePrice();

        //������������ ������� ������
        CalculateInput();

        //���� � ��������� ���� ������� ������ � ��������� ����������.
        float[] output = network.FeedForward(input);

        //������� �������� �� ������ "�������" ��������� ����
        //�� ������ ��������� ���� 9 �����.
        MakeStep(output);
    }

    //������������ ������� ��� ��������� ����.
    //������� ��� �������� ������ � ����������� ��������� ������������ ������� ����.
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

    //������������� �������� �� ������ ������� ��������� ����.
    //�������� ����������� � �������, ����� ������� ����������� �� ���������.
    //�� ����������� �������� ������� ������ �� ���� �����(����) �����.
    //�� ������ ����� ������ ��������:
    // 0 ����� ����-����� (�� ���������)     
    //1 ��� �����
    //2 ����� �����-����� (�� ���������)  
    //3 ��� �����
    //4 ������� ����
    //5 ��� ������
    //6 ����� ����-����� (�� ���������)
    //7 ��� �����
    //8 ����� �����-����� (�� ���������)
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

                //������� �� "�����" �����
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

    //�����, ������� ���������� ������ Unity ��� ����������� ������� (����)
    //��� ����������� ������� ���������� ������������� ����������� � ������ ��������� ����.
    private void OnDestroy()
    {
        if (!canSurvive)
            TakeDeadPrice();

        UpdateFitness();
    }

    //���������� ������������� � ��������� ����
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