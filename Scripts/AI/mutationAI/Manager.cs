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
    //Запуск только с показом лучшей популяции.
    //Нужен после обучения, чтобы смотреть результат обучения.
    public bool OnlyBestPopulation = false;

    //Запуск популяций со случайными весами
    public bool startWithStrongRandom = false;

    //Создать новый файл со статистикой. Иначе статистика будет продолжать записываться в старый файл
    public bool startNewStatistic = false;

    //Количество "передач хода".
    //Эта цифра показывает через сколько раундов будет вызвана принудительная остановка симуляции матча.
    public float lifeRaunds = 40;
    [Range(2, 100)] public int populationSize;
    public int countBadPopulationHaveChilds = 2;
    [Range(1, 100)] public int countRandomMatch = 5;

    //Слои Нейронной сети. Дляинна это количество слоёв. Число это количество звеньев.
    //Данный параметр регулируется в редакторе Unity через инспектор.
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

    //Лучший показатель эффективности.
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
        //Запуск первого матча
        StartCoroutine(st());
    }

    public void InitNetworks()
    {
        networks = new List<NeuralNetwork>();
        for (int i = 0; i < populationSize; i++)
        {
            //Инициализация пустой нейронной сети
            NeuralNetwork net = new NeuralNetwork(layers, i);

            //Загрузка сохранённых популяций
            if (OnlyBestPopulation)
                net.Load($"Assets/Save-BestSoldier.txt");
            else if (!startWithStrongRandom)
                net.Load($"Assets/Save-Soldier{i % (populationSize / 2)}.txt");

            networks.Add(net);

            //Первая мутация, которая при первом запуске сильно рандомизирует веса нейронной сети.
            if (startWithStrongRandom)
                networks[i].Mutate((int)1, Random.Range(0, 100));
        }

        //Создание файла для сбора статистики.
        //В этот файл добавляется показатель эффективности лучшей популяции, после того как лучшие популяции дали мутированное потомство.
        if (startNewStatistic)
            CreateStatistic("Assets/Statistics.txt");
    }

    public void GameRestartMatch()
    {
        //Костыль отлавливающий случай при котором игра закончилась ничьёй.
        //При данном сценарии результат не учитывается и запускается переигрывание этого матча.
        RefreshFitPosition();

        //Удалить всех выживших ботов и перенести их Fitnes (показатель эффективности) в класс нейронной сети
        DestroyBots();

        //Дебаг пишет в лог unity editor
        Debug.Log($"Matches finished {countMatches} : numberNN {activePopulation} {{NN_ID = {networks[activePopulation].ID}}} (fit={networks[activePopulation].Fitness}) : countRandomMatchNow {countRandomMatchNow} : LooseTeam = {looseTeam}");

        //Вторая часть костыля (она должна выполняться после удаления всех ботов)
        RefreshFitPositionStep2();

        //Дополнительная очистка карты
        Singleton<MapSystem>.MainSingleton.ChanksController.DeAttachUnitsOnMap();

        //Переключить популяцию для симуляции.
        NextPopulation();

        //Очистка общих данных
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

    //Метод, который вызывается при проигрыше одной из команд
    public void GameToLoose(GameTeam looseTeam, Bot sender)
    {
        this.looseTeam = looseTeam;

        //Триггер, который срабатывает при победе
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

    //Добавить бота в лист активных ботов
    //При создании бота, он добавляется путём вызова этого метода.
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
        //Каждые 5 тиков пытается сделать ход действующей команды и передать право хода другой
        if (ticker > 5)
        {
            if (bots.Count >= 20)
                NextRaund();
            ticker = 0;
            raundTicker++;
        }

        // Принудительный перезапуск матча при длительной игре
        // Данная функция нужна для отсеивания случаев похожих на "Пат". Алгоритм Бота не всегда может сделать ход
        if (raundTicker >= lifeRaunds)
            GameRestartMatch();

        ticker++;
    }

    float magForKing = 0;
    public void NextRaund()
    {
        //Подготовка данных
        GameTeam acTeam;
        Bot king;
        PreData(out acTeam, out king);

        //Использовать "мозги" активной команды.
        //У красной команды это алгоритм.
        //У синей команды это нейронная сеть.
        UseTeamBrains(acTeam);

        //Попытаться выдать fitness, который зависит от дистанции до вражеского короля.
        //Этот метод работает только для бота с нейронной сетью.
        TryGetFitnessDistanceToKing(acTeam, king);

        //Передача права хода следующей команде
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
        //Данная проверка позволяет просимулировать все популяции первый раз.
        //Симуляции после первого потомства будут только у потомства мутировавших.
        if (activePopulation >= (populationSize / 2) - 1 && (!firstSort || activePopulation >= (populationSize) - 1))//(numMut >= (populationSize) - 1)
            SortNetworks(tasks);
        else
            activePopulation++;


    }

    private void SortNetworks(List<Task> tasks)
    {
        if (countRandomMatchNow >= countRandomMatch)
        {
            //Данная проверка необходима для режима просмотра.
            //Это режим при котором постоянно показывается только лучшая популяция.
            if (!OnlyBestPopulation)
            {
                //Отсортировать лист нейронных сетей по показателю эффективности Fitness
                networks.Sort();

                //Отобразить в логе половину худших
                ShowBadPopulations();

                //Сохранить половину лучших популяций
                SaveBestPopulations();

                //Опция принудительной мутации.
                //Нужна для добавления новых "векторов развития" популяций. Подобное требуется если все имеющиеся популяции очень похожи.
                //По дефолту эту опцию я не использовал, так как у меня не было столько итераций, чтобы все популяции стали "как одна".
                TryForceMutation(tasks);

                //Стандартная мутация
                //Половина популяции лучших копируется и заменяет половину худших, после у второй половины изменяются коофициенты весов на звеньях
                //(добавляется случайные величины с определённой вероятностью)
                DefaultMutation(tasks);

                //Очистка показателей эффективности у половины, которая была заменена мутированными копиями лучших.
                //Очищается только эта половина, потому что половина лучших не будет переигрывать. Они просто
                //сохраняют свою эффективность и ждут пока их кто нибудь превзойдёт.
                ClearBadFit();

                //Ожидание всех потоков. (Для распарал. прог.)
                if (tasks.Count > 0)
                    Task.WaitAll(tasks.ToArray());

                //Корректор значеений мутации. Каждый раунд уменьшает их для того чтобы на большом количестве итераций чуть улучшить точность.
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
        //Одного из лучших копируем в одного из худших
        networks[i] = networks[i + (populationSize / 2)].copy(new NeuralNetwork(layers, networks[i].ID));

        //Мутируем
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