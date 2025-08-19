using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class BackgroundColorChanger : MonoBehaviour {
    [Header("Color Cycle Settings")]
    [SerializeField] private List<Color> colors = new List<Color>() {
        Color.red,
        Color.green,
        Color.blue,
        Color.magenta
    };

    [SerializeField] private float transitionDuration = 2f;
    [SerializeField] private float holdDuration = 1f;

    [Header("Transition Curve")]
    [SerializeField] private AnimationCurve fadeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    private Camera _camera;
    private int _currentIndex = 0;

    private void Awake() {
        _camera = GetComponent<Camera>();
    }

    private void Start() {
        if (colors.Count > 1) {
            StartCoroutine(CycleColors());
        }
        else if (colors.Count == 1) {
            _camera.backgroundColor = colors[0];
        }
    }

    private IEnumerator CycleColors() {
        while (true) {
            Color startColor = colors[_currentIndex];
            Color targetColor = colors[(_currentIndex + 1) % colors.Count];
            
            float elapsed = 0f;
            while (elapsed < transitionDuration) {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / transitionDuration);
                float curveT = fadeCurve.Evaluate(t);
                _camera.backgroundColor = Color.Lerp(startColor, targetColor, curveT);
                yield return null;
            }
            
            _currentIndex = (_currentIndex + 1) % colors.Count;
            
            if (holdDuration > 0f) {
                yield return new WaitForSeconds(holdDuration);
            }
        }
    }
}

