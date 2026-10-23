using UnityEngine;
using Nova;

// Fills the HUD from GameEvents. No game logic
public class HUDUI : MonoBehaviour
{
    [Header("REFERENCES")]
    [SerializeField] private TextBlock pointText;
    [SerializeField] private UIBlock2D healthSlider;
    [SerializeField] private TextBlock healthText;
    [SerializeField] private TextBlock deckText;

    void OnEnable(){
        GameEvents.PointChanged += OnPointChanged;
        GameEvents.HealthChanged += OnHealthChanged;
        GameEvents.DeckChanged += OnDeckChanged;
    }

    void OnDisable(){
        GameEvents.PointChanged -= OnPointChanged;
        GameEvents.HealthChanged -= OnHealthChanged;
        GameEvents.DeckChanged -= OnDeckChanged;
    }

    void OnPointChanged(int point){
        if(pointText != null) pointText.Text = $"Point: {point}";
    }

    void OnHealthChanged(int health, int max){
        if(healthSlider != null){
            float value = (float)health / max;
            healthSlider.Size.X.Percent = value;
        }
        if(healthText != null) healthText.Text = $"{health} / {max}";
    }

    void OnDeckChanged(int remaining){
        if(deckText != null) deckText.Text = $"Deck: {remaining}";
    }
}