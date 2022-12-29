using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyPlayer;

public class FieldManager : MonoBehaviour
{
    #region PointDataClass
    public class PointData
    {
        #region PublicField
        public Vector3Int Point { get; private set; }
        public Material Material { get; private set; }
        public PlayerInfo Player { get; private set; }
        #endregion

        #region PrivateField
        #endregion

        #region Constructor
        public PointData(Vector3Int pos, Material m)
        {
            Point = pos;
            Material = m;
        }
        #endregion

        #region PublicMethod
        /// <summary>
        /// オブジェクトを設定する
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>設定に成功したら true を返す</returns>
        public bool SetObject(PlayerInfo obj)
        {
            // null を入れたい時  
            if (obj == null)
            {
                Player = null;
                return true;
            }

            if (Player != null)
            {
                return false;
            }
            else
            {
                Player = obj;
                return true;
            }
        }

        public void ChangeMaterialColor(Color c)
        {
            Material.color = c;
        }

        public void ResetMaterialColors()
        {
            Material.color = Color.white;
        }
        #endregion
    }
    #endregion

    #region ConstField
    private const byte FieldWidth = 5;
    private const byte FieldHeight = 5;
    #endregion

    #region SerializeField
    [SerializeField]
    private GameObject _cellPrefab;
    #endregion

    #region PrivateField
    private PointData[,] _cells = new PointData[FieldWidth, FieldHeight];
    /// <summary>
    /// 攻撃側
    /// </summary>
    private PointData _attacker;
    /// <summary>
    /// 攻撃を受ける側
    /// </summary>
    private PointData _defender;
    private Stack<PointData> _traces = new Stack<PointData>();
    #endregion

    #region MonoBehaviour
    private void Awake()
    {
        InitSetCells();
    }

    void Start()
    {

    }

    void Update()
    {

    }
    #endregion

    #region PublicMethod
    public void SetPlayer(Vector3Int pos, PlayerInfo player)
    {
        var currentIndex = ConvertPositionToIndex(pos);
        _cells[currentIndex.x, currentIndex.z].SetObject(player);
        _cells[currentIndex.x, currentIndex.z].ChangeMaterialColor(Color.red);
        _traces.Push(_cells[currentIndex.x, currentIndex.z]); // 移動の履歴を登録
    }


    public bool CanMove(Vector3Int pos, Vector3Int dir)
    {
        var nextPos = pos + dir;
        // 移動先がフィールド内であるかどうか
        if (!CheckOutOfRange(nextPos)) { return false; }

        var nextIndex = ConvertPositionToIndex(nextPos);

        // 移動先にコマがあるかどうか
        if (_cells[nextIndex.x, nextIndex.z].Player != null)
        {
            return false;
        }

        return true;
    }

    public void UpdateData(Vector3Int pos, Vector3Int dir)
    {
        var nextPos = pos + dir;
        var nextIndex = ConvertPositionToIndex(nextPos);
        var currentIndex = ConvertPositionToIndex(pos);

        // 移動先を登録
        _cells[nextIndex.x, nextIndex.z].SetObject(_cells[currentIndex.x, currentIndex.z].Player);
        _cells[currentIndex.x, currentIndex.z].SetObject(null);

        // マスの色変更
        _cells[nextIndex.x, nextIndex.z].ChangeMaterialColor(Color.red);
        _cells[currentIndex.x, currentIndex.z].ChangeMaterialColor(Color.green);

        _traces.Push(_cells[currentIndex.x, currentIndex.z]); // 移動の履歴を登録
    }

    public void UpdateData(Vector3Int pos, Vector3Int dir, bool traceMode)
    {
        var nextPos = pos + dir;
        var nextIndex = ConvertPositionToIndex(nextPos);
        var currentIndex = ConvertPositionToIndex(pos);

        // 移動先を登録
        _cells[nextIndex.x, nextIndex.z].SetObject(_cells[currentIndex.x, currentIndex.z].Player);
        _cells[currentIndex.x, currentIndex.z].SetObject(null);

        // マスの色変更
        _cells[nextIndex.x, nextIndex.z].ChangeMaterialColor(Color.red);

        if (traceMode)
        {
            _cells[currentIndex.x, currentIndex.z].ChangeMaterialColor(Color.white);
        }
        else
        {
            _cells[currentIndex.x, currentIndex.z].ChangeMaterialColor(Color.green);
            _traces.Push(_cells[currentIndex.x, currentIndex.z]); // 移動の履歴を登録
        }
    }

    public bool SearchEnemysAround(Vector3Int pos, User user)
    {
        var currentIndex = ConvertPositionToIndex(pos);
        var x = currentIndex.x;
        var z = currentIndex.z;

        _attacker = _cells[x, z]; // 攻撃側を確保

        // 上下左右を見た時、配列の範囲外じゃなかったら調べる
        if (z + 1 < _cells.GetLength(1))
        {
            // コマが存在していて、そのコマが敵だった場合に true を返す
            if (_cells[x, z + 1].Player != null && _cells[x, z + 1].Player.User != user)
            {
                _defender = _cells[x, z + 1]; // 攻撃を受ける側を確保しておく
                return true;
            }
        }
        if (z - 1 >= 0)
        {
            if (_cells[x, z - 1].Player != null && _cells[x, z - 1].Player.User != user)
            {
                _defender = _cells[x, z - 1];
                return true;
            }
        }
        if (x + 1 < _cells.GetLength(0))
        {
            if (_cells[x + 1, z].Player != null && _cells[x + 1, z].Player.User != user)
            {
                _defender = _cells[x + 1, z];
                return true;
            }
        }
        if (x - 1 >= 0)
        {
            if (_cells[x - 1, z].Player != null && _cells[x - 1, z].Player.User != user)
            {
                _defender = _cells[x - 1, z];
                return true;
            }
        }

        return false;
    }

    public void DoBattle()
    {
        // ダメージ計算
        _defender.Player.BeAttacked(BattleManager.JudgeTheBattle(_attacker.Player.PawnState, _defender.Player.PawnState));

        // 吹っ飛ばし処理
        Vector3Int attackerPos = _attacker.Point;
        Vector3Int defenderPos = _defender.Point;
        // それぞれが飛ぶ方向を算出
        Vector3Int attackerMoveDir = attackerPos - defenderPos;
        Vector3Int defenderMoveDir = defenderPos - attackerPos;
        // それぞれ飛ぶ方向に移動できるか確認
        var attackerCanMove = CanMove(_attacker.Point, attackerMoveDir);
        var defenderCanMove = CanMove(_defender.Point, defenderMoveDir);

        // 両方動ける状態なら、それぞれ1マスずつ飛ばす
        if (attackerCanMove && defenderCanMove)
        {
            _attacker.Player.DoMove(attackerMoveDir);
            UpdateData(_attacker.Point, attackerMoveDir);
            _defender.Player.DoMove(defenderMoveDir);
            UpdateData(_defender.Point, defenderMoveDir);
        }
        else if (attackerCanMove)
        {
            // 防衛側が動けず、攻撃側が動けるなら、攻撃側を2マス動かせるか確認する
            Debug.Log("攻撃側は動けます。");
        }
        else if (defenderCanMove)
        {
            // 攻撃側が動けず、防衛側が動けるなら、防衛側を2マス動かせるか確認する
            Debug.Log("防衛側は動けます。");
        }
        else
        {
            // 両方動けない場合
            Debug.Log("両方動けません。");
        }

        ResetColors();
    }
    #endregion

    #region PrivateMethod
    private void InitSetCells()
    {
        for (int x = -2; x < FieldWidth - 2; x++)
        {
            for (int z = -2; z < FieldHeight - 2; z++)
            {
                var obj = Instantiate(_cellPrefab, new Vector3(x, 0, z), _cellPrefab.transform.rotation, transform);
                var m = obj.GetComponent<Renderer>().material;
                Vector3Int pos = new Vector3Int(x, 0, z);
                _cells[x + 2, z + 2] = new PointData(pos, m);
            }
        }
    }
    private Vector3Int ConvertPositionToIndex(Vector3Int pos)
    {
        var index = new Vector3Int(pos.x, pos.y, pos.z);
        index.x += 2;
        index.z += 2;
        return index;
    }

    private bool CheckOutOfRange(Vector3Int pos)
    {
        if (pos.x < -2 || pos.x > 2)
        {
            return false;
        }
        if (pos.z < -2 || pos.z > 2)
        {
            return false;
        }

        return true;
    }

    private void ResetColors()
    {
        foreach (var item in _traces)
        {
            item.ResetMaterialColors();
        }

        _traces.Clear();
    }
    #endregion
}
