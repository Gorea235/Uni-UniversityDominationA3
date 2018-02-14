using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;


[Serializable]

public class SerializableColor
{
    [SerializeField]

    float[] _color = new float[4];

    public Color Color
    {
        get
        {
            return new Color(_color[0], _color[1], _color[2], _color[3]);
        }
        set
        {
            _color[0] = value.r;
            _color[1] = value.g;
            _color[2] = value.b;
            _color[3] = value.a;
        }
    }

    public static explicit operator SerializableColor(Color v)
    {
        SerializableColor casted = new SerializableColor();
        casted.Color = v;
        return casted;

    }

    public static explicit operator Color(SerializableColor v)
    {
        Color casted = new Color();
        casted = v.Color;
        return casted;
    }
}
