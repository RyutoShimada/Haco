using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyPlayer;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    [SerializeField]
    private Player _player1;
    [SerializeField]
    private Player _player2;

    private void Awake()
    {
        Instance = this;
    }

    public void StartBattle(PlayerInfo attaker, PlayerInfo defender)
    {

    }
}
