using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region PublicEnum
public enum PawnState
{
    Attack,
    Shield,
    DoubleAttack,
    Wing,
}
#endregion

public class PawnController : MonoBehaviour
{
    #region SerializeField
    [SerializeField]
    private PawnState _foward;
    [SerializeField]
    private PawnState _back;
    [SerializeField]
    private PawnState _left;
    [SerializeField]
    private PawnState _right;
    [SerializeField]
    private PawnState _up;
    [SerializeField]
    private PawnState _down;
    [SerializeField]
    private float _rollSpeed = 2.5f;
    [SerializeField]
    private float _moveSpeed = 2f;
    [SerializeField]
    private float _moveSmoothness = 100f;
    #endregion

    #region PublicField
    public PawnState State { get; private set; }
    public Vector3Int Point { get => _point; }
    public bool IsMoving { get => _isMoving; }
    #endregion

    #region PrivateField
    private Dictionary<Vector3, PawnState> _stateDic = new Dictionary<Vector3, PawnState>();
    private Vector3Int _point;
    private bool _isMoving = false;
    #endregion

    #region MonoBehaviour
    private void Awake()
    {
        SetState();
    }

    private void Start()
    {
        
    }
    #endregion

    #region PublicMethod
    public void SetState()
    {
        _stateDic.Add(Vector3.forward, _foward);
        _stateDic.Add(Vector3.back, _back);
        _stateDic.Add(Vector3.left, _left);
        _stateDic.Add(Vector3.right, _right);
        _stateDic.Add(Vector3.up, _up);
        _stateDic.Add(Vector3.down, _down);
        _point = new Vector3Int((int)transform.position.x, 0, (int)transform.position.z);
        State = _stateDic[Vector3.up];
    }

    public void ChangeState(Vector3Int input)
    {
        if (_isMoving) return;
        
        _point += input;

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

    public void Move(Vector3 dir)
    {
        var intVec = new Vector3Int((int)dir.x, 0, (int)dir.z);
        _point += intVec;
        StartCoroutine(Movement(dir));
    }
    #endregion

    #region PrivateMethod
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
    #endregion

    #region IEnumerator
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

    private IEnumerator Movement(Vector3 vec)
    {
        _isMoving = true;
        for (int i = 0; i < _moveSmoothness / _moveSpeed; i++)
        {
            transform.position += vec * (_moveSpeed / _moveSmoothness);
            yield return null;
        }
        _isMoving = false;
    }
    #endregion
}
