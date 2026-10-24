# Higher or Lower Card Game

A small Unity card game in which the player predicts whether the next card has a higher or lower rank than the current card. The project is built with Unity 6.2 and uses Nova UI components.

## Run the project

1. Install **Unity 6.2.6f2** (editor version `6000.2.6f2`) through Unity Hub.
2. Open the Unity project folder (the one containing `Assets`, `Packages`, and `ProjectSettings`).
3. Let Unity resolve the packages listed in `Packages/manifest.json`. The project uses Nova UI assets included in `Assets/Nova`.
4. In the Project window, open `Assets/_Projects/Scenes/SampleScene.unity`.
5. Press **Set Up Card**, then choose **Higher** or **Lower** for each next card. The round ends when the 52-card deck is empty (win) or the player's health reaches zero (lose).

`GameUI` exposes `cardShowCount` (how many cards are visible at once, default `5`) and `gameOverDelay` (delay before returning to the menu after a round ends). Game rules — starting health, points per correct guess, damage per wrong guess, Ace-high comparison, and suit tiebreak — live on a `GameData` ScriptableObject, injected into `GameSession`.

## Code organization

Scripts are grouped by responsibility under `Assets/_Projects/1. Scripts`:

- **Game** — `GameSession` coordinates a round, `PlayerData` stores score and health, `GameStateMachine` manages state transitions, `GameEvents` publishes presentation updates, and `GameData` (a ScriptableObject) holds tunable rules like starting health, points per correct guess, damage per wrong guess, and the Ace-high flag.
- **Deck** — `Deck` implements `IDeck` and supplies a shuffled standard deck of `ICard` models.
- **Card** — `CardModel` stores rank and suit; `CardView` binds a model to its card sprite and controls whether it is revealed.
- **UI/Panel** — `GameUI` owns the session and menu/card/game-over flow; `HUDUI` listens for events and updates score, health, and remaining cards.
- **UI/Element** — `CoreButton` adapts Nova interaction gestures into UnityEvents; `UIBlockAnimator` supplies reusable visual animations.

## Architecture

The gameplay model is kept separate from its Unity/Nova presentation. `GameSession` is built from injected pieces, a `GameData` config asset, a `GameStateMachine`, and a `Func<IDeck>` deck factory, so no concrete dependencies are constructed inside the session. 

It applies guesses, updates `PlayerData`, and raises `GameEvents`. UI components subscribe and render those changes. `GameUI` translates button clicks into session actions and toggles the menu, gameplay, and game-over panels directly.

Card comparison lives in `CardModel.CompareValue` (rank, respecting `GameData.aceHigh`) and `CardModel.SuitWeight` (♣ > ♦ > ♥ > ♠). `GameSession.IsHigher` combines them. The state machine models the menu, gameplay, and game-over lifecycle; the menu panels themselves are currently switched directly by `GameUI`.

```mermaid
classDiagram
    class GameUI {
        -GameSession session
        +StartGame()
        +OnCardsRevealed(current, next)
        +OnGameEnded(won)
    }
    class HUDUI {
        +OnPointChanged(point)
        +OnHealthChanged(health, max)
        +OnDeckChanged(remaining)
    }
    class GameSession {
        +PlayerData Player
        +GameStateMachine Fsm
        +StartNewGame()
        +GuessHigher(higher)
        +IsHigher(a, b)
    }
    class PlayerData {
        +int Point
        +int Health
        +int MaxHealth
        +AddPoint(amount)
        +LoseHealth(amount)
        +Reset()
    }
    class GameData {
        <<ScriptableObject>>
        +int startingHealth
        +int pointsPerCorrectGuess
        +int damagePerWrongGuess
        +bool aceHigh
        +int cardShowCount
        +float gameOverDelay
    }
    class GameStateMachine {
        +IGameState Current
        +ChangeState(next)
    }
    class IGameState {
        <<interface>>
        +Enter()
        +Exit()
    }
    class MainMenuState
    class GameplayState
    class GameOverState
    class GameEvents {
        <<static>>
        +CardsRevealed
        +PointChanged
        +HealthChanged
        +DeckChanged
        +GameEnded
    }
    class IDeck {
        <<interface>>
        +Shuffle()
        +Draw()
    }
    class Deck
    class ICard {
        <<interface>>
        +Suit
        +Rank
        +Value
    }
    class CardModel {
        +CompareValue(card, data)
        +SuitWeight(suit)
    }
    class CardView
    class CoreButton
    class UIBlockAnimator

    GameUI --> GameSession : creates and invokes
    GameUI --> GameData : passes in
    GameUI ..> GameEvents : subscribes
    HUDUI ..> GameEvents : subscribes
    GameSession --> PlayerData : owns
    GameSession --> GameStateMachine : injected
    GameSession --> GameData : reads rules
    GameSession --> IDeck : injected factory
    GameSession ..> GameEvents : raises
    GameStateMachine o--> IGameState : current
    IGameState <|.. MainMenuState
    IGameState <|.. GameplayState
    IGameState <|.. GameOverState
    IDeck <|.. Deck
    Deck ..> ICard : creates
    ICard <|.. CardModel
    CardView ..> ICard : displays
    CoreButton --> UIBlockAnimator : animates with
    GameUI --> CardView : creates and binds