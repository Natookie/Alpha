using System.Text;
using UnityEngine;
using Nova;
using NaughtyAttributes;

public class DebugCheatSheet : MonoBehaviour
{
    [Header("REFERENCES")]
    [Tooltip("Harus nya semua yg berhubungan ama peak card, di safe guard pake if unity editor, cuman saya skip")]
    [SerializeField] private bool showCheatSheet = true;

    [Header("REFERENCES")]
    [ShowIf("showCheatSheet")][SerializeField] private GameUI gameUI;
    [ShowIf("showCheatSheet")][SerializeField] private TextBlock cheatText;
    [ShowIf("showCheatSheet")][SerializeField] private TextBlock moveText;

    [Header("SETTINGS")]
    [ShowIf("showCheatSheet")][SerializeField] private int previewCount = 10;

    void Update(){
        if(!showCheatSheet) return;
        if(gameUI == null || cheatText == null) return;

        var session = gameUI.Session;
        if(session == null) return;

        var sb = new StringBuilder();
        sb.AppendLine($"Current: {session.CurrentCard}");
        sb.AppendLine("Next cards:");
        for(int i = 0; i < previewCount; i++){
            var c = session.PeekNext(i);
            if(c == null) break;
            sb.AppendLine($"  [{i + 1}] {c}");
        }
        cheatText.Text = sb.ToString();

        var current = session.CurrentCard;
        var next = session.PeekNext(0);
        if(current != null && next != null && moveText != null){
            bool higher = session.OptimalMoveIsHigher();
            moveText.Text = higher ? "→ HIGHER" : "→ LOWER";
        }
    }
}