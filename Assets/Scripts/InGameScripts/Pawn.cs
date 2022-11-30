using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PawnState
{
    Attack,
    Shield,
    DoubleAttack,
    Wing,
    None = 0
}

public class Pawn : MonoBehaviour
{
    public PawnState State { get; private set; }
    public int PointX { get; private set; }
    public int PointZ { get; private set; }
    public bool IsMoving { get => _isMoving; }

    private Dictionary<Vector3, PawnState> _stateDic = new Dictionary<Vector3, PawnState>();
    private float _rollSpeed = 2.5f;
    private bool _isMoving = false;

    private void Start()
    {
        SetState();
    }

    public void SetState()
    {
        _stateDic.Add(Vector3.forward, PawnState.Attack);
        _stateDic.Add(Vector3.back, PawnState.Attack);
        _stateDic.Add(Vector3.left, PawnState.Shield);
        _stateDic.Add(Vector3.right, PawnState.Shield);
        _stateDic.Add(Vector3.up, PawnState.Wing);
        _stateDic.Add(Vector3.down, PawnState.DoubleAttack);
        PointX = (int)transform.position.x;
        PointZ = (int)transform.position.z;
        State = _stateDic[Vector3.up];
    }

    public void ChangeState(Vector3 input)
    {
        if (_isMoving) return;

        PointX += (int)input.x;
        PointZ += (int)input.z;

        if (input == Vector3.forward)
        {
            State = ForwardRoll();
            StartCoroutine(Roll(input));
        }
        if (input == Vector3.back)
        {
            State = BackRoll();
            StartCoroutine(Roll(input));
        }
        if (input == Vector3.right)
        {
            State = RightRoll();
            StartCoroutine(Roll(input));
        }
        if (input == Vector3.left)
        {
            State = LeftRoll();
            StartCoroutine(Roll(input));
        }
    }

    private PawnState ForwardRoll()
    {
        var prevTop = _stateDic[Vector3.up];
        var prevBottom = _stateDic[Vector3.down];
        var prevFoward = _stateDic[Vector3.forward];
        var prevBack = _stateDic[Vector3.back];

        // back -> top
        _stateDic[Vector3.up] = prevBack;
        // top -> foward
        _stateDic[Vector3.forward] = prevTop;
        // foward -> bottom
        _stateDic[Vector3.down] = prevFoward;
        // bottom -> back
        _stateDic[Vector3.back] = prevBottom;

        return _stateDic[Vector3.up];
    }

    private PawnState BackRoll()
    {
        var prevTop = _stateDic[Vector3.up];
        var prevBottom = _stateDic[Vector3.down];
        var prevFoward = _stateDic[Vector3.forward];
        var prevBack = _stateDic[Vector3.back];

        // foward -> top
        _stateDic[Vector3.up] = prevFoward;
        // top -> back
        _stateDic[Vector3.back] = prevTop;
        // back -> bottom
        _stateDic[Vector3.down] = prevBack;
        // bottom -> foward
        _stateDic[Vector3.forward] = prevBottom;

        return _stateDic[Vector3.up];
    }

    private PawnState RightRoll()
    {
        var prevTop = _stateDic[Vector3.up];
        var prevBottom = _stateDic[Vector3.down];
        var prevLeft = _stateDic[Vector3.left];
        var prevRight = _stateDic[Vector3.right];

        // left -> top
        _stateDic[Vector3.up] = prevLeft;
        // top -> right
        _stateDic[Vector3.right] = prevTop;
        // right -> bottom
        _stateDic[Vector3.down] = prevRight;
        // bottom -> left
        _stateDic[Vector3.left] = prevBottom;

        return _stateDic[Vector3.up];
    }

    private PawnState LeftRoll()
    {
        var prevTop = _stateDic[Vector3.up];
        var prevBottom = _stateDic[Vector3.down];
        var prevLeft = _stateDic[Vector3.left];
        var prevRight = _stateDic[Vector3.right];

        // right -> top
        _stateDic[Vector3.up] = prevRight;
        // top -> left
        _stateDic[Vector3.left] = prevTop;
        // left -> bottom
        _stateDic[Vector3.down] = prevLeft;
        // bottom -> right
        _stateDic[Vector3.right] = prevBottom;

        return _stateDic[Vector3.up];
    }

    private IEnumerator Roll(Vector3 dir)
    {
        var anchor = transform.position + (Vector3.down + dir) * 0.5f;
        var axis = Vector3.Cross(Vector3.up, dir);

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
