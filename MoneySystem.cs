using UnityEngine;
using System.Collections;

public class MoneySystem : MonoBehaviour
{
    public static MoneySystem instance;

    public int playerMoney;
    [SerializeField] float incomeDelay;
    [SerializeField] int incomeAmount;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
        StartCoroutine(MoneyIncome());
    }
    IEnumerator MoneyIncome()
    {
        while (true)
        {
            yield return new WaitForSeconds(incomeDelay);
            playerMoney += incomeAmount;
        }

    }
}
