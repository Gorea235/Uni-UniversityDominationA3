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

    /// <summary>
    /// Saves the current landmark state to a memento.
    /// </summary>
    /// <returns>The memento of the current landmark state.</returns>
    public SerializableLandmark SaveToMemento()
    {
        return new SerializableLandmark
        {
            resourceType = resourceType,
            amount = amount
        };
    }

    /// <summary>
    /// Restores the landmark state from a memento.
    /// </summary>
    /// <param name="memento">The memento to restore from.</param>
    public void RestoreFromMemento(SerializableLandmark memento)
    {
        resourceType = memento.resourceType;
        amount = memento.amount;
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// The kind of resource that this landmark gives.
    /// </summary>
    public ResourceType ResourceType
    {
        get { return resourceType; }
        set { resourceType = value; }
    }
    /// <summary>
    /// The amount of resource that this landmark gives.
    /// </summary>
    public int Amount
    {
        get { return amount; }
        set { amount = value; }
    }

    #endregion
}
