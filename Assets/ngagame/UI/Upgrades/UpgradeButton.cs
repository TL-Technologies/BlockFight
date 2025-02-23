using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButton : MonoBehaviour
{
    [SerializeField] string id;
    [SerializeField] Text priceText;
    [SerializeField] Text levelText;
    [SerializeField] int[] prices = new int[] { 500, 600, 800, 1000, 1500, 2000, 3000, 4000, 5000, 8000, 10000 };
    [SerializeField] Animation UpgradeAnimation;
    [SerializeField] GameObject priceGroup;
    [SerializeField] GameObject rvGroup;

    Button button;
    [HideInInspector] public int price, level;

    private void Awake()
    {
        button = GetComponent<Button>();
        if(button != null)
            button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        if(rvGroup.gameObject.activeSelf)
        {
            LevelUp();
        } else
		{
            if(Profile.Instance.Coins >= price)
			{
                LevelUp();
                Profile.Instance.Coins -= price;
            }
        }
    }

    void LevelUp()
	{
        PlayerPrefs.SetInt(UpgradePanel.UPGRADE_LEVEL_KEY + id, level + 1);
        UpgradeAnimation.Play(PlayMode.StopAll);
    }
 
    void Refresh(GameEvent Event_Type, Component Sender, object Param = null)
    {
        level = PlayerPrefs.GetInt(UpgradePanel.UPGRADE_LEVEL_KEY + id, 0);
        price = level < prices.Length ? prices[level] : prices[prices.Length - 1];
        priceText.text = price.ToString();
        levelText.text = $"LVL <b>{level + 1}</b>";

        var rvRequire = Profile.Instance.Coins < price || (level > 0 && level % 3 == 0);
        priceGroup.gameObject.SetActive(!rvRequire);
        rvGroup.gameObject.SetActive(rvRequire);
    }

    private void OnEnable()
    {
        EventManager.Instance.AddListener(GameEvent.OnCoinChange, Refresh);
        EventManager.Instance.AddListener(GameEvent.OnPreviewCoin, Refresh);
        Refresh(GameEvent.OnCoinChange, null);
    }

    private void OnDisable()
    {
        var eventManager = EventManager.Instance;
        if (eventManager == null)
        {
            return;
        }
        eventManager.RemoveEvent(GameEvent.OnCoinChange, Refresh);
        eventManager.RemoveEvent(GameEvent.OnPreviewCoin, Refresh);
    }
}
