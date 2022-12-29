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
        private Player _player;

        public PlayerInfo(User user, PawnController pawn, Player player)
        {
            User = user;
            _pawn = pawn;
            _player = player;
        }

        public void DoMove(Vector3 vec)
        {
            _pawn.Move(vec);
        }

        public void BeAttacked(int damage)
        {
            _player.Life -= damage;
            if (_player.Life <= 0)
            {
                _player.CanMove(false);
                _player.Life = 0;
            }
        }
    }
    #endregion

    public class Player : MonoBehaviour
    {
        #region PrivateEnum
        private enum SelectPawn
        {
            Up,
            Down
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
        private PawnController _pawnU = default;
        [SerializeField]
        private PawnController _pawnD = default;
        [SerializeField]
        private FieldManager _field = default;
        [SerializeField]
        private SelectPawn _selectPawn = SelectPawn.Down;
        [SerializeField]
        private int _life = 3;
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
        public int Life
        {
            get { return _life; }
            set { _life = value; }
        }
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
            _playerInfo1 = new PlayerInfo(_user, _pawnU, this);
            _playerInfo2 = new PlayerInfo(_user, _pawnD, this);

            // �������ǂ��ɂ�������
            _field.SetPlayer(_pawnU.Point, _playerInfo1);
            _field.SetPlayer(_pawnD.Point, _playerInfo2);

            _currentPawn = _pawnD;
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
                Movement(Vector3Int.forward);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                Movement(Vector3Int.left);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                Movement(Vector3Int.back);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                Movement(Vector3Int.right);
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
                case SelectPawn.Up:
                    _currentPawn = _pawnU;
                    break;
                case SelectPawn.Down:
                    _currentPawn = _pawnD;
                    break;
                default:
                    break;
            }
        }

        private void Movement(Vector3Int input)
        {
            // ��߂肵�Ă��邩�̊m�F
            if (_tracesMovement.Count > 0) // ���ɍs�����Ă�����
            {
                // ���Ɉړ�����ꏊ���A�������ꏊ���ǂ����m�F����
                var next = input;
                next += _currentPawn.Point;
                if (_tracesMovement.Peek() == next)
                {
                    _field.CanMove(_currentPawn.Point, input);
                    _field.UpdateData(_currentPawn.Point, input, true);
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
            if (_field.CanMove(_currentPawn.Point, input))
            {
                var prevPoint = _currentPawn.Point;

                _currentPawn.ChangeState(input); // ������_currentPawn��Point�����������
                _field.UpdateData(prevPoint, input);

                _tracesMovement.Push(prevPoint);
            }

            UseMovaleRange();

            // Wing��������{�[�i�X��ǉ����A�o�g�������Ȃ�
            if (_currentPawn.State == PawnState.Wing)
            {
                AddMovaleRange(true);
                return;
            }

            // �o�g���`�F�b�N
            if (_field.SearchEnemysAround(_currentPawn.Point, _user))
            {
                _canMove = false;
                _field.DoBattle();
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

