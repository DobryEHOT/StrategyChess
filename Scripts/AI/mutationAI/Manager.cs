using Assets.Scripts.GameManager.States;
using Game;
using Game.AI;
using Game.MapSystems;
using Game.MapSystems.Enums;
using Game.Singleton;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using System.Text;

public class Manager : Singleton<Manager>
{
    //������ ������ � ������� ������ ���������.
    //����� ����� ��������, ����� �������� ��������� ��������.
    public bool OnlyBestPopulation = false;

    //������ ��������� �� ���������� ������
    public bool startWithStrongRandom = false;

    //������� ����� ���� �� �����������. ����� ���������� ����� ���������� ������������ � ������ ����
    public bool startNewStatistic = false;

    //���������� "������� ����".
    //��� ����� ���������� ����� ������� ������� ����� ������� �������������� ��������� ��������� �����.
    public float lifeRaunds = 40;
    [Range(2, 100)] public int populationSize;
    public int countBadPopulationHaveChilds = 2;
    [Range(1, 100)] public int countRandomMatch = 5;

    //���� ��������� ����. ������� ��� ���������� ����. ����� ��� ���������� �������.
    //������ �������� ������������ � ��������� Unity ����� ���������.
    public int[] layers = new int[5] { 576, 576, 288, 288, 9 };


    [Range(0.0001f, 1f)] public float ProcentMutation = 0.99f;
    [Range(0.0001f, 1f)] public float MutationChance = 0.01f;

    [Range(0f, 1f)] public float MutationStrength = 0.5f;

    [Range(0.1f, 100f)] public float Gamespeed = 1f;


    public List<NeuralNetwork> networks;
    public List<OutputActions> outputActions;
    public List<Bot> bots = new List<Bot>();
    private List<Bot> botKings = new List<Bot>();
    private int netNumber;
    private int ticker;
    private int raundTicker;
    private int countRandomMatchNow = 0;

    //������ ���������� �������������.
    private float bestFit = -10000f;

    private int countMatches = 0;
    Bot[] listGoToKing;
    GameTeam looseTeam;
    private void Awake()
    {
        Inicialize(this);

        if (populationSize % 2 != 0)
            populationSize = 50;

        InitNetworks();
    }

    void Start()
    {
        //������ ������� �����
        StartCoroutine(st());
    }

    public void InitNetworks()
    {
        networks = new List<NeuralNetwork>();
        for (int i = 0; i < populationSize; i++)
        {
            //������������� ������ ��������� ����
            NeuralNetwork net = new NeuralNetwork(layers, i);

            //�������� ���������� ���������
            if (OnlyBestPopulation)
                net.Load($"Assets/Save-BestSoldier.txt");
            else if (!startWithStrongRandom)
                net.Load($"Assets/Save-Soldier{i % (populationSize / 2)}.txt");

            networks.Add(net);

            //������ �������, ������� ��� ������ ������� ������ ������������� ���� ��������� ����.
            if (startWithStrongRandom)
                networks[i].Mutate((int)1, Random.Range(0, 100));
        }

        //�������� ����� ��� ����� ����������.
        //� ���� ���� ����������� ���������� ������������� ������ ���������, ����� ���� ��� ������ ��������� ���� ������������ ���������.
        if (startNewStatistic)
            CreateStatistic("Assets/Statistics.txt");
    }

    public void GameRestartMatch()
    {
        //������� ������������� ������ ��� ������� ���� ����������� ������.
        //��� ������ �������� ��������� �� ����������� � ����������� ������������� ����� �����.
        RefreshFitPosition();

        //������� ���� �������� ����� � ��������� �� Fitnes (���������� �������������) � ����� ��������� ����
        DestroyBots();

        //����� ����� � ��� unity editor
        Debug.Log($"Matches finished {countMatches} : numberNN {activePopulation} {{NN_ID = {networks[activePopulation].ID}}} (fit={networks[activePopulation].Fitness}) : countRandomMatchNow {countRandomMatchNow} : LooseTeam = {looseTeam}");

        //������ ����� ������� (��� ������ ����������� ����� �������� ���� �����)
        RefreshFitPositionStep2();

        //�������������� ������� �����
        Singleton<MapSystem>.MainSingleton.ChanksController.DeAttachUnitsOnMap();

        //����������� ��������� ��� ���������.
        NextPopulation();

        //������� ����� ������
        ClearGlobalData();
    }

    private void RefreshFitPositionStep2()
    {
        if (looseTeam == GameTeam.Neutral)
            activePopulation--;
        looseTeam = GameTeam.Neutral;
    }

    private void RefreshFitPosition()
    {
        if (looseTeam == GameTeam.Neutral)
        {
            foreach (var b in bots)
                b.position = 0;
        }
    }

    private void ClearGlobalData()
    {
        botKings = new List<Bot>();
        bots = new List<Bot>();
        netNumber = 0;

        Time.timeScale = Gamespeed;
        raundTicker = 0;
        magForKing = 0;

        if (outputActions != null)
            foreach (var act in outputActions)
                act.RestartCards();
    }

    private void DestroyBots()
    {
        if (bots != null)
        {
            for (int i = 0; i < bots.Count; i++)
            {
                if (bots[i] != null)
                {
                    bots[i].canSurvive = true;
                    GameObject.Destroy(bots[i].gameObject);
                }
            }
        }
    }

    //�����, ������� ���������� ��� ��������� ����� �� ������
    public void GameToLoose(GameTeam looseTeam, Bot sender)
    {
        this.looseTeam = looseTeam;

        //�������, ������� ����������� ��� ������
        if (sender != null)
            if (sender.unit.senor.Team != looseTeam)
                sender.TakeWinPrice();

        if (bots != null)
        {
            for (int i = 0; i < bots.Count; i++)
            {
                if (bots[i] != null)
                {
                    if (bots[i].unit.senor.Team == looseTeam)
                        bots[i].TakeDeadPrice();
                }
            }
        }
    }

    public Bot GetEnemyKings(GameTeam myTeam)
    {
        foreach (var king in botKings)
            if (king.unit.senor.Team != myTeam)
                return king;

        return null;
    }

    //�������� ���� � ���� �������� �����
    //��� �������� ����, �� ����������� ���� ������ ����� ������.
    public void AddBot(Bot bot)
    {
        if (bot.unit.nameUnit.Equals("King"))
            botKings.Add(bot);

        if (bot.typeBot == BotType.NeuralNetwork)
        {
            bot.network = networks[activePopulation];
            netNumber++;
        }

        bots.Add(bot);
    }

    private void FixedUpdate()
    {
        //������ 5 ����� �������� ������� ��� ����������� ������� � �������� ����� ���� ������
        if (ticker > 5)
        {
            if (bots.Count >= 20)
                NextRaund();
            ticker = 0;
            raundTicker++;
        }

        // �������������� ���������� ����� ��� ���������� ����
        // ������ ������� ����� ��� ���������� ������� ������� �� "���". �������� ���� �� ������ ����� ������� ���
        if (raundTicker >= lifeRaunds)
            GameRestartMatch();

        ticker++;
    }

    float magForKing = 0;
    public void NextRaund()
    {
        //���������� ������
        GameTeam acTeam;
        Bot king;
        PreData(out acTeam, out king);

        //������������ "�����" �������� �������.
        //� ������� ������� ��� ��������.
        //� ����� ������� ��� ��������� ����.
        UseTeamBrains(acTeam);

        //���������� ������ fitness, ������� ������� �� ��������� �� ���������� ������.
        //���� ����� �������� ������ ��� ���� � ��������� �����.
        TryGetFitnessDistanceToKing(acTeam, king);

        //�������� ����� ���� ��������� �������
        if (Singleton<GameStatesController>.MainSingleton.Active is ProcessGameState active)
            active.ForcePassMove();
    }

    private void PreData(out GameTeam acTeam, out Bot king)
    {
        acTeam = Singleton<GameManager>.MainSingleton.ActiveTeam;
        int countBestBots = 3;
        listGoToKing = new Bot[countBestBots];
        king = GetEnemyKings(GameTeam.Blue);
    }

    private void UseTeamBrains(GameTeam acTeam)
    {
        foreach (var bot in bots)
        {
            if (bot != null)
                if (bot.unit.senor.Team == acTeam)
                    bot.UseBrain();
        }
    }

    private int TryGetFitnessDistanceToKing(GameTeam acTeam, Bot king)
    {
        int countNN = 0;

        foreach (var bot in bots)
        {
            if (bot != null)
            {
                if (bot.unit.senor.Team == acTeam)
                {
                    if (bot.typeBot == BotType.NeuralNetwork)
                    {
                        countNN++;
                        var mag = -SqrtMagnitudePawnToKing(bot, king) * 0.01f * bot.WantGoToKing;
                        if (mag < magForKing)
                            magForKing = mag;
                    }
                }
            }
        }

        networks[activePopulation].Fitness += (20 * 20 * 0.01f - Mathf.Abs(magForKing)) / countRandomMatch;
        return countNN;
    }

    private float SqrtMagnitudePawnToKing(Bot pawn, Bot king)
    {
        return (pawn.transform.position - king.transform.position).sqrMagnitude;
    }

    int activePopulation = 0;
    bool firstSort = true;
    NeuralNetwork nowNN;
    public void NextPopulation()
    {
        if (nowNN == null)
            nowNN = networks[0];

        List<Task> tasks = new List<Task>();
        //������ �������� ��������� ��������������� ��� ��������� ������ ���.
        //��������� ����� ������� ��������� ����� ������ � ��������� ������������.
        if (activePopulation >= (populationSize / 2) - 1 && (!firstSort || activePopulation >= (populationSize) - 1))//(numMut >= (populationSize) - 1)
            SortNetworks(tasks);
        else
            activePopulation++;


    }

    private void SortNetworks(List<Task> tasks)
    {
        if (countRandomMatchNow >= countRandomMatch)
        {
            //������ �������� ���������� ��� ������ ���������.
            //��� ����� ��� ������� ��������� ������������ ������ ������ ���������.
            if (!OnlyBestPopulation)
            {
                //������������� ���� ��������� ����� �� ���������� ������������� Fitness
                networks.Sort();

                //���������� � ���� �������� ������
                ShowBadPopulations();

                //��������� �������� ������ ���������
                SaveBestPopulations();

                //����� �������������� �������.
                //����� ��� ���������� ����� "�������� ��������" ���������. �������� ��������� ���� ��� ��������� ��������� ����� ������.
                //�� ������� ��� ����� � �� �����������, ��� ��� � ���� �� ���� ������� ��������, ����� ��� ��������� ����� "��� ����".
                TryForceMutation(tasks);

                //����������� �������
                //�������� ��������� ������ ���������� � �������� �������� ������, ����� � ������ �������� ���������� ����������� ����� �� �������
                //(����������� ��������� �������� � ����������� ������������)
                DefaultMutation(tasks);

                //������� ����������� ������������� � ��������, ������� ���� �������� ������������� ������� ������.
                //��������� ������ ��� ��������, ������ ��� �������� ������ �� ����� ������������. ��� ������
                //��������� ���� ������������� � ���� ���� �� ��� ������ ���������.
                ClearBadFit();

                //�������� ���� �������. (��� ��������. ����.)
                if (tasks.Count > 0)
                    Task.WaitAll(tasks.ToArray());

                //��������� ��������� �������. ������ ����� ��������� �� ��� ���� ����� �� ������� ���������� �������� ���� �������� ��������.
                CurrectMutationValues();
            }

            firstSort = false;
            countMatches++;
            countRandomMatchNow = 0;
            activePopulation = 0;
        }
        else
        {
            countRandomMatchNow++;
            activePopulation = 0;
        }
    }

    private void CurrectMutationValues()
    {
        MutationChance *= ProcentMutation;
        MutationStrength *= ProcentMutation;
    }

    private void ClearBadFit()
    {
        for (int i = 0; i < (populationSize / 2); i++)
            networks[i].Fitness = 0;
    }

    private void DefaultMutation(List<Task> tasks)
    {
        for (int i = 0; i < (populationSize / 2); i++)
        {
            int temp = i;

            tasks.Add(Task.Factory.StartNew(() => CurrectLayers(temp)));
        }
    }

    private void TryForceMutation(List<Task> tasks)
    {
        if (countBadPopulationHaveChilds < populationSize)
        {

            for (int i = countBadPopulationHaveChilds; i < populationSize; i++)
            {
                int temp = i;
                if (networks[temp].Fitness > 0)
                    continue;

                tasks.Add(Task.Factory.StartNew(() => ForceMutateLayers(temp)));
            }
            if (tasks.Count > 0)
                Task.WaitAll(tasks.ToArray());
        }

        tasks.Clear();
    }

    private void SaveBestPopulations()
    {
        for (int i = populationSize / 2; i < populationSize; i++)
        {
            var fit = networks[i].Fitness;
            Debug.Log($"Best-Fitness {{NN_ID = {networks[i].ID}}} num[{i}] -> {fit}");
            networks[populationSize - 1].Save($"Assets/Save-Soldier{-(populationSize / 2) + i}.txt");//saves networks weights and biases to file, to preserve network performance
            if (bestFit < fit)
            {
                bestFit = fit;
                networks[populationSize - 1].Save($"Assets/Save-BestSoldier.txt");
            }

        }
        SaveStatistic("Assets/Statistics.txt", networks[populationSize - 1].Fitness);
    }

    private void ShowBadPopulations()
    {
        for (int i = 0; i < populationSize / 2; i++)
        {
            var fit = networks[i].Fitness;
            Debug.Log($"Bad-Fitness {{NN_ID = {networks[i].ID}}} num[{i}] -> {fit}");
        }
    }

    private void CurrectLayers(int i)
    {
        //������ �� ������ �������� � ������ �� ������
        networks[i] = networks[i + (populationSize / 2)].copy(new NeuralNetwork(layers, networks[i].ID));

        //��������
        networks[i].Mutate((int)(1 / MutationChance), MutationStrength);
    }

    private void ForceMutateLayers(int i)
    {
        networks[i].Mutate(1, 10);
    }

    private void SaveStatistic(string path, float bestFit)
    {
        var file = File.Open(path, FileMode.OpenOrCreate);
        file.Close();
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(bestFit);
        writer.Close();
    }
    private void CreateStatistic(string path)
    {
        File.Create(path).Close();
    }

    IEnumerator st()
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        GameRestartMatch();

    }
}