using Assets.Scripts.SelectorSpace;
using Game;
using Game.Singleton;
using Game.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private SelectorItems selector;
    [SerializeField] private GameObject board;
    [SerializeField] private float speedMove = 10f;
    [SerializeField] private Transform openTransform;
    [SerializeField] private Transform closedTransform;

    private MainScreen screen;
    private bool isDown = false;
    private Transform nextPosition;
    private float errorDistanceBoard = 0.2f;

    void Start()
    {
        screen = Singleton<MainScreen>.MainSingleton;
        selector = Singleton<LochalClientInformation>.MainSingleton.Info.player.GetComponent<SelectorItems>();
        nextPosition = openTransform;
    }

    void Update()
    {
        if (!isDown && board.activeSelf
            && (board.transform.position == nextPosition.position
            || (board.transform.position - nextPosition.position).magnitude <= errorDistanceBoard))
        {
            board.SetActive(false);
            return;
        }

        if (board.activeSelf)
            board.transform.position = Vector3.Lerp(board.transform.position, nextPosition.position, Time.deltaTime * speedMove);
    }

    public void SwithMenu()
    {
        board.SetActive(true);
        isDown = isDown ? false : true;

        screen.SetActiveIcons(!isDown);
        Transform tran;
        if (isDown)
            tran = openTransform;
        else
            tran = closedTransform;

        selector.ActiveVizor = !isDown;
        Singleton<GameManager>.MainSingleton.PauseIsActive = isDown;
        nextPosition = tran;
    }

    public void Pause()
    {
        selector.ActiveVizor = false;
        Singleton<GameManager>.MainSingleton.PauseIsActive = true;
    }

    public void Continue()
    {
        selector.ActiveVizor = true;
        Singleton<GameManager>.MainSingleton.PauseIsActive = false;
    }

    public void Restart() => SceneManager.LoadSceneAsync(2, LoadSceneMode.Single);

    public void Exit() => selector.ActiveVizor = false;
}
