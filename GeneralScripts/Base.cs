using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private float maxHealth = 1000;
    [SerializeField] private float currentHealth = 1000;

    private void Start()
    {
        currentHealth = maxHealth;
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
