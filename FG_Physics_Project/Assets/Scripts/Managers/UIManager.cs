using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    private static UIManager instance = null;
    public static UIManager Instance => instance;

    [SerializeField] private Text goldDisplay;

    private void Start()
    {
        if (instance != null)
        {
            Destroy(this);
        }

        instance = this;
    }

    public void AddGold()
    {
        
    }
}
