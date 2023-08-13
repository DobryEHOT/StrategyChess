using Game;
using Game.Singleton;
using Game.UI;
using Game.Units;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconOnScreen : MonoBehaviour
{
    [SerializeField] private bool UseTeamColor;
    [SerializeField] private Sprite icon;
    [SerializeField] private Vector3 offSet;
    [SerializeField] private TeamInfo teamInfo;

    private Camera mCamera;
    private Camera resCamera;
    private Canvas canvas;
    private GameObject spawned;
    private SpriteRenderer spawnedSprite;
    private Player player;

    private bool isInicialized = false;

    private Transform spawnedTransform;
    private Transform myTransform;
    private Transform resCameraTransform;

    private Vector3 posOnScreen;
    private Vector3 size = new Vector3(3f, 3f, 3f);
    void Start()
    {
        myTransform = transform;
    }

    /// <summary>
    /// Крайне прожорливый метод.
    /// В будующем лучше придумать что-то другое.
    /// </summary>
    void Update()
    {
        TryInicialize();

        if (!isInicialized)
            return;

        CalculatePositionOnScreen();
    }

    private void CalculatePositionOnScreen()
    {
        posOnScreen = mCamera.WorldToScreenPoint(myTransform.position + offSet);
        posOnScreen = resCamera.ScreenToWorldPoint(posOnScreen);
        spawnedTransform.position = posOnScreen;
        spawnedTransform.LookAt(resCameraTransform.forward + spawnedTransform.position);
    }

    private void OnDestroy()
    {
        Singleton<MainScreen>.MainSingleton.RemoveIconScreen(gameObject);
        Destroy(spawned);
    }

    public void SetAlpha(float power)
    {
        if (!isInicialized)
            return;

        teamInfo.teamColor.a = Mathf.Clamp(power, 0f, 1f);
        spawnedSprite.color = teamInfo.teamColor;
    }

    private bool TryInicialize()
    {
        if (isInicialized)
            return true;

        player = Singleton<LochalClientInformation>.MainSingleton.Info.player;
        if (player == null)
            return false;

        TrySettingTeamColor();

        mCamera = player.MainCamera;
        resCamera = player.ResourcesCamera;

        InitGO();
        InitSprite();

        Singleton<MainScreen>.MainSingleton.AddIconScreen(spawned);
        isInicialized = true;

        return true;
    }

    private void InitSprite()
    {
        if (isInicialized)
            return;

        var image = spawned.AddComponent<SpriteRenderer>();
        image.sprite = icon;
        image.color = teamInfo.teamColor;
        image.sortingOrder = -10;
        spawnedSprite = image;
    }

    private void InitGO()
    {
        if (isInicialized)
            return;

        spawned = new GameObject("Icon");
        spawned.layer = 11;
        spawned.transform.SetParent(transform);
        spawnedTransform = spawned.transform;
        resCameraTransform = resCamera.transform;
        spawnedTransform.LookAt(resCameraTransform.forward + spawnedTransform.position);
        spawnedTransform.localScale = size;
        spawned.transform.position = resCamera.WorldToScreenPoint(transform.position);
    }

    private void TrySettingTeamColor()
    {
        if (UseTeamColor)
        {
            var unit = GetComponent<Unit>();
            if (unit != null)
                Singleton<GameManager>.MainSingleton.TryGetTeamInfo(unit.senor.Team, out teamInfo);
        }
    }
}
