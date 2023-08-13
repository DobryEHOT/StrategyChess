using Game.Singleton;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelDescripton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private List<string> descriptins = new List<string>();
    void Start()
    {
        ActiveDescription();
    }

    private void ActiveDescription()
    {
        for (var i = 6; i > 0; i--)
        {
            if (Singleton<SaveSystem>.MainSingleton.TryGetAccessLevel(1, i))
            {
                var index = i - 1;
                if (descriptins.Count > 0 && descriptins.Count >= index)
                    textMesh.text = descriptins[index];

                return;
            }
        }
    }
}
