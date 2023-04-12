using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KorYmeLibrary.SaveSystem;

public class TestSave : MonoBehaviour, IDataSave
{
    private int _deathCount;
    private Vector3 _position;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            _deathCount++;
            Debug.Log("DeathCount : " + _deathCount);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            _position = new Vector3(0, 0, _position.z + 1);
            Debug.Log("Position : " + _position);
        }
    }

    public void LoadData(GameData gameData)
    {
        this._deathCount = gameData._deathCount;
        this._position = gameData._position;
    }

    public void SaveData(ref GameData gameData)
    {
        gameData._deathCount = this._deathCount;
        gameData._position = this._position;
    }
}
