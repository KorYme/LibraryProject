using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using ToolLibrary;

public class PlayerStats : MonoBehaviour
{
    [GDModif]
    [SerializeField]
    private string _playerName;

    [GDModif]
    [SerializeField]
    private Vector3 _currentHealth;

    [GDModif]
    [SerializeField]
    private int _maxHealth;

    [GDModif]
    [SerializeField]
    private float _speed;

    [GDModif]
    [SerializeField]
    private float _damage;

    [GDModif]
    [SerializeField]
    private Rigidbody rb;
}
