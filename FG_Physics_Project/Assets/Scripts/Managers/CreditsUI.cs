using UnityEngine;
using UnityEngine.UI;

public class CreditsUI : MonoBehaviour
{
    [SerializeField] private Text gemText;
    [SerializeField] private Text deathText;
    [SerializeField] private Text deathTauntText;
    [SerializeField] private Text hiddenItemText;
    [SerializeField] private int tauntAmount = 25;

    private int crystals;
    private int deaths;
    private int hiddenItems;
    void Start()
    {
        UI_Manager manager = UI_Manager.Instance;
        crystals = manager.GetCrystals();
        deaths = manager.GetDeath();
        hiddenItems = manager.GetHiddenItems();

        gemText.text = crystals.ToString();
        deathText.text = deaths.ToString();
        hiddenItemText.text = hiddenItems.ToString();
        if (deaths >= tauntAmount)
        {
            deathTauntText.gameObject.SetActive(true);
        }
    }

    

    public void Exit()
    {
       Application.Quit();
    }
}
