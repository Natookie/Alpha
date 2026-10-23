using System;

public enum Suit { Hearts, Diamonds, Clubs, Spades }
public enum Rank { Ace = 1, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King }

// Ace = 1, King = 13. Deck and GameSession compare cards by Value
public interface ICard
{
    Suit Suit { get; }
    Rank Rank { get; }
    int Value { get; }
}

public class CardModel : ICard
{
    public Suit Suit { get; }
    public Rank Rank { get; }
    public int Value => (int)Rank;

    public CardModel(Suit suit, Rank rank) { Suit = suit; Rank = rank; }
    public override string ToString() => $"{Rank} of {Suit}";
}