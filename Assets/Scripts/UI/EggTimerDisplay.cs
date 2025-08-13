using TMPro;
using UnityEngine;

public class EggTimerDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerText;
    private float _duration;
    private float _timeLeft;
    private System.Action _onTimerComplete;
    
    public void Initialize(float duration, System.Action onComplete)
    {
        _duration = duration;
        _timeLeft = duration;
        _onTimerComplete = onComplete;
        UpdateTimerText();
    }

    private void Update()
    {
        if (_timeLeft > 0)
        {
            _timeLeft -= Time.deltaTime;
            UpdateTimerText();
            
            if (_timeLeft <= 0)
            {
                _onTimerComplete?.Invoke();
                Destroy(gameObject);
            }
        }
    }

    private void UpdateTimerText()
    {
        _timerText.text = Mathf.CeilToInt(_timeLeft).ToString();
    }
}
