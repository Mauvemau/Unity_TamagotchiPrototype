using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class RewardPool {
    [SerializeField] public float pullRate = 1.0f; // 1.0f = 100%;
    [SerializeField] private List<SO_Reward> rewards;

    public SO_Reward GetRandomReward() {
        return rewards[UnityEngine.Random.Range(0, rewards.Count)];
    }
}

public class RewardsManager : MonoBehaviour {
    [Header("Reward Inventory")]
    [SerializeField] List<SO_Reward> rewardsCollected;

    [Header("Reward Pool Settings")]
    [SerializeField] List<RewardPool> rewardPools;

    [Header("Rewards Counter")]
    [SerializeField] private int eggs = 0;

    private void HandleHappyRewardCyclePassed() {
        eggs++;
    }

    private void OpenEgg() {
        if (eggs <= 0) return;
        float totalWeight = 0f;

        foreach (RewardPool rewardPool in rewardPools) {
            totalWeight += rewardPool.pullRate;
        }

        float randomValue = UnityEngine.Random.Range(0, totalWeight);
        float currentSum = 0f;

        foreach (RewardPool rewardPool in rewardPools) {
            currentSum += rewardPool.pullRate;
            if (randomValue <= currentSum) {
                SO_Reward reward = rewardPool.GetRandomReward();
                eggs--;
                rewardsCollected.Add(reward);
                Debug.Log($"You got: {reward.name}");
                return;
            }
        }

        Debug.LogWarning("No reward pool selected. Check pull rates.");
    }


    private void HandleControls() {
        if (Input.GetKeyDown(KeyCode.Z)) {
            OpenEgg();
        }
    }

    private void Update() {
        HandleControls();   
    }

    private void Awake() {
        UnityEngine.Random.InitState((int)Time.time);
    }

    private void OnEnable() {
        Pet.onHappyRewardCyclePassed += HandleHappyRewardCyclePassed;
    }

    private void OnDisable() {
        Pet.onHappyRewardCyclePassed -= HandleHappyRewardCyclePassed;
    }
}
