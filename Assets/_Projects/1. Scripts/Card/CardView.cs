using UnityEngine;
using Nova;

// One card on screen. Bind() picks the sprite id from the model,
// Reveal() flips between face and back.
public class CardView : MonoBehaviour
{
    [SerializeField] private UIBlock2D cardBlock;

    private const string CardFolder = "CardDeck/";
    private const string BackId = "CardDeck/back_light";
    private string spriteId = "";

    void Awake(){
        if(cardBlock == null) cardBlock = GetComponent<UIBlock2D>();
    }

    public void Bind(ICard card){
        spriteId = $"{card.Suit}_{RankShort(card.Rank)}".ToLowerInvariant();
        gameObject.name = spriteId;
        Reveal(false);
    }

    public void Reveal(bool isShown){
        string path = isShown ? CardFolder + spriteId : BackId;
        Sprite sprite = Resources.Load<Sprite>(path);
        if(sprite == null) Debug.LogWarning($"Sprite not found: {path}");
        cardBlock.SetImage(sprite);
    }

    private static string RankShort(Rank r) => r switch{
        Rank.Ace => "A",
        Rank.Jack => "J",
        Rank.Queen => "Q",
        Rank.King => "K",
        _ => ((int)r).ToString()
    };
}