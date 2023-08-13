using Assets.Scripts.StateMachin;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManipulationGameBox : MonoBehaviour
{
    [SerializeField]
    private List<Animator> animators;
    [SerializeField]
    private CameraSwitcher cameraSwitcher;
    private SMGameBox gameBox;

    [Header("Buttons")]
    [SerializeField]
    private GameObject buttonBack;

    [Header("LoadItems")]
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject toms;
    [SerializeField] private GameObject levels;

    [Header("Scroll")]
    [SerializeField] private ScrollContent scrollContent;

    private void Start()
    {
        gameBox = new SMGameBox(this, animators, cameraSwitcher);
    }

    public void ShowGameDescription()
    {
        scrollContent.OpenScroll();
    }

    public void HideGameDescription()
    {
        scrollContent.CloseScroll();
    }

    public void LoadItems(TypeBoxScens type)
    {
        //Нарушения принцмпа открытости/закрытости
        //Лучше сделать словарь и доставать по ключу TypeBoxScens некоторый Action
        if (type == TypeBoxScens.MainMenu)
        {
            menu.SetActive(true);
            toms.SetActive(false);
            levels.SetActive(false);
        }
        if (type == TypeBoxScens.Tom)
        {
            menu.SetActive(false);
            toms.SetActive(true);
            levels.SetActive(false);
        }
        if (type == TypeBoxScens.Levels)
        {
            menu.SetActive(false);
            toms.SetActive(false);
            levels.SetActive(true);
        }
    }

    public void StartStateIdleMainMenuBox()
    {
        buttonBack.SetActive(false);
        gameBox.SwichTo<IdleMainMenuGBState>();
    }

    public void StartStateOpenSettingsBox()
    {
        if (gameBox.ActiveState is OpenSettingsGBState)
            StartStateIdleMainMenuBox();
        else
            gameBox.SwichTo<OpenSettingsGBState>();
    }

    public void StartStateOpenManualBox()
    {
        buttonBack.SetActive(true);
        gameBox.SwichTo<OpenManualGBState>();
    }

    public void StartStateOpenNetworkGameBox()
    {
        gameBox.SwichTo<OpenNetworkGameGBState>();
    }

    public void StartStateOpenSingleGameBox()
    {
        gameBox.SwichTo<OpenSingleGameGBState>();
    }

    public void StartStateLoadMainMenu()
    {
        gameBox.SwichTo<LoadIdleMainMenuGBState>();
    }

    public void StartStateLoadFirstTom()
    {
        gameBox.SwichTo<OpenFirstTomState>();
    }

}

public class SMGameBox : StateMachine
{
    private ManipulationGameBox manipulation;
    private List<Animator> animators;
    private CameraSwitcher cameraSwitcher;
    public SMGameBox(ManipulationGameBox manipulation, List<Animator> animators, CameraSwitcher cameraSwitcher)
    {
        this.cameraSwitcher = cameraSwitcher;
        this.animators = animators;
        this.manipulation = manipulation;
        InicializeMachine();
    }

    protected override void InicializeMachine()
    {
        AddState(new StartGBState(manipulation, animators, cameraSwitcher));
        AddState(new IdleMainMenuGBState(manipulation, animators, cameraSwitcher));
        AddState(new OpenSettingsGBState(manipulation, animators, cameraSwitcher));
        AddState(new OpenManualGBState(manipulation, animators, cameraSwitcher));
        AddState(new OpenNetworkGameGBState(manipulation, animators, cameraSwitcher));
        AddState(new OpenSingleGameGBState(manipulation, animators, cameraSwitcher));
        AddState(new OpenFirstTomState(manipulation, animators, cameraSwitcher));
        AddState(new LoadIdleMainMenuGBState(manipulation, animators, cameraSwitcher));

        SwichTo<StartGBState>();
    }
}
