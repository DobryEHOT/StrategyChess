using Game.Global;
using Game.Singleton;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelStarter : MonoBehaviour
{
    public int Tom { get; set; } = 0;
    public int Level { get; set; } = 1;
    public void StartLevel()
    {
        Singleton<GlobalManager>.MainSingleton.SetTomeAndLevel(Tom, Level);
        SceneManager.LoadSceneAsync(2, LoadSceneMode.Single);
    }
}
