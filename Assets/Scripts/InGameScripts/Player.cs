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
        /// true なら動ける
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

            // ここをどうにかしたい
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
            // 後戻りしているかの確認
            if (_tracesMovement.Count > 0) // 既に行動していたら
            {
                // 次に移動する場所が、元いた場所かどうか確認する
                var next = input;
                next += _currentPawn.Point;
                if (_tracesMovement.Peek() == next)
                {
                    _field.CanMove(_currentPawn.Point, input);
                    _field.UpdateData(_currentPawn.Point, input, true);
                    _currentPawn.ChangeState(input);
                    _tracesMovement.Pop(); // 元いた場所に戻ったので、最後の記録を破棄

                    // Wing だったらボーナスを回復し、ボーナスがある状態だったら、それを消して、通常の移動数を回復
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

            // 移動できるか確認する
            if (_field.CanMove(_currentPawn.Point, input))
            {
                var prevPoint = _currentPawn.Point;

                _currentPawn.ChangeState(input); // ここで_currentPawnのPointが書き換わる
                _field.UpdateData(prevPoint, input);

                _tracesMovement.Push(prevPoint);
            }

            UseMovaleRange();

            // Wingだったらボーナスを追加し、バトルをしない
            if (_currentPawn.State == PawnState.Wing)
            {
                AddMovaleRange(true);
                return;
            }

            // バトルチェック
            if (_field.SearchEnemysAround(_currentPawn.Point, _user))
            {
                _canMove = false;
                _field.DoBattle();
            }
        }

        /// <summary>
        /// 可動範囲数を消費する
        /// </summary>
        private void UseMovaleRange()
        {
            // 追加移動数があれば、そちらを使用する
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
        /// 可動範囲を追加する
        /// インクリメントで追加する
        /// </summary>
        /// <param name="bonus">ボーナスがあるか</param>
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
        /// 可動範囲を追加する
        /// ※数値で可動範囲を指定する時に使う
        /// </summary>
        /// <param name="num">追加したい数</param>
        private void AddMovaleRange(byte num)
        {
            _movaleRange = num;
        }
        #endregion
    }
}

