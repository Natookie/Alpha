using System;
using System.Collections.Generic;

// Used by GameSession. Deck is the concrete 52-card version
public interface IDeck
{
    int Remaining { get; }
    bool IsEmpty { get; }
    void Shuffle();
    ICard Draw();
    ICard Peek(int offset);
}

// Standard 52-card deck. Shuffle() rewinds the draw index, so the same
// instance can be reshuffled for a new round instead of rebuilt
public class Deck : IDeck
{
    private readonly List<ICard> cards = new();
    private int index;

    public int Remaining => cards.Count - index;
    public bool IsEmpty => Remaining == 0;

    public Deck(){
        foreach(Suit s in Enum.GetValues(typeof(Suit)))
            foreach(Rank r in Enum.GetValues(typeof(Rank)))
                cards.Add(new CardModel(s, r));
    }

    public void Shuffle(){
        var rng = new Random();
        for(int i = cards.Count - 1; i > 0; i--){
            int j = rng.Next(i + 1);
            ICard temp = cards[i];
            cards[i] = cards[j];
            cards[j] = temp;
        }
        index = 0;
    }

    public ICard Draw(){
        if(IsEmpty) throw new InvalidOperationException("Deck is empty.");
        return cards[index++];
    }

    //Cheat Sheet :>
    public ICard Peek(int offset){
        int i = index + offset;
        if(i < 0 || i >= cards.Count) return null;
        return cards[i];
    }
}