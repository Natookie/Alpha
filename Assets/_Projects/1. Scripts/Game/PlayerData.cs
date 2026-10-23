using UnityEngine;

// Score and health for the current round. Reset() wipes both so the same
// instance can be reused across games
public class PlayerData
{
    public int Point { get; private set; }
    public int Health { get; private set; }
    public int MaxHealth { get; }
    public bool IsAlive => Health > 0;

    public PlayerData(int maxHealth) => MaxHealth = maxHealth;
    public void AddPoint(int amount = 1) => Point += amount;
    public bool LoseHealth(int amount = 1){
        Health = Mathf.Max(0, Health - amount);
        return IsAlive;
    }
    public void Reset(){
        Point = 0;
        Health = MaxHealth;
    }
}