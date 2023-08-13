using Assets.Scripts.StateMachin;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoxState : State
{
    protected List<Animator> animators;
    protected ManipulationGameBox manipulation;
    protected CameraSwitcher cameraSwitcher;
    public GameBoxState(ManipulationGameBox manipulation, List<Animator> animators, CameraSwitcher cameraSwitcher)
    {
        if (manipulation == null)
            throw new System.Exception("You need set manipulation for gameBOX");

        this.cameraSwitcher = cameraSwitcher;
        this.manipulation = manipulation;
        this.animators = animators;
    }

    public override void Enter(StateMachine machine) { }

    public override void Exit(StateMachine machine) { }

    public override void Update(StateMachine machine) { }

    protected void AnimatorsSetBool(string name, bool active)
    {
        foreach (var anim in animators)
        {
            anim.SetBool(name, active);
        }
    }

    protected void AnimatorsSetTrigger(string name)
    {
        foreach (var anim in animators)
        {
            anim.SetTrigger(name);
        }
    }

    protected void LoadBoxItems(TypeBoxScens type)
    {
        AnimatorsSetTrigger("SwitchContainer");
        manipulation.StartCoroutine(DoActionWithDelay(() => manipulation.LoadItems(type), 1f));
    }

    protected void DoActionDelay(Action action, float secondDelay)
    {
        manipulation.StartCoroutine(DoActionWithDelay(action, 1f));
    }

    private IEnumerator DoActionWithDelay(Action action, float secondDelay)
    {
        yield return new WaitForSeconds(secondDelay);
        action();
    }
}

public class StartGBState : GameBoxState
{
    public StartGBState(ManipulationGameBox manipulation, List<Animator> animators, CameraSwitcher cameraSwitcher) 
        : base(manipulation, animators, cameraSwitcher) { }
    public override void Enter(StateMachine machine)
    {
        manipulation.StartCoroutine(nextStage(machine));
    }
    IEnumerator nextStage(StateMachine machine)
    {
        yield return new WaitForSeconds(2f);
        machine.SwichTo<IdleMainMenuGBState>();
    }
}

public class LoadIdleMainMenuGBState : GameBoxState
{
    public LoadIdleMainMenuGBState(ManipulationGameBox manipulation, List<Animator> animators, CameraSwitcher cameraSwitcher) 
        : base(manipulation, animators, cameraSwitcher) { }

    public override void Enter(StateMachine machine)
    {
        cameraSwitcher.SwitchToUpper();
        LoadBoxItems(TypeBoxScens.MainMenu);
        DoActionDelay(() => machine.SwichTo<IdleMainMenuGBState>(), 1f);
        DoActionDelay(() => cameraSwitcher.SwitchToMain(), 1f);
    }
}


public class IdleMainMenuGBState : GameBoxState
{
    public IdleMainMenuGBState(ManipulationGameBox manipulation, List<Animator> animators, CameraSwitcher cameraSwitcher) 
        : base(manipulation, animators, cameraSwitcher) { }

    public override void Enter(StateMachine machine)
    {
        cameraSwitcher.SwitchToMain();
    }
}

public class OpenSettingsGBState : GameBoxState
{
    public OpenSettingsGBState(ManipulationGameBox manipulation, List<Animator> animators, CameraSwitcher cameraSwitcher) 
        : base(manipulation, animators, cameraSwitcher) { }

    public override void Enter(StateMachine machine)
    {
        AnimatorsSetBool("SettingOpen", true);
        cameraSwitcher.SwitchToSettings();
    }
    public override void Exit(StateMachine machine)
    {
        AnimatorsSetBool("SettingOpen", false);
    }
}

public class OpenManualGBState : GameBoxState
{
    public OpenManualGBState(ManipulationGameBox manipulation, List<Animator> animators, CameraSwitcher cameraSwitcher) 
        : base(manipulation, animators, cameraSwitcher) { }
    public override void Enter(StateMachine machine)
    {
        AnimatorsSetBool("SvitokOpen", true);
        manipulation.ShowGameDescription();
    }
    public override void Exit(StateMachine machine)
    {
        AnimatorsSetBool("SvitokOpen", false);
        manipulation.HideGameDescription();
    }
}

public class OpenNetworkGameGBState : GameBoxState
{
    public OpenNetworkGameGBState(ManipulationGameBox manipulation, List<Animator> animators, CameraSwitcher cameraSwitcher) 
        : base(manipulation, animators, cameraSwitcher) { }
    public override void Enter(StateMachine machine)
    {
        cameraSwitcher.SwitchToMain();
    }
}

public class OpenSingleGameGBState : GameBoxState
{
    public OpenSingleGameGBState(ManipulationGameBox manipulation, List<Animator> animators, CameraSwitcher cameraSwitcher) 
        : base(manipulation, animators, cameraSwitcher) { }

    public override void Enter(StateMachine machine)
    {
        cameraSwitcher.SwitchToUpper();
        LoadBoxItems(TypeBoxScens.Tom);
        DoActionDelay(() => cameraSwitcher.SwitchToMain(), 1f);
    }
}

public class OpenFirstTomState : GameBoxState
{
    public OpenFirstTomState(ManipulationGameBox manipulation, List<Animator> animators, CameraSwitcher cameraSwitcher) 
        : base(manipulation, animators, cameraSwitcher) { }

    public override void Enter(StateMachine machine)
    {
        cameraSwitcher.SwitchToUpper();
        LoadBoxItems(TypeBoxScens.Levels);
        DoActionDelay(() => cameraSwitcher.SwitchToDown(), 1f);
    }
}


public enum TypeBoxScens
{
    MainMenu,
    Tom,
    Levels
}