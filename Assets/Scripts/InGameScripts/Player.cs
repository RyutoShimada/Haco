using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyPlayer
{
    public enum User
    {
        Player1,
        Player2,
        NPC
    }

    public class PlayerInfo
    {
        public PawnState PawnState { get; private set; }

        public User User { get; private set; }

        public PlayerInfo(PawnState state, User user)
        {
            PawnState = state;
            User = user;
        }

        public void ChangeState(PawnState state)
        {
            PawnState = state;
        }
    }

    public class Player : MonoBehaviour
    {
        [SerializeField]
        private User _user;
        [SerializeField]
        private Pawn _pawn1;
        [SerializeField]
        private Pawn _pawn2;
        [SerializeField]
        private Field _cell;

        private PlayerInfo _playerInfo1;
        private PlayerInfo _playerInfo2;
        private Pawn _currentPawn;

        private void Awake()
        {
            
        }

        void Start()
        {
            _playerInfo1 = new PlayerInfo(_pawn1.State, _user);
            _playerInfo2 = new PlayerInfo(_pawn2.State, _user);

            // ‚±‚±‚ð‚Ç‚¤‚É‚©‚µ‚½‚¢
            _cell.SetPlayer((int)_pawn1.PointX + 2, (int)_pawn1.PointZ + 2, _playerInfo1);
            _cell.SetPlayer((int)_pawn2.PointX + 2, (int)_pawn2.PointZ + 2, _playerInfo2);

            _currentPawn = _pawn1;
        }

        // Update is called once per frame
        void Update()
        {
            if (_currentPawn.IsMoving) return;

            if (Input.GetKeyDown(KeyCode.W))
            {
                if(_cell.MoveTo(_currentPawn.PointX, _currentPawn.PointZ, Vector3.forward))
                _currentPawn.ChangeState(Vector3.forward);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                if(_cell.MoveTo(_currentPawn.PointX, _currentPawn.PointZ, Vector3.left))
                _currentPawn.ChangeState(Vector3.left);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                if(_cell.MoveTo(_currentPawn.PointX, _currentPawn.PointZ, Vector3.back))
                _currentPawn.ChangeState(Vector3.back);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                if (_cell.MoveTo(_currentPawn.PointX, _currentPawn.PointZ, Vector3.right))
                _currentPawn.ChangeState(Vector3.right);
            }
        }
    }
}

