using Game.Global;
using Game.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonStartLVL : MonoBehaviour
{
    [SerializeField] private int Tom = 0;
    [SerializeField] private int Lvl = 0;
    [SerializeField] private GameObject lockerObj;
    [SerializeField] private bool access = true;
    private void Start()
    {
        access = Singleton<SaveSystem>.MainSingleton.TryGetAccessLevel(Tom, Lvl);
        if (lockerObj != null)
            lockerObj.SetActive(!access);
    }

    public void StartLvl()
    {
        if (!access)
            return;

        Singleton<GlobalManager>.MainSingleton.SetTomeAndLevel(Tom, Lvl);

        Application.backgroundLoadingPriority = ThreadPriority.Low;
        StartCoroutine(Load());
        //SceneManager.LoadSceneAsync(2, LoadSceneMode.Single);
    }

    IEnumerator Load()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(2, LoadSceneMode.Single);
        while (!asyncLoad.isDone)
            yield return null;
    }
}
