using System;

[Serializable]
public class SerializableGame
{
    public TurnState turnState;
    public SerializablePlayer[] players;
    public SerializableSector[] sectors;
    public int currentPlayerId;
    public bool pvcEncountered;
    public int? lastDiscovererOfPvcId;
}
