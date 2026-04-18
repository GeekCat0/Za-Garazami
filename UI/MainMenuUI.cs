using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public static int lvlIndex;

    [SerializeField] GameObject startScreen;
    [SerializeField] GameObject lvlScreen;

    private void Start()
    {
        StartScreenActive();
    }

    public void SetLvl(int lvl)
    {
        lvlIndex = lvl;
    }

    public void StartScreenActive()
    {
        startScreen.SetActive(true);
        lvlScreen.SetActive(false);
    }

    public void LvlScreenActive()
    {
        startScreen.SetActive(false);
        lvlScreen.SetActive(true);
    }
}
