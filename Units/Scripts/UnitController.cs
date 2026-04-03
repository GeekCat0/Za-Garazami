using UnityEngine;
using System.Collections.Generic;

public enum UnitState
{
    Moving = 0,
    Attacking = 1,
    Dead = 2,
}
public enum UnitType
{
    Tank = 0,
    Rogue = 1,
    Ranged = 2,
}
public class UnitController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [field: SerializeField] public UnitState unitState { get; private set; } = UnitState.Moving;
    [SerializeField] private string enemyTag;

    [Header("MovementStats")]
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private bool movingLeft = false;

    [Header("CombatStats")]
    [SerializeField] private UnitType unitType;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float damage = 1f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private float attackRange = 1f;

    [SerializeField] private float currentHealth = 100f; //serialize tylko jako debug 
    private float cooldownTimer = 1f;
    private UnitController target;


    private void Start() // ustawiamy zmienne które tego potrzebują
    {
        currentHealth = maxHealth;
        cooldownTimer = attackCooldown;
    }


    void FixedUpdate() // tbh mieszane opinie są o tym i zastanawiam się czy chcemy FixedUpdate czy Update ale napewno przy rigidbody movement nie trzeba używać Time.deltaTime w obu przypadkach 
    {
        StateUpdate();

        switch (unitState)
        {
            case UnitState.Moving:
                Move();
                break;

            case UnitState.Attacking:
                Attack();
                break;

            case UnitState.Dead:
                break;
        }
    }

    private void Move() // super simple: ruszamy rigidbody oraz "movingLeft" wybiera kierunek bo mamy tylko 2 opcje xd
    {
        rb.linearVelocityX = movingLeft ? -moveSpeed : moveSpeed;
    }
    private void Attack()
    {
        rb.linearVelocityX = 0f;
        cooldownTimer -= Time.fixedDeltaTime;

        if (cooldownTimer <= 0f)
        {
            float FinalDamage = damage * GetMultiplier(target.unitType);
            Debug.Log(this.tag + " attacked " + target.tag + " for " + FinalDamage + " damage.");
            target.TakeDamage(FinalDamage);
            cooldownTimer = attackCooldown; 
        }
    }
    private void TakeDamage(float damage)
    {
        currentHealth -= damage;
    }
    private void Death() // tutaj potem damy by zagrało najpierw jakąś animacje etc. ale narazie przeciwnik znika po sekundzie
    {
        rb.linearVelocityX = 0f;
        Destroy(gameObject, 1f);
    }
    private void StateUpdate() // ustawiamy stan jednostki
    {
        if (currentHealth <= 0) // blehhhh jednostka umarla
        {
            if (unitState != UnitState.Dead)
                Death();

            unitState = UnitState.Dead;
            return;
        }

        UnitController nearest = FindNearest();
        if (nearest != null) // jeśli mamy jakiegoś przeciwnika w zasięgu to atakujemy a jak nie to idzie dalej
        {
            target = nearest;
            unitState = UnitState.Attacking;
        }
        else
        {
            unitState = UnitState.Moving;
        }
    }
    private UnitController FindNearest() // zwraca przeciwnika najbliżej do jednostki w zasięgu ataku
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange); 

        UnitController nearest = null;
        float nearestDistance = Mathf.Infinity;

        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag(enemyTag)) continue;

            UnitController unit = hit.GetComponent<UnitController>();

            if (unit == null || unit.unitState == UnitState.Dead) continue;

            float distance = Vector2.Distance(transform.position, hit.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = unit;
            }
        }
        return nearest;
    }

    private readonly Dictionary<UnitType, Dictionary<UnitType, float>> matchups // słownik z typami i jakie bonusy mają
    = new Dictionary<UnitType, Dictionary<UnitType, float>>()
    {
        {
            UnitType.Tank, new Dictionary<UnitType, float>
            {
                { UnitType.Tank, 1.0f },
                { UnitType.Rogue,  1.5f },  
                { UnitType.Ranged,   0.5f },  
            }
        },
        {
            UnitType.Rogue, new Dictionary<UnitType, float>
            {
                { UnitType.Tank, 0.5f },
                { UnitType.Rogue,  1.0f },
                { UnitType.Ranged,   1.5f },
            }
        },
        {
            UnitType.Ranged, new Dictionary<UnitType, float>
            {
                { UnitType.Tank, 1.5f },
                { UnitType.Rogue,  0.5f },
                { UnitType.Ranged,   1.0f },
            }
        },
    };

    public float GetMultiplier( UnitType defender)
    {
        if (matchups.TryGetValue(unitType, out var defenderTable))
            if (defenderTable.TryGetValue(defender, out float multiplier))
                return multiplier;

        return 1.0f;
    }

    void OnDrawGizmosSelected() // by ładnie było widać zasięg ataku w edytorze 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
