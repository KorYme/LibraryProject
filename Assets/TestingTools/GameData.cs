using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int _deathCount;
    public Vector3 _position;

    public GameData()
    {
        _deathCount = 0;
        _position = Vector3.zero;
    }
}
