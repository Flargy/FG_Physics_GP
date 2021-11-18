using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    private static UI_Manager instance;
    public static UI_Manager Instance => instance;

    [SerializeField] private Text crystalText;
    [SerializeField] private Text rotationTimeText;
    [SerializeField] private Text DeathText;
    [SerializeField] private Text HiddenItemText;
    [SerializeField] private Image rotationBar;
    [SerializeField] private Image deathScreen;
    [SerializeField] private float deathScreenDuration = 0.7f;
    [SerializeField, Range(0, 255)] private int deathScreenAlpha = 125;

    private int crystalsGathered = 0;
    private int deaths = 0;
    private int hiddenItemsFound = 0;
    private float halfDeathDuration;
    private float deathTimer;
    private float alphaPercentage;
    private Coroutine deathRoutine;

    public int GetCrystals()
    {
        return crystalsGathered;
    }

    public int GetDeath()
    {
        return deaths;
    }

    public int GetHiddenItems()
    {
        return hiddenItemsFound;
    }

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        DontDestroyOnLoad(this);
        halfDeathDuration = deathScreenDuration * 0.5f;
        alphaPercentage = deathScreenAlpha / (float)255;
    }

    public void SetCrystalText(int value)
    {
        crystalText.text = value.ToString();
    }

    public void AddCrystal()
    {
        crystalsGathered++;
        crystalText.text = crystalsGathered.ToString();
    }

    public void SetTimerText(int value)
    {
        rotationTimeText.text = value.ToString();
    }

    public void SetTimerBar(float value)
    {
        rotationBar.fillAmount = value;
    }

    public void RespawnDeathScreen()
    {
        if(deathRoutine != null)
            StopCoroutine(deathRoutine);
        deaths++;
        DeathText.text = deaths.ToString();
        deathRoutine = StartCoroutine(DeathTint());
    }

    private IEnumerator DeathTint()
    {
        Color col = deathScreen.color;
        float alpha = 0;
        while (deathTimer <= 1)
        {
            yield return null;
            deathTimer += Time.deltaTime / halfDeathDuration;
            alpha = deathTimer * alphaPercentage;
            deathScreen.color = new Color(col.r, col.g, col.b, alpha);
        }

        float currentAlpha = alpha;
        deathTimer = 0.0f;

        while (deathTimer <= 1)
        {
            yield return null;
            deathTimer += Time.deltaTime / halfDeathDuration;
            alpha = deathTimer * alphaPercentage;
            deathScreen.color = new Color(col.r, col.g, col.b, currentAlpha- alpha);
        }
        deathScreen.color = new Color(col.r, col.g, col.b, 0);
        deathTimer = 0.0f;

    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void AddHiddenItem()
    {
        hiddenItemsFound++;
        HiddenItemText.text = hiddenItemsFound.ToString();
    }

}
