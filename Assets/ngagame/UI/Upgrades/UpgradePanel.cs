using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePanel : MonoBehaviour
{
    public static string UPGRADE_LEVEL_KEY = "UPGRADE_LEVEL_KEY_";

    private void Awake()
    {

    }

    public static int GetUpdradeLevel(string id)
    {

        return PlayerPrefs.GetInt(UPGRADE_LEVEL_KEY + id, 0);
    }
    public void OnMorePowerClicked(UpgradeButton upgradeButton)
    {

    }

    public void OnMoreSpeedClicked(UpgradeButton upgradeButton)
    {

    }

    public void OnMoreStarClicked(UpgradeButton upgradeButton)
    {

    }
}
