using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RaundController : MonoBehaviour
{
    [SerializeField] private Animator showerYourMove;
    [SerializeField] private Animator timePasser;
    [SerializeField] private TextMeshPro time;
    [SerializeField] private TextMeshPro moveText;
    [SerializeField] private GameObject WinBoard;
    [SerializeField] private GameObject LooseBoard;
    private GameObject timePasserObj;
    private string enemyMoveText = "Ход вашего противника";
    private string youMoveText = "Ваш ход";

    private void Start()
    {
        if (timePasser != null)
            timePasserObj = timePasser.gameObject;
    }

    public void ShowWin() => ShowEndWindow(WinBoard);

    public void ShowLoose() => ShowEndWindow(LooseBoard);

    private void ShowEndWindow(GameObject endWindow)
    {
        if (endWindow != null)
            endWindow.SetActive(true);

        if (timePasserObj != null)
            timePasserObj.SetActive(false);
    }

    public void ShowYourMove()
    {
        if (showerYourMove != null)
            showerYourMove.SetTrigger("DoPass");

        SwitchText(youMoveText);
    }

    public void ShowRotateTime()
    {
        if (showerYourMove != null)
            timePasser.SetTrigger("DoPass");
    }

    public void StartShowTime()
    {
        if (showerYourMove != null)
            timePasser.SetTrigger("DoTime");
    }

    public void SetTime(int time)
    {
        if (this.time != null)
            this.time.text = time.ToString();
    }

    public void ShowMoveEnemy() => SwitchText(enemyMoveText);

    private void SwitchText(string text) => StartCoroutine(DoWithDelay(() => moveText.text = text, 1));

    IEnumerator DoWithDelay(Action action, float second)
    {
        yield return new WaitForSeconds(second);
        action();
    }
}
