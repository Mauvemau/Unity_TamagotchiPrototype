using System;
using Unity.VisualScripting;
using UnityEngine;

public class Pet : MonoBehaviour {
    [Header("References")] [SerializeField]
    private SessionDataManager sessionManagerRef;
    [Header("Hud References")] 
    [SerializeField] private ProgressBar hungerProgressBarRef;
    [SerializeField] private ProgressBar hygieneProgressBarRef;
    [SerializeField] private ProgressBar energyProgressBarRef;
    [SerializeField] private ProgressBar debugProgressBarRef;
    [Header("General Settings")]
    [SerializeField, Min(0.001f)] private float timeScale = 1f; // 1f = 12 hours / 0.01f = 12 minutes / 2f = 24 hours
    [Header("Need Settings")]                            // (For the following commented calculus it's assumed general decay rate is 1f)
    [SerializeField] private float hungerDecayRate = 3f; // 3f = 36 hours until starvation
    [SerializeField] private float hygieneDecayRate = 1f; // 1f = 12 hours until dirty
    [SerializeField] private float energyDecayRate = 2f; // 2 = 24 hours until exhausted
    [SerializeField] private float debugDecayRate = 0.1f;
    [Header("Happiness Threshold Settings")]
    [SerializeField] private float happinessTreshold = 0.2f; // All stats must be over 20% full for the pet to be happy 
    [Header("Reward Settings")]
    [SerializeField] private float rewardRate = 0.08f; // 0.05f = 1 reward every hour aprox?

    private float _hunger = 100f;
    private float _hygiene = 100f;
    private float _energy = 100f;
    private float _debugNeed = 100f;

    private float _lastRewardTimeStamp = 0f;

    private bool _petLoaded = false;

    public static event Action onHappyRewardCyclePassed = delegate {};

    private void HandleInput() {
        if (Input.GetKeyDown(KeyCode.Q)) {
            _hunger += 5f;
            if(_hunger > 100f)
                _hunger = 100f;
        }
        if (Input.GetKeyDown(KeyCode.W)) {
            _hygiene += 5f;
            if(_hygiene > 100f)
                _hygiene = 100f;
        }
        if (Input.GetKeyDown(KeyCode.E)) {
            _energy += 5f;
            if(_energy > 100f)
                _energy = 100f;
        }
        if (Input.GetKeyDown(KeyCode.R)) {
            _debugNeed += 5f;
            if(_debugNeed > 100f)
                _debugNeed = 100f;
        }
    }
    
    private bool WasPetHappyAtTimestamp(DateTime timeStamp) {
        // Calculate the stats of the pet at timeStamp and return if the pet's stats were over the treshold at that point in time.
        return false;
    }

    private void HandleRewards(float baseDecayPerSecond) {
        // Calculate based on rewardRate how many rewards fit between the last reward timestamp and current timestamp.
        // For every reward get the timestamp of that reward call WasPetHappyAtTimestamp().
        // If true invoke onHappyRewardCyclePassed;
        // Update last reward timestamp
    }

    private void CalculateStatDecayBasedOnTimePassed(float timePassedInSeconds) {
        // Base: at 1x global and 1x stat, 100 -> 0 in 12 hours (43200s)
        const float baseDecayPerSecond = 100f / (12f * 3600f);

        HandleRewards(baseDecayPerSecond);

        float hungerDecay = baseDecayPerSecond * timePassedInSeconds / (timeScale * hungerDecayRate);
        float hygieneDecay = baseDecayPerSecond * timePassedInSeconds / (timeScale * hygieneDecayRate);
        float energyDecay  = baseDecayPerSecond * timePassedInSeconds / (timeScale * energyDecayRate);
        float debugDecay = baseDecayPerSecond * timePassedInSeconds / (timeScale * debugDecayRate);

        _hunger = Mathf.Max(0f, _hunger - hungerDecay);
        _hygiene = Mathf.Max(0f, _hygiene - hygieneDecay);
        _energy = Mathf.Max(0f, _energy - energyDecay);
        _debugNeed = Mathf.Max(0f, _debugNeed - debugDecay);
        
        if (!hungerProgressBarRef || !hygieneProgressBarRef || !energyProgressBarRef) return;
        
        hungerProgressBarRef.SetCurrentValue(_hunger);
        hygieneProgressBarRef.SetCurrentValue(_hygiene);
        energyProgressBarRef.SetCurrentValue(_energy);

        if (!debugProgressBarRef) return;
        
        debugProgressBarRef.SetCurrentValue(_debugNeed);
    }

    private void OnApplicationPause(bool pauseStatus) {
        if (!_petLoaded) return;
        if (!sessionManagerRef) {
            Debug.LogWarning("No session manager reference attached!");
        }
        if (pauseStatus) {
            sessionManagerRef.SaveSession();
        }
        else {
            TimeSpan pausedTime = sessionManagerRef.GetTimeSinceLastPlayed();
            
            CalculateStatDecayBasedOnTimePassed(pausedTime.Seconds);
        }
    }

    private void OnApplicationQuit() {
        if (!sessionManagerRef) {
            Debug.LogWarning("No session manager reference attached!");
        }
        sessionManagerRef.SaveSession(_hunger, _hygiene, _energy, _debugNeed);
    }
    
    private void LoadPet() {
        if (!sessionManagerRef) {
            Debug.LogWarning("No session manager reference attached!");
        }
        Debug.Log(sessionManagerRef.LoadPetStats(out _hunger, out _hygiene, out _energy, out _debugNeed)
            ? "Loaded saved pet stats!"
            : "No saved pet stats found.");
        
        TimeSpan offlineTime = sessionManagerRef.GetTimeSinceLastPlayed();
        CalculateStatDecayBasedOnTimePassed((float)offlineTime.TotalSeconds);

        _petLoaded = true;
        Debug.Log("Pet loaded!");
    }
    
    private void Start() {
        LoadPet();
        if (!hungerProgressBarRef || !hygieneProgressBarRef || !energyProgressBarRef) return;
        hungerProgressBarRef.SetMaxValue(100f);
        hygieneProgressBarRef.SetMaxValue(100f);
        energyProgressBarRef.SetMaxValue(100f);

        if (!debugProgressBarRef) return;
        debugProgressBarRef.SetMaxValue(100f);
    }

    private void Update() {
        if (!_petLoaded) return;
        HandleInput();
        CalculateStatDecayBasedOnTimePassed(Time.deltaTime);
    }
}
