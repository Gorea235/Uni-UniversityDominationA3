using System;

[Serializable]
public class SerializableGame
{
    public TurnState turnState;
    public SerializablePlayer[] players;
    public SerializableSector[] sectors;
    public int currentPlayerId;
    public bool PVCEncountered;
    public int? LastDiscovererOfPVCid;
}
