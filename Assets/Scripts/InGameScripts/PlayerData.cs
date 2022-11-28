using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public enum User
    {
        Player1,
        Player2,
        NPC
    }

    public class PlayerData
    {
        /// <summary>
        /// プレイしているユーザー
        /// </summary>
        public User User { get; set; }
        /// <summary>
        /// コマの状態
        /// </summary>
        public PawnState State { get => _pawn.State; }

        public int PointX { get => (int)_pawn.Point.x; }
        public int PointY { get => (int)_pawn.Point.y; }

        private Pawn _pawn;

        public PlayerData(User u, int x, int y)
        {
            User = u;

            _pawn = new Pawn(PawnState.Attack,
                         PawnState.Attack,
                         PawnState.Shield,
                         PawnState.Shield,
                         PawnState.Wing,
                         PawnState.DoubleAttack,
                         new Vector2(x, y));
        }

        public void ChangeState(Vector3 input)
        {
            _pawn.ChangeState(input);
        }
    }
}

