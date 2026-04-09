using Unity.VisualScripting;
using UnityEngine;
using System;

public class Base : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float maxHealth = 1000;
    [SerializeField] private float currentHealth = 1000;
    

    [Header("Spawning")]
    [SerializeField] private UnitPool unitPool;
    [SerializeField] private Transform spawnPoint;

    public static event Action<int> UnitSpawnedEvent;
    private void Start()
    {
        currentHealth = maxHealth;
    }
    public void SpawnUnit(int unitTypeID)
    {
        UnitType unitType = (UnitType)unitTypeID;
        GameObject unit = unitPool.Get(unitType, spawnPoint.position);
        if (unit == null) return;
        unit.GetComponent<UnitController>().Initialize(unitPool);
        UnitSpawnedEvent.Invoke(unitTypeID);
    }
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
            Defeat();
    }
    private void Defeat() // później tutaj dodamy logic przegranej lub wygranej, coś takiego
    {
        Destroy(gameObject, 1);
    }
}
