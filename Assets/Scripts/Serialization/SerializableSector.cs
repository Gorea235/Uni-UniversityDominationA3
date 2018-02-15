using System;

[Serializable]
public class SerializableSector
{
    public SerializableUnit unit;
    public SerializableLandmark landmark;
    public int ownerId;
    public bool PVC;
}
