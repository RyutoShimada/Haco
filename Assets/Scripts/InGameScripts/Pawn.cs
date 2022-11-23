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

public class Pawn
{
    public PawnState State { get; private set; }
    public Vector2 CellPos { get => _cellPos; }

    private Dictionary<Vector3, PawnState> _stateDic = new Dictionary<Vector3, PawnState>();
    private Vector2 _cellPos;

    public Pawn(PawnState forwardState, PawnState backState, PawnState leftState, PawnState rightState, PawnState topState, PawnState bottomState, Vector2 cellPos)
    {
        _stateDic.Add(Vector3.forward, forwardState);
        _stateDic.Add(Vector3.back, backState);
        _stateDic.Add(Vector3.left, leftState);
        _stateDic.Add(Vector3.right, rightState);
        _stateDic.Add(Vector3.up, topState);
        _stateDic.Add(Vector3.down, bottomState);
        _cellPos = cellPos;
        State = _stateDic[Vector3.up];
    }

    public Vector2 ChangeCellPos(Vector2 cellPos)
    {
        return _cellPos = cellPos;
    }

    public PawnState ChangeState(Vector3 input)
    {
        if (input == Vector3.forward)
        {
            return State = ForwardRoll();
        }
        if (input == Vector3.back)
        {
            return State = BackRoll();
        }
        if (input == Vector3.right)
        {
            return State = RightRoll();
        }
        if (input == Vector3.left)
        {
            return State = LeftRoll();
        }

        return PawnState.None;
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
}
