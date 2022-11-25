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
        /// �v���C���Ă��郆�[�U�[
        /// </summary>
        public User User { get; set; }
        /// <summary>
        /// �R�}�̏��
        /// </summary>
        public PawnState State { get; set; }
        public int PointX { get; set; }
        public int PointY { get; set; }

        public PlayerData(User u, PawnState s, int x, int y)
        {
            User = u;
            State = s;
            PointX = x;
            PointY = y;
        }
    }
}

