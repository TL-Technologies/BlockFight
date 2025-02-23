using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class Leaderboard : MonoBehaviour
{
    [SerializeField] Transform content;
    [SerializeField] Sprite[] flags;
    public RankItemView playerRankItem;
    [SerializeField] int fromPosition = 23;
    [SerializeField] int toPosition = 2;
    [SerializeField] float delayPlayerRankUp = 0.5f;
    [SerializeField] float timePlayerRankUp = 1f;
    [SerializeField] UnityEvent<int, int> onInfoChanged;
    [SerializeField] UnityEvent<int, int> onShowInfo;
    RankItemView[] rankItems;
    int startRank, changeRankAmount;
    Animator animator;

    static int rank = 10000;
    static int score, newScore = 0;
    public static int MinRankAmountEachLevel = 50;
    public static int MaxRankAmountEachLevel = 100;
    public static int MinScoreAmountEachLevel = 50;
    public static int Rank
    {
        get
        {
            LoadData();
            return rank;
        }

        set
        {
            rank = value;
            SaveData();
        }
    }

    public static int Score
    {
        get
        {
            LoadData();
            return score;
        }

        set
        {
            score = value;
            SaveData();
        }
    }

    static void LoadData()
    {
        rank = PlayerPrefs.GetInt("PLAYER_RANK_KEY", Random.Range(10000, 11000));
        rank = Mathf.Max(rank, Random.Range(1000, 1500)); // Do not be top 1
        score = PlayerPrefs.GetInt("PLAYER_SCORE_KEY", 0);
    }

    static void SaveData()
    {
        PlayerPrefs.SetInt("PLAYER_RANK_KEY", rank);
        PlayerPrefs.SetInt("PLAYER_SCORE_KEY", score);
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        changeRankAmount = Random.Range(MinRankAmountEachLevel, MaxRankAmountEachLevel);
        startRank = Rank - fromPosition;
        newScore = Score + 10;
        
        playerRankItem.rankText.text = $"#{startRank + fromPosition}";
        playerRankItem.scoreText.text = $"{Score}";
        rankItems = content.GetComponentsInChildren<RankItemView>(true);

        for (int i = 0; i < rankItems.Length; i++)
        {
            rankItems[i].rankText.text = $"#{startRank + i}";
            rankItems[i].nameText.text = $"{AINamesGenerator.Utils.GetRandomName()}";
            var dis = newScore / (rankItems.Length - fromPosition - 1);
            dis = Mathf.Max(1, dis);
            rankItems[i].scoreText.text = $"{(rankItems.Length - i) * dis + (i < fromPosition ? Random.Range(0, dis) : -Random.Range(0, dis))}";
            rankItems[i].flagImage.sprite = flags[Random.Range(0, flags.Length)];
        }

        onShowInfo?.Invoke(startRank - changeRankAmount + toPosition, newScore);
    }

    private void OnEnable()
    {
    }

    public void AllRankUp()
    {
        var dis = newScore / (rankItems.Length - toPosition);
        dis = Mathf.Max(1, dis);
        for (int i = 0; i < rankItems.Length; i++)
        {
            rankItems[i].rankText.text = $"#{startRank - changeRankAmount + i}";
            rankItems[i].scoreText.text = $"{(rankItems.Length - i) * dis + (i < toPosition ? Random.Range(0, dis) : - Random.Range(0, dis))}";
        }
    }

    Tween rankTween = null;
    public void GoUp(bool secondRun = false)
    {
        var fromScore = newScore;
        if(secondRun)
        {
            newScore += 10;
            var dis = newScore / (rankItems.Length - fromPosition);
            dis = Mathf.Max(1, dis);
            for (int i = 0; i < rankItems.Length; i++)
            {
                rankItems[i].rankText.text = $"#{Rank - fromPosition + i}";
                rankItems[i].scoreText.text = $"{(rankItems.Length - i) * dis + (i < fromPosition ? Random.Range(0, dis) : -Random.Range(0, dis))}";
            }
        }
        animator.SetTrigger("play");

        var fromRank = startRank + fromPosition;
        var newRank = startRank - changeRankAmount + toPosition;
        var lastRank = fromRank;
        var currentRank = fromRank;
        var hapticCappingTime = 0.1f; 
        var startRankUpTime = Time.realtimeSinceStartup;

        rankTween = DOVirtual.Float(fromRank, newRank, timePlayerRankUp, (value) =>
        {
            currentRank = Mathf.CeilToInt(value);
            if(currentRank != lastRank)
            {
                playerRankItem.rankText.text = "#" + currentRank;
                if(secondRun)
                {
                    playerRankItem.scoreText.text = 
                    $"{Mathf.CeilToInt(Mathf.Lerp(fromScore, newScore, (value - fromRank) / (newRank - fromRank)))}";
                }
                lastRank = currentRank;
                if(Time.realtimeSinceStartup - startRankUpTime > hapticCappingTime)
                {
                    startRankUpTime = Time.realtimeSinceStartup;
                }
            }
        }).SetDelay(delayPlayerRankUp).OnComplete(delegate
        {
            EventManager.Instance.PostNotification(GameEvent.OnTriggerAchievement, this, new AchievementEarned("get-ranks", changeRankAmount));
            changeRankAmount = Random.Range(MinRankAmountEachLevel, MaxRankAmountEachLevel);
            startRank -= changeRankAmount;
            Rank = newRank;
            Score = newScore;
            onInfoChanged?.Invoke(newRank, newScore);
        });
    }

	private void OnDisable()
	{
        if(rankTween != null)
		{
            rankTween.Kill();

        }
	}

	public void OnStarIncrease(float value)
    {
        playerRankItem.scoreText.text = Mathf.RoundToInt(Mathf.Lerp(score, newScore, value)).ToString();
    }
}
