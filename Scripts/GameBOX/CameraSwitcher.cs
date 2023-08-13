using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    [SerializeField] private float speedMove = 10f;
    [SerializeField] private Transform main;
    [SerializeField] private Transform settings;
    [SerializeField] private Transform upper;
    [SerializeField] private Transform down;
    private Transform myTransform;
    private Transform activeTransform;

    public void Awake()
    {
        myTransform = transform;
        activeTransform = main;
    }

    public void SwitchToMain() => activeTransform = main;
    public void SwitchToSettings() => activeTransform = settings;
    public void SwitchToUpper() => activeTransform = upper;
    public void SwitchToDown() => activeTransform = down;

    void Update()
    {
        myTransform.position = Vector3.Lerp(myTransform.position, activeTransform.position, speedMove * Time.deltaTime);
    }
}
