using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitToMenu : MonoBehaviour
{
    public List<GameObject> destroyFromExit = new List<GameObject>();
    public void ExitMenu()
    {
        foreach (var obj in destroyFromExit)
            Destroy(obj);

        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
}
