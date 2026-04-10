using Unity.VisualScripting;
using UnityEngine;
using System;

public class Base : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject endScreen;
    [SerializeField] private GameObject winStatus;

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
    private void Defeat() 
    {
        winStatus.SetActive(true);
        endScreen.SetActive(true);
        Destroy(gameObject, 1);
        Time.timeScale = 0.0f;
    }
}
