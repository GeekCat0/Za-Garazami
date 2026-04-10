using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

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
    Base = 3,
}
public class UnitController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Slider hpSlider;
    [field: SerializeField] public UnitState unitState { get; private set; } = UnitState.Moving;
    [SerializeField] private string enemyTag;
    [SerializeField] private string enemyBaseTag;

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
    public int unitCost = 5;
    private float cooldownTimer = 1f;
    private UnitController target;
    private bool attackingBase = false;
    private GameObject enemyBase;
    private UnitPool pool;

    private void Start() // ustawiamy zmienne które tego potrzebują
    {
        currentHealth = maxHealth;
        cooldownTimer = attackCooldown;
        enemyBase = GameObject.FindWithTag(enemyBaseTag);
    }


    void FixedUpdate() // tbh mieszane opinie są o tym i zastanawiam się czy chcemy FixedUpdate czy Update ale napewno przy rigidbody movement nie trzeba używać Time.deltaTime w obu przypadkach 
    {
        StateUpdate();

        switch (unitState)
        {
            case UnitState.Moving:
                animator.SetBool("Attack", false);
                Move();
                break;

            case UnitState.Attacking:
                Attack();
                break;

            case UnitState.Dead:
                animator.SetBool("Attack", false);
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

        animator.SetBool("Attack", false);
        if (cooldownTimer <= 0f)
        {
            animator.SetBool("Attack", true);
            if (!attackingBase)
            {
                float finalDamage = damage * Matchups.GetMultiplier(unitType, target.unitType);
                Debug.Log(this.tag + " attacked " + target.tag + " for " + finalDamage + " damage.");
                target.TakeDamage(finalDamage);
                cooldownTimer = attackCooldown;
            }else
            {
                float finalDamage = damage * Matchups.GetMultiplier(unitType, UnitType.Base);
                Debug.Log(this.tag + " attacked " + enemyBaseTag + " for " + finalDamage + " damage.");
                enemyBase.GetComponent<Base>().TakeDamage(finalDamage);
                cooldownTimer = attackCooldown;
            }
        }
    }

    private void TakeDamage(float damage)
    {
        sprite.color = Color.red;
        Invoke(nameof(ResetColor), 0.2f);
        currentHealth -= damage;
        hpSlider.value = currentHealth / maxHealth * 100;
    }
    private void ResetColor()
    {
        sprite.color = Color.white;
    }
    public void Initialize(UnitPool unitPool)
    {
        pool = unitPool;
    }
    public void ResetUnit()
    {
        currentHealth = maxHealth;
        cooldownTimer = attackCooldown;
        unitState = UnitState.Moving;
        target = null;
        attackingBase = false;
        enemyBase = GameObject.FindWithTag(enemyBaseTag);
        hpSlider.value = currentHealth / maxHealth * 100;
    }

    private void Death() // tutaj potem damy by zagrało najpierw jakąś animacje etc. ale narazie przeciwnik znika 
    {
        rb.linearVelocityX = 0f;
        unitState = UnitState.Dead;

        if (pool != null)
            pool.Return(unitType, gameObject); // wraca do poola
        else
            Destroy(gameObject); // jakby cos sie zjebalo z poolem to leci destroy
    }

    private void StateUpdate() // ustawiamy stan jednostki
    {
        if (currentHealth <= 0) // blehhhh jednostka umarla
        {
            if (unitState != UnitState.Dead)
                Death();

            return;
        }

        UnitController nearest = FindNearest();
        if (nearest != null) // jeśli mamy jakiegoś przeciwnika w zasięgu to atakujemy
        {
            attackingBase = false;
            target = nearest;
            unitState = UnitState.Attacking;
            return;
        }
        
        if (enemyBase != null) // jeśli nie ma przeciwników to dopiero wtedy atakujemy bazę jeśli ta jest w zasięgu
        {
            float distanceToBase = Vector2.Distance(transform.position, enemyBase.transform.position);
            if (distanceToBase <= attackRange)
            {
                attackingBase = true;
                unitState = UnitState.Attacking;
                return;
            }
        }

        unitState = UnitState.Moving;
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

    void OnDrawGizmosSelected() // by ładnie było widać zasięg ataku w edytorze 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
