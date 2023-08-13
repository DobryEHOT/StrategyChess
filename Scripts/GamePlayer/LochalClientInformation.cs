using Game;
using Game.MapSystems.Enums;
using Game.Singleton;
using System;

public class LochalClientInformation : Singleton<LochalClientInformation>
{
    public ClientInfo Info;

    private void Awake() => Inicialize(this);
}

[Serializable]
public struct ClientInfo
{
    public string Name;
    public GameTeam Team;
    public Player player;
}