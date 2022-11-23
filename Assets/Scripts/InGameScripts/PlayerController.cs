using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Player
{
    Player1,
    Player2,
    NPC
}

public class PlayerController : MonoBehaviour
{
    private enum PlayerState
    {
        Stay,
        RollDaice,
        SelectPawn,
        Input,
        Movement,
        CheckBattle,
        Battle,
    }

    [SerializeField]
    private Player _player;
    [SerializeField]
    private Transform _leftPawnPos = default;
    [SerializeField]
    private Transform _rightPawnPos = default;
    [SerializeField, Header("回転速度")]
    private float _rollSpeed = 3f;
    [SerializeField]
    private byte[] _dice = { 1, 1, 2, 2, 2, 3 };

    private Pawn _leftPawn = default;
    private Pawn _rightPawn = default;
    private bool _isMoving = default;
    private Vector3 _input = default;
    private PlayerState _state = PlayerState.Stay;
    private byte _movableRange = default;
    private Transform _currentPawnTransform = default;
    private Coroutine _corutine;

    public Player Player { get => _player; }
    public PawnState State { get => _leftPawn.State; }


    private void Awake()
    {
        _leftPawn = new Pawn(PawnState.Attack,
                         PawnState.Attack,
                         PawnState.Shield,
                         PawnState.Shield,
                         PawnState.Wing,
                         PawnState.DoubleAttack,
                         new Vector2(_leftPawnPos.position.x, _leftPawnPos.position.z));

        _rightPawn = new Pawn(PawnState.Attack,
                         PawnState.Attack,
                         PawnState.Shield,
                         PawnState.Shield,
                         PawnState.Wing,
                         PawnState.DoubleAttack,
                         new Vector2(_rightPawnPos.position.x, _rightPawnPos.position.z));
    }

    // Start is called before the first frame update
    void Start()
    {
        ChangeState(PlayerState.RollDaice);
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void ChangeState(PlayerState state)
    {
        _state = state;
        Debug.Log(state);

        switch (_state)
        {
            case PlayerState.Stay:
                break;
            case PlayerState.RollDaice:
                RollDaice();
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

    private void RollDaice()
    {
        byte random = (byte)Random.Range(0, _dice.Length);
        if (random >= 0 && random < _dice.Length)
        {
            _movableRange = _dice[random];
            Debug.Log($"移動可能数：{_movableRange}");
            ChangeState(PlayerState.SelectPawn);
        }
    }

    private void SelectPawn()
    {
        _currentPawnTransform = _leftPawnPos;
        //_currentPawnTransform = _rightPawnPos;
        ChangeState(PlayerState.Input);
    }

    private void PlayerInput()
    {
        _input = Vector3.zero; // 初期化

        // 開発用（後でスマホ用に変更予定）
        if (Player == Player.Player1)
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

        if (FieldController.Instance.CanMove(_currentPawnTransform.position, dir))
        {
            //          基準となる自分の座標　　移動の処理（Vector3.down をいれないと上に上がっていく）
            var anchor = _currentPawnTransform.position + (Vector3.down + dir) * 0.5f;
            var axis = Vector3.Cross(Vector3.up, dir);

            // 更新 ※Stateに代入できるのは避けたい（この形式はどうにかしたい）
            _leftPawn.ChangeState(dir);
            //FieldController.Instance.ChangePos(dir, _playerModel);
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
