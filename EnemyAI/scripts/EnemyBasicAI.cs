using UnityEngine;
using System.Collections;

public class EnemyBasicAI : MonoBehaviour
{
    [SerializeField] Base enemyBase;

    [SerializeField] float spawnDelay;
    [SerializeField, Range(0,2)] int defaultUnitID;
    [SerializeField, Range(0,2)] int maxUnitID;
    int nextUnitID;

    int unitsSpawnedByPlayerId0 = 0;
    int unitsSpawnedByPlayerId1 = 0;
    int unitsSpawnedByPlayerId2 = 0;

    private void Start()
    {
        if (defaultUnitID > maxUnitID)
        {
            Debug.LogError("Domyœlna jednostka wroga jest niemo¿liwa do zespawnowania!!!!");
        }

        nextUnitID = defaultUnitID;

        StartCoroutine(SpawningProcess());
    }

    private void OnEnable()
    {
        Base.UnitSpawnedEvent += NewPlayerUnit;
    }

    private void OnDisable()
    {
        Base.UnitSpawnedEvent += NewPlayerUnit;
    }
    //Zapisujemy styl gry gracza, AI mo¿e delikatnie reagowaæ dziêki temu...
    void NewPlayerUnit(int unitID)
    {
        if (unitID == 0) unitsSpawnedByPlayerId0++;
        if (unitID == 1) unitsSpawnedByPlayerId1++;
        if (unitID == 2) unitsSpawnedByPlayerId2++;
    }
    //Funkcja sprawdza jakie warunki s¹ spe³nione
    void CheckNextUnit()
    {
        if (unitsSpawnedByPlayerId0 >= unitsSpawnedByPlayerId1 + unitsSpawnedByPlayerId2)
        {
            nextUnitID = 2;
        }
        else if (unitsSpawnedByPlayerId1 >= unitsSpawnedByPlayerId0 + unitsSpawnedByPlayerId2)
        {
            nextUnitID = 0;
        }
        else if (unitsSpawnedByPlayerId2 >= unitsSpawnedByPlayerId1 + unitsSpawnedByPlayerId0)
        {
            nextUnitID = 1;
        }
        else nextUnitID = defaultUnitID;

    }

    IEnumerator SpawningProcess()
    {
        while (enemyBase != null)
        {
            enemyBase.SpawnUnit(nextUnitID);
            yield return new WaitForSeconds(spawnDelay);
            CheckNextUnit();
        }
    }


}
