using System;
using UnityEngine;

public class SessionDataManager : MonoBehaviour {
    private const string LastPlayedPrefKey = "LastPlayed";
    private const string HungerKey = "Pet_Hunger";
    private const string HygieneKey = "Pet_Hygiene";
    private const string EnergyKey = "Pet_Energy";
    private const string DebugKey = "Pet_Debug";

    public TimeSpan GetTimeSinceLastPlayed() {
        Debug.Log("Retrieving last played time...");
        if (!PlayerPrefs.HasKey(LastPlayedPrefKey)) {
            Debug.Log("No file found! New session started.");
            return TimeSpan.Zero;
        }
        
        long binary = Convert.ToInt64(PlayerPrefs.GetString(LastPlayedPrefKey));
        DateTime lastPlayed = DateTime.FromBinary(binary);
        Debug.Log($"File found! {(DateTime.UtcNow - lastPlayed).TotalHours} hours since last played!");
        return DateTime.UtcNow - lastPlayed;
    }
    
    public bool LoadPetStats(out float hunger, out float hygiene, out float energy, out float debug) {
        if (PlayerPrefs.HasKey(HungerKey)) {
            hunger = PlayerPrefs.GetFloat(HungerKey, 100f);
            hygiene = PlayerPrefs.GetFloat(HygieneKey, 100f);
            energy = PlayerPrefs.GetFloat(EnergyKey, 100f);
            debug = PlayerPrefs.GetFloat(DebugKey, 100f);
            return true;
        }

        hunger = hygiene = energy = debug = 100f;
        return false;
    }
    
    public void SaveSession() { // Only saves time (good for pause)
        DateTime now = DateTime.UtcNow;
        PlayerPrefs.SetString(LastPlayedPrefKey, now.ToBinary().ToString());
        PlayerPrefs.Save();
    }
    
    public void SaveSession(float hunger, float hygiene, float energy, float debug) { // Saves time and stats
        DateTime now = DateTime.UtcNow;
        PlayerPrefs.SetString(LastPlayedPrefKey, now.ToBinary().ToString());

        PlayerPrefs.SetFloat(HungerKey, hunger);
        PlayerPrefs.SetFloat(HygieneKey, hygiene);
        PlayerPrefs.SetFloat(EnergyKey, energy);
        PlayerPrefs.SetFloat(DebugKey, debug);

        PlayerPrefs.Save();
        Debug.Log("Session saved!");
    }
}
