using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Player
{
    Player1,
    Player2,
    NPC
}

public class PlayerModel
{
    public Player Player { get; private set; }

    public PawnState State { get; set; }

    public PlayerModel(Player player, PawnState state)
    {
        Player = player;
        State = state;
    }
}
