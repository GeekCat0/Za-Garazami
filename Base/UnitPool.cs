using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class UnitPoolEntry
{
    public UnitType unitType;       
    public GameObject unitPrefab;   
    public int poolSize = 20;       
}

public class UnitPool : MonoBehaviour
{
    [SerializeField] private List<UnitPoolEntry> unitEntries;

    // tworzymy kolejkę dla każdego rodzaju jednostki
    private Dictionary<UnitType, Queue<GameObject>> pools = new Dictionary<UnitType, Queue<GameObject>>();

    private void Awake()
    {
        foreach (UnitPoolEntry entry in unitEntries)
        {
            Queue<GameObject> pool = new Queue<GameObject>();

            // pre-spawn wszystkich jednostek
            for (int i = 0; i < entry.poolSize; i++)
            {
                GameObject unit = Instantiate(entry.unitPrefab, transform);
                unit.SetActive(false);
                pool.Enqueue(unit);
            }

            pools[entry.unitType] = pool;
        }
    }

    // zabiera jednostkę z poola
    public GameObject Get(UnitType unitType, Vector3 position)
    {
        pools.TryGetValue(unitType, out Queue<GameObject> pool);

        GameObject unit = pool.Count > 0 ? pool.Dequeue() : Instantiate(unitEntries.Find(e => e.unitType == unitType).unitPrefab, transform);
        if (unit.CompareTag("Friendly"))
        {
            UnitController unitScript = unit.GetComponent<UnitController>();
            if (unitScript == null)
            {
                Debug.Log("Prefab nie ma na sobie unitControlera");
                unit.SetActive(false);
                pool.Enqueue(unit);
                return null;
            }
            if (unitScript.unitCost <= MoneySystem.instance.playerMoney)
            {
                MoneySystem.instance.playerMoney -= unitScript.unitCost;
                MoneySystem.instance.updateUI();
            }
            else
            {
                unit.SetActive(false);
                pool.Enqueue(unit);
                return null;
            }
        }
        Debug.Log("Idzie skrypt dalej");
        unit.transform.position = position;
        unit.SetActive(true);
        unit.GetComponent<UnitController>().ResetUnit();
        return unit;
    }

    // zwracanie jednostek do poola
    public void Return(UnitType unitType, GameObject unit)
    {
        unit.SetActive(false);

        if (pools.TryGetValue(unitType, out Queue<GameObject> pool))
            pool.Enqueue(unit);
        else
            Destroy(unit);
    }
}