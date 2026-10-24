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

    // Ace = 1 in the enum, but it plays as 14 in high-low.
    public static int CompareValue(ICard card, GameData data){
        if(data.aceHigh && card.Rank == Rank.Ace) return 14;
        return (int)card.Rank;
    }

    // Clubs > Diamonds > Hearts > Spades. Used when ranks are equal
    public static int SuitWeight(Suit suit) => suit switch{
        Suit.Clubs      => 4,
        Suit.Diamonds   => 3,
        Suit.Hearts     => 2,
        Suit.Spades     => 1,
        _ => 0
    };

    public CardModel(Suit suit, Rank rank) { Suit = suit; Rank = rank; }
    public override string ToString() => $"{Rank} of {Suit}";
}