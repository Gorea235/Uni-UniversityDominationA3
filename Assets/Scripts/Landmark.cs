using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landmark : MonoBehaviour
{
    #region Private Fields

    ResourceType resourceType;
    int amount = 2;

    #endregion

    #region Serialization

    public SerializableLandmark SaveToMemento()
    {
        return new SerializableLandmark
        {
            resourceType = resourceType,
            amount = amount
        };
    }

    public void RestoreFromMemento(SerializableLandmark memento)
    {
        resourceType = memento.resourceType;
        amount = memento.amount;
    }

    #endregion

    #region Public Properties

    public ResourceType ResourceType
    {
        get { return resourceType; }
        set { resourceType = value; }
    }

    public int Amount
    {
        get { return amount; }
        set { amount = value; }
    }

    #endregion
}
