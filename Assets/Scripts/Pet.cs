using System;
using Unity.VisualScripting;
using UnityEngine;

public class Pet : MonoBehaviour {
    [Header("References")] [SerializeField]
    private SessionDataManager sessionManagerRef;
    [Header("General Settings")]
    [SerializeField, Min(0.001f)] private float timeScale = 1f; // 1f = 12 hours / 0.01f = 12 minutes / 2f = 24 hours
    [Header("Need Settings")]                            // (For the following commented calculus it's assumed general decay rate is 1f)
    [SerializeField] private float hungerDecayRate = 3f; // 3f = 36 hours until starvation
    [SerializeField] private float hygieneDecayRate = 1f; // 1f = 12 hours until dirty
    [SerializeField] private float energyDecayRate = 2f; // 2 = 24 hours until exhausted
    [Header("Stats (Make this private later)")]
    [SerializeField] private float _hunger = 100f;
    [SerializeField] private float _hygiene = 100f;
    [SerializeField] private float _energy = 100f;

    private bool _petLoaded = false;

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
    }
    
    private void CalculateStatDecayBasedOnTimePassed(float timePassedInSeconds) {
        // Base: at 1x global and 1x stat, 100 -> 0 in 12 hours (43200s)
        const float baseDecayPerSecond = 100f / (12f * 3600f);
        
        float hungerDecay = baseDecayPerSecond * timePassedInSeconds / (timeScale * hungerDecayRate);
        float hygieneDecay = baseDecayPerSecond * timePassedInSeconds / (timeScale * hygieneDecayRate);
        float energyDecay  = baseDecayPerSecond * timePassedInSeconds / (timeScale * energyDecayRate);

        _hunger = Mathf.Max(0f, _hunger - hungerDecay);
        _hygiene = Mathf.Max(0f, _hygiene - hygieneDecay);
        _energy = Mathf.Max(0f, _energy - energyDecay);
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
        sessionManagerRef.SaveSession(_hunger, _hygiene, _energy);
    }
    
    private void LoadPet() {
        if (!sessionManagerRef) {
            Debug.LogWarning("No session manager reference attached!");
        }
        Debug.Log(sessionManagerRef.LoadPetStats(out _hunger, out _hygiene, out _energy)
            ? "Loaded saved pet stats!"
            : "No saved pet stats found.");
        
        TimeSpan offlineTime = sessionManagerRef.GetTimeSinceLastPlayed();
        CalculateStatDecayBasedOnTimePassed((float)offlineTime.TotalSeconds);

        _petLoaded = true;
        Debug.Log("Pet loaded!");
    }
    
    private void Start() {
        LoadPet();
    }

    private void Update() {
        if (!_petLoaded) return;
        HandleInput();
        CalculateStatDecayBasedOnTimePassed(Time.deltaTime);
    }
}
