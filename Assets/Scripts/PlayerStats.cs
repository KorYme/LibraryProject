using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [ExposedField]
    [SerializeField]
    private string _playerName;

    [ExposedField]
    [SerializeField]
    private int _currentHealth;

    [ExposedField]
    [SerializeField]
    private int _maxHealth;

    [ExposedField]
    [SerializeField]
    private float _speed;

    [ExposedField]
    [SerializeField]
    private float _damage;

    [ExposedField]
    [SerializeField]
    private Rigidbody rb;
}
