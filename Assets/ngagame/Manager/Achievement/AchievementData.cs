using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AchievementData", menuName = "AchievementData")]
public class AchievementData : ScriptableObject
{
    public List<AchievementEntity> achievements;
}
