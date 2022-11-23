using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Player _player;
    private Pawn _pawn;
    public PlayerModel _playerModel;


    [SerializeField, Header("回転速度")]
    private float _rollSpeed = 3f;

    private bool _isMoving;

    private void Awake()
    {
        _pawn = new Pawn(PawnState.Attack,
                         PawnState.Attack,
                         PawnState.Shield,
                         PawnState.Shield,
                         PawnState.Wing,
                         PawnState.DoubleAttack);

        _playerModel = new PlayerModel(_player, _pawn.State);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_isMoving) { return; }

        if (Input.GetKeyDown(KeyCode.A)) { Assemble(Vector3.left); }
        if (Input.GetKeyDown(KeyCode.D)) { Assemble(Vector3.right); }
        if (Input.GetKeyDown(KeyCode.W)) { Assemble(Vector3.forward); }
        if (Input.GetKeyDown(KeyCode.S)) { Assemble(Vector3.back); }

        void Assemble(Vector3 dir)
        {
            if (FieldController.Instance.CanMove(transform.position, dir))
            {
                //          基準となる自分の座標　　移動の処理（Vector3.down をいれないと上に上がっていく）
                var anchor = transform.position + (Vector3.down + dir) * 0.5f;
                var axis = Vector3.Cross(Vector3.up, dir);
                StartCoroutine(Roll(anchor, axis));

                // 更新 ※Stateに代入できるのは避けたい（この形式はどうにかしたい）
                _playerModel.State = _pawn.ChangeState(dir);
                FieldController.Instance.ChangePos(dir, _playerModel);
            }
        }
    }

    private IEnumerator Roll(Vector3 anchor, Vector3 axis)
    {
        _isMoving = true;
        for (int i = 0; i < (90 / _rollSpeed); i++)
        {
            //ワールド座標の point を中心とした軸( axis )で angle 度回転させる
            //これは Transform の位置と回転が同時に変更されます。
            transform.RotateAround(anchor, axis, _rollSpeed);
            yield return null;
        }
        _isMoving = false;
    }
}
