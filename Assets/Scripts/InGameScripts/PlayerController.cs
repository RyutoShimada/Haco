using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

public class PlayerController : MonoBehaviour
{
    private enum PlayerState
    {
        Stay,
        RollDice,
        SelectPawn,
        Input,
        Movement,
        CheckBattle,
        Battle,
    }

    [SerializeField]
    private User _user;
    [SerializeField]
    private Transform _leftPawnT = default;
    [SerializeField]
    private Transform _rightPawnT = default;
    [SerializeField, Header("回転速度")]
    private float _rollSpeed = 3f;
    [SerializeField]
    private byte[] _dice = { 1, 1, 2, 2, 2, 3 };

    //private Pawn _leftPawn = default;
    //private Pawn _rightPawn = default;
    private bool _isMoving = default;
    private Vector3 _input = default;
    private PlayerState _state = PlayerState.Stay;
    private byte _movableRange = default;
    private Transform _currentPawnTransform = default;
    private Coroutine _corutine;
    private PlayerData _leftData;
    private PlayerData _rightData;
    private PlayerData _currentData;

    public PlayerData LeftData { get => _leftData; }
    public PlayerData RightData { get => _rightData; }

    public System.Func<int, int, PlayerData, bool> CanMove;

    private void Awake()
    {
        //_leftPawn = new Pawn(PawnState.Attack,
        //                 PawnState.Attack,
        //                 PawnState.Shield,
        //                 PawnState.Shield,
        //                 PawnState.Wing,
        //                 PawnState.DoubleAttack,
        //                 new Vector2(_leftPawnT.position.x, _leftPawnT.position.z));

        //_rightPawn = new Pawn(PawnState.Attack,
        //                 PawnState.Attack,
        //                 PawnState.Shield,
        //                 PawnState.Shield,
        //                 PawnState.Wing,
        //                 PawnState.DoubleAttack,
        //                 new Vector2(_rightPawnT.position.x, _rightPawnT.position.z));

        _leftData = new PlayerData(_user, (int)_leftPawnT.position.x, (int)_leftPawnT.position.z);
        _rightData = new PlayerData(_user, (int)_rightPawnT.position.x, (int)_rightPawnT.position.z);
    }

    // Start is called before the first frame update
    void Start()
    {
        ChangeState(PlayerState.RollDice);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void ChangeState(PlayerState state)
    {
        _state = state;
        
        switch (_state)
        {
            case PlayerState.Stay:
                break;
            case PlayerState.RollDice:
                RollDice();
                break;
            case PlayerState.SelectPawn:
                SelectPawn();
                break;
            case PlayerState.Input:
                _corutine = StartCoroutine(OnUpdate(() => PlayerInput()));
                break;
            case PlayerState.Movement:
                Movement(_input);
                break;
            case PlayerState.CheckBattle:
                CheckBattle();
                break;
            case PlayerState.Battle:
                Battle();
                break;
            default:
                break;
        }
    }

    private void RollDice()
    {
        byte random = (byte)Random.Range(0, _dice.Length);
        if (random >= 0 && random < _dice.Length)
        {
            _movableRange = _dice[random];
            //Debug.Log($"移動可能数：{_movableRange}");
            ChangeState(PlayerState.SelectPawn);
        }
    }

    private void SelectPawn()
    {
        _currentPawnTransform = _leftPawnT;
        //_currentPawnTransform = _rightPawnPos;
        _currentData = _leftData;
        ChangeState(PlayerState.Input);
    }

    private void PlayerInput()
    {
        _input = Vector3.zero; // 初期化

        // 開発用（後でスマホ用に変更予定）
        if (_user == User.Player1)
        {
            if (Input.GetKeyDown(KeyCode.A)) { _input = Vector3.left; }
            if (Input.GetKeyDown(KeyCode.D)) { _input = Vector3.right; }
            if (Input.GetKeyDown(KeyCode.W)) { _input = Vector3.forward; }
            if (Input.GetKeyDown(KeyCode.S)) { _input = Vector3.back; }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow)) { _input = Vector3.left; }
            if (Input.GetKeyDown(KeyCode.RightArrow)) { _input = Vector3.right; }
            if (Input.GetKeyDown(KeyCode.UpArrow)) { _input = Vector3.forward; }
            if (Input.GetKeyDown(KeyCode.DownArrow)) { _input = Vector3.back; }
        }

        if (_input != Vector3.zero) {
            if (_corutine != null) StopCoroutine(_corutine);
            ChangeState(PlayerState.Movement);
        }
    }

    private void Movement(Vector3 dir)
    {
        if (_corutine != null) StopCoroutine(_corutine);

        // nullだった時動かなくなるのを防ぐ
        if (CanMove == null)
        {
            CanMove += (x, y, z) => true;
        }

        Debug.Log($"request : ({_leftData.PointX}, {_leftData.PointY}) -> ({_leftData.PointX + dir.x}, {_leftData.PointY + dir.z})  ({this})");

        if (CanMove.Invoke((int)dir.x, (int)dir.z, _currentData))
        {
            //          基準となる自分の座標　　移動の処理（Vector3.down をいれないと上に上がっていく）
            var anchor = _currentPawnTransform.position + (Vector3.down + dir) * 0.5f;
            var axis = Vector3.Cross(Vector3.up, dir);

            _leftData.ChangeState(dir);
            _corutine = StartCoroutine(Roll(anchor, axis));
        }
        else
        {
            ChangeState(PlayerState.Input);
        }
    }

    private IEnumerator Roll(Vector3 anchor, Vector3 axis)
    {
        _isMoving = true;
        for (int i = 0; i < (90 / _rollSpeed); i++)
        {
            //ワールド座標の point を中心とした軸( axis )で angle 度回転させる
            //これは Transform の位置と回転が同時に変更されます。
            _currentPawnTransform.RotateAround(anchor, axis, _rollSpeed);
            yield return null;
        }
        _isMoving = false;

        if (_corutine != null) StopCoroutine(_corutine);
        ChangeState(PlayerState.Input);
    }

    /// <summary>
    /// Updateを使いたい時だけ使えるようにした
    /// </summary>
    /// <param name="action">Updateで回したいメソッド</param>
    /// <returns></returns>
    private IEnumerator OnUpdate(System.Action action)
    {
        while (true)
        {
            yield return null;
            action(); // 1フレーム待ってから処理(でないと Stack Over Fllow する)
        }
    }

    private void CheckBattle()
    {
        // Fieldにバトルが発生するか確認
        // 発生した場合 -> Battleへ
        // 発生せず、移動が残っていれば Movementへ
        // 発生せず、移動が残っていなければ Stayに変更して、GameManagerへ通知
    }

    private void Battle()
    {
        // GameManager へ戦闘処理を任せる
    }
}
