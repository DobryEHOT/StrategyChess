using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonReset : MonoBehaviour
{
    private Button button;
    public void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ResetButton);
    }

    public void ResetButton() => EventSystem.current.SetSelectedGameObject(null);

}
