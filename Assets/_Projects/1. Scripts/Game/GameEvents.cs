using System;

// GameSession fires these and UI listens
public static class GameEvents
{
    public static event Action<ICard, ICard> CardsRevealed;
    public static event Action<int> PointChanged;
    public static event Action<int, int> HealthChanged;
    public static event Action<int> DeckChanged;
    public static event Action<bool> GameEnded;

    public static void RaiseCardsRevealed(ICard current, ICard next) => CardsRevealed?.Invoke(current, next);
    public static void RaisePointChanged(int point) => PointChanged?.Invoke(point);
    public static void RaiseHealthChanged(int health, int max) => HealthChanged?.Invoke(health, max);
    public static void RaiseDeckChanged(int remaining) => DeckChanged?.Invoke(remaining);
    public static void RaiseGameEnded(bool won) => GameEnded?.Invoke(won);
}