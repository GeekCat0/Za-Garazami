using UnityEngine;
using System.Collections;
using TMPro;

public class MoneySystem : MonoBehaviour
{
    public static MoneySystem instance;

    public int playerMoney;
    [SerializeField] float incomeDelay;
    [SerializeField] int incomeAmount;
    [SerializeField] TextMeshProUGUI currencyText;
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
        updateUI();
        StartCoroutine(MoneyIncome());
    }
    IEnumerator MoneyIncome()
    {
        while (true)
        {
            yield return new WaitForSeconds(incomeDelay);
            playerMoney += incomeAmount;
            updateUI();
        }

    }
    public void updateUI()
    {
        currencyText.text = playerMoney.ToString();
    }
}
