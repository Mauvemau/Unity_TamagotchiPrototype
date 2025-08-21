using UnityEngine;

public class RewardsManager : MonoBehaviour {
    [SerializeField] private int eggs = 0;

    private void HandleHappyRewardCyclePassed() {
        eggs++;
    }

    private void OnEnable() {
        Pet.onHappyRewardCyclePassed += HandleHappyRewardCyclePassed;
    }

    private void OnDisable() {
        Pet.onHappyRewardCyclePassed -= HandleHappyRewardCyclePassed;
    }
}
