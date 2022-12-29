using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyPlayer
{
    #region PublicEnum
    public enum User
    {
        Player1,
        Player2,
        NPC
    }
    #endregion

    #region PlayerInfoClass
    public class PlayerInfo
    {
        public PawnState PawnState { get => _pawn.State; }

        public User User { get; private set; }

        private PawnController _pawn;

        public PlayerInfo(User user, PawnController pawn)
        {
            User = user;
            _pawn = pawn;
        }
    }
    #endregion

    public class Player : MonoBehaviour
    {
        #region PrivateEnum
        private enum SelectPawn
        {
            Left,
            Right
        }

        private enum State
        {
            Stay,
            Select,
            Move
        }
        #endregion

        #region SerializeField
        [SerializeField]
        private User _user = default;
        [SerializeField]
        private PawnController _pawnL = default;
        [SerializeField]
        private PawnController _pawnR = default;
        [SerializeField]
        private FieldManager _field = default;
        [SerializeField]
        private SelectPawn _selectPawn = default;
        #endregion

        #region PrivateField
        private PlayerInfo _playerInfo1 = default;
        private PlayerInfo _playerInfo2 = default;
        private PawnController _currentPawn = default;
        private byte[] _dice = { 1, 1, 2, 2, 2, 3 };
        private Stack<Vector3> _tracesMovement = new Stack<Vector3>();
        private State _currentState = State.Stay;
        #endregion

        #region PublicField
        public byte _movaleRange = default;
        public byte _bonusMovaleRange = default;
        /// <summary>
        /// true �Ȃ瓮����
        /// </summary>
        public bool _canMove;
        #endregion

        #region MonoBehaviour

        private void OnValidate()
        {
            ChangeSelectPawn(_selectPawn);
        }

        private void Awake()
        {

        }

        void Start()
        {
            _playerInfo1 = new PlayerInfo(_user, _pawnL);
            _playerInfo2 = new PlayerInfo(_user, _pawnR);

            // �������ǂ��ɂ�������
            _field.SetPlayer((int)_pawnL.PointX + 2, (int)_pawnL.PointZ + 2, _playerInfo1);
            _field.SetPlayer((int)_pawnR.PointX + 2, (int)_pawnR.PointZ + 2, _playerInfo2);

            _currentPawn = _pawnL;
            RollDice();
        }

        // Update is called once per frame
        void Update()
        {
            if (_canMove != true) return;
            if (_currentPawn.IsMoving) return;
            if (_bonusMovaleRange <= 0 && _movaleRange <= 0)
            {
                _movaleRange = 0;
                return;
            }

            if (Input.GetKeyDown(KeyCode.W))
            {
                Movement(Vector3.forward);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                Movement(Vector3.left);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                Movement(Vector3.back);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                Movement(Vector3.right);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _currentPawn.Move(Vector3.forward, 1);
            }
        }
        #endregion

        #region PublicMethod
        public void CanMove(bool move)
        {
            _canMove = move;
        }

        public void RollDice()
        {
            byte random = (byte)Random.Range(0, _dice.Length);
            AddMovaleRange(_dice[random]);
        }
        #endregion

        #region PrivateMethod
        private void ChangeSelectPawn(SelectPawn select)
        {
            _selectPawn = select;

            switch (_selectPawn)
            {
                case SelectPawn.Left:
                    _currentPawn = _pawnL;
                    break;
                case SelectPawn.Right:
                    _currentPawn = _pawnR;
                    break;
                default:
                    break;
            }
        }

        private void Movement(Vector3 input)
        {
            // ��߂肵�Ă��邩�̊m�F
            if (_tracesMovement.Count > 0)
            {
                // ���Ɉړ�����ꏊ���A�������ꏊ���ǂ����m�F����
                var next = input;
                next.x += _currentPawn.PointX;
                next.z += _currentPawn.PointZ;
                if (_tracesMovement.Peek() == next)
                {
                    _field.MoveTo(_currentPawn.PointX, _currentPawn.PointZ, input, true);
                    _currentPawn.ChangeState(input);
                    _tracesMovement.Pop(); // �������ꏊ�ɖ߂����̂ŁA�Ō�̋L�^��j��

                    // Wing ��������{�[�i�X���񕜂��A�{�[�i�X�������Ԃ�������A����������āA�ʏ�̈ړ�������
                    if (_currentPawn.State != PawnState.Wing)
                    {
                        AddMovaleRange(false);
                        if (_bonusMovaleRange > 0) UseMovaleRange();
                    }
                    else
                    {
                        AddMovaleRange(true);
                    }
                    return;
                }
            }

            // �ړ��ł��邩�m�F����
            if (_field.MoveTo(_currentPawn.PointX, _currentPawn.PointZ, input, false))
            {
                var prevX = _currentPawn.PointX;
                var prevZ = _currentPawn.PointZ;

                _currentPawn.ChangeState(input); // ������_currentPawn��Point�����������

                // ��new �������Ȃ��̂�input������������
                input.x = prevX;
                input.z = prevZ;
                _tracesMovement.Push(input);
            }

            UseMovaleRange();

            // Wing��������{�[�i�X��ǉ����A�o�g�������Ȃ�
            if (_currentPawn.State == PawnState.Wing)
            {
                AddMovaleRange(true);
                return;
            }

            // �o�g���`�F�b�N
            if (_field.SearchEnemysAround(_currentPawn.PointX, _currentPawn.PointZ, _user))
            {
                _canMove = false;
            }
        }

        /// <summary>
        /// ���͈͐��������
        /// </summary>
        private void UseMovaleRange()
        {
            // �ǉ��ړ���������΁A��������g�p����
            if (_bonusMovaleRange > 0)
            {
                _bonusMovaleRange--;
            }
            else
            {
                _movaleRange--;
            }
        }

        /// <summary>
        /// ���͈͂�ǉ�����
        /// �C���N�������g�Œǉ�����
        /// </summary>
        /// <param name="bonus">�{�[�i�X�����邩</param>
        private void AddMovaleRange(bool bonus)
        {
            if (bonus)
            {
                _bonusMovaleRange++;
            }
            else
            {
                _movaleRange++;
            }
        }

        /// <summary>
        /// ���͈͂�ǉ�����
        /// �����l�ŉ��͈͂��w�肷�鎞�Ɏg��
        /// </summary>
        /// <param name="num">�ǉ���������</param>
        private void AddMovaleRange(byte num)
        {
            _movaleRange = num;
        }
        #endregion
    }
}

