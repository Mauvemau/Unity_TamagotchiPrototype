using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour {
    [Header("References")]
    [SerializeField] private Image fillImage;
    [Header("Visual Settings")] 
    [SerializeField] private Color fullColor = Color.green; // 100 - 60
    [SerializeField] private Color halfFullColor = Color.yellow; // 60 - 30
    [SerializeField] private Color lowColor = Color.red; // 30 - 0

    private float _maxValue = 100;

    public void SetMaxValue(float amount) {
        _maxValue = amount;
        SetCurrentValue(amount);
    }

    public void SetCurrentValue(float amount) {
        if (!fillImage) return;
        
        float normalizedValue = Mathf.Clamp01(amount / _maxValue);
        fillImage.fillAmount = normalizedValue;
        
        if (normalizedValue > 0.6f) {
            float t = Mathf.InverseLerp(0.6f, 1f, normalizedValue);
            fillImage.color = Color.Lerp(halfFullColor, fullColor, t);
        }
        else if (normalizedValue > 0.3f) {
            float t = Mathf.InverseLerp(0.3f, 0.6f, normalizedValue);
            fillImage.color = Color.Lerp(lowColor, halfFullColor, t);
        }
        else {
            fillImage.color = lowColor;
        }
    }   
}
