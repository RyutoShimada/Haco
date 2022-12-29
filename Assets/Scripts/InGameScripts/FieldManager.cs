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
        /// �I�u�W�F�N�g��ݒ肷��
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>�ݒ�ɐ��������� true ��Ԃ�</returns>
        public bool SetObject(PlayerInfo obj)
        {
            // null ����ꂽ����  
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
    /// �U����
    /// </summary>
    private PointData _attacker;
    /// <summary>
    /// �U�����󂯂鑤
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
        _traces.Push(_cells[currentIndex.x, currentIndex.z]); // �ړ��̗�����o�^
    }


    public bool CanMove(Vector3Int pos, Vector3Int dir)
    {
        var nextPos = pos + dir;
        // �ړ��悪�t�B�[���h���ł��邩�ǂ���
        if (!CheckOutOfRange(nextPos)) { return false; }

        var nextIndex = ConvertPositionToIndex(nextPos);

        // �ړ���ɃR�}�����邩�ǂ���
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

        // �ړ����o�^
        _cells[nextIndex.x, nextIndex.z].SetObject(_cells[currentIndex.x, currentIndex.z].Player);
        _cells[currentIndex.x, currentIndex.z].SetObject(null);

        // �}�X�̐F�ύX
        _cells[nextIndex.x, nextIndex.z].ChangeMaterialColor(Color.red);
        _cells[currentIndex.x, currentIndex.z].ChangeMaterialColor(Color.green);

        _traces.Push(_cells[currentIndex.x, currentIndex.z]); // �ړ��̗�����o�^
    }

    public void UpdateData(Vector3Int pos, Vector3Int dir, bool traceMode)
    {
        var nextPos = pos + dir;
        var nextIndex = ConvertPositionToIndex(nextPos);
        var currentIndex = ConvertPositionToIndex(pos);

        // �ړ����o�^
        _cells[nextIndex.x, nextIndex.z].SetObject(_cells[currentIndex.x, currentIndex.z].Player);
        _cells[currentIndex.x, currentIndex.z].SetObject(null);

        // �}�X�̐F�ύX
        _cells[nextIndex.x, nextIndex.z].ChangeMaterialColor(Color.red);

        if (traceMode)
        {
            _cells[currentIndex.x, currentIndex.z].ChangeMaterialColor(Color.white);
        }
        else
        {
            _cells[currentIndex.x, currentIndex.z].ChangeMaterialColor(Color.green);
            _traces.Push(_cells[currentIndex.x, currentIndex.z]); // �ړ��̗�����o�^
        }
    }

    public bool SearchEnemysAround(Vector3Int pos, User user)
    {
        var currentIndex = ConvertPositionToIndex(pos);
        var x = currentIndex.x;
        var z = currentIndex.z;

        _attacker = _cells[x, z]; // �U�������m��

        // �㉺���E���������A�z��͈̔͊O����Ȃ������璲�ׂ�
        if (z + 1 < _cells.GetLength(1))
        {
            // �R�}�����݂��Ă��āA���̃R�}���G�������ꍇ�� true ��Ԃ�
            if (_cells[x, z + 1].Player != null && _cells[x, z + 1].Player.User != user)
            {
                _defender = _cells[x, z + 1]; // �U�����󂯂鑤���m�ۂ��Ă���
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
        // �_���[�W�v�Z
        _defender.Player.BeAttacked(BattleManager.JudgeTheBattle(_attacker.Player.PawnState, _defender.Player.PawnState));

        // ������΂�����
        Vector3Int attackerPos = _attacker.Point;
        Vector3Int defenderPos = _defender.Point;
        // ���ꂼ�ꂪ��ԕ������Z�o
        Vector3Int attackerMoveDir = attackerPos - defenderPos;
        Vector3Int defenderMoveDir = defenderPos - attackerPos;
        // ���ꂼ���ԕ����Ɉړ��ł��邩�m�F
        var attackerCanMove = CanMove(_attacker.Point, attackerMoveDir);
        var defenderCanMove = CanMove(_defender.Point, defenderMoveDir);

        // �����������ԂȂ�A���ꂼ��1�}�X����΂�
        if (attackerCanMove && defenderCanMove)
        {
            _attacker.Player.DoMove(attackerMoveDir);
            UpdateData(_attacker.Point, attackerMoveDir);
            _defender.Player.DoMove(defenderMoveDir);
            UpdateData(_defender.Point, defenderMoveDir);
        }
        else if (attackerCanMove)
        {
            // �h�q�����������A�U������������Ȃ�A�U������2�}�X�������邩�m�F����
            Debug.Log("�U�����͓����܂��B");
        }
        else if (defenderCanMove)
        {
            // �U�������������A�h�q����������Ȃ�A�h�q����2�}�X�������邩�m�F����
            Debug.Log("�h�q���͓����܂��B");
        }
        else
        {
            // ���������Ȃ��ꍇ
            Debug.Log("���������܂���B");
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
