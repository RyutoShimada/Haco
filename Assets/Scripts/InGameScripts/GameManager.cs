using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum Player
    {
        Player1,
        Player2,
        NPC
    }

    public enum GamePhase
    {
        GameStart,
        ChangeTurn,
        PlayerBehavior,
        CheckLife,
        GameOver
    }

    private GamePhase _currentGamePhase;
    private GamePhase _prevGamePhase;

    private void Start()
    {
        ChangePhase(GamePhase.GameStart);
    }

    private void ChangePhase(GamePhase nextPhase)
    {
        _prevGamePhase = _currentGamePhase;
        _currentGamePhase = nextPhase;
        Debug.Log(_currentGamePhase);

        switch (_currentGamePhase)
        {
            case GamePhase.GameStart:
                GameStart();
                break;
            case GamePhase.ChangeTurn:
                ChangeTurn();
                break;
            case GamePhase.PlayerBehavior:
                PlayerBehavior();
                break;
            case GamePhase.CheckLife:
                CheckLife();
                break;
            case GamePhase.GameOver:
                GameOver();
                break;
            default:
                break;
        }
    }
}
