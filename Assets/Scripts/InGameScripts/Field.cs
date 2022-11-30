using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyPlayer;

public class Field : MonoBehaviour
{
    public class PointData
    {
        public int PointX { get; private set; }
        public int PointZ { get; private set; }
        public Material Material { get; private set; }

        public PlayerInfo Player { get; private set; }


        public PointData(int x, int z, Material m)
        {
            PointX = x;
            PointZ = z;
            Material = m;
        }

        /// <summary>
        /// オブジェクトを設定する
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>設定に成功したら true を返す</returns>
        public bool SetObject(PlayerInfo obj)
        {
            if (obj == null)
            {
                Player = obj;
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
    }

    private const byte FieldWidth = 5;
    private const byte FieldHeight = 5;

    [SerializeField]
    private GameObject _cellPrefab;

    private PointData[,] _cells = new PointData[FieldWidth, FieldHeight];

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

    private void InitSetCells()
    {
        for (int x = -2; x < FieldWidth - 2; x++)
        {
            for (int z = -2; z < FieldHeight - 2; z++)
            {
                var obj = Instantiate(_cellPrefab, new Vector3(x, 0, z), _cellPrefab.transform.rotation, transform);
                var m = obj.GetComponent<Renderer>().material;
                _cells[x + 2, z + 2] = new PointData(x, z, m);
            }
        }
    }

    public void SetPlayer(int indexX, int indexY, PlayerInfo player)
    {
        _cells[indexX, indexY].SetObject(player);
        _cells[indexX, indexY].ChangeMaterialColor(Color.red);
    }

    public bool MoveTo(int x, int z, Vector3 dir)
    {
        int posX = x;
        int posZ = z;
        int dirX = (int)dir.x;
        int dirZ = (int)dir.z;

        int nextX = posX + dirX;
        int nextZ = posZ + dirZ;

        //Debug.Log($"Point({posX}, {posZ}) -> Point({nextX}, {nextZ})");

        if (!CheckOutOfRange(nextX, nextZ)) 
        {
            //Debug.Log("範囲外");
            return false; 
        }

        //Debug.Log("範囲内");

        ConvertPositionToIndex(ref nextX, ref nextZ);
        ConvertPositionToIndex(ref posX, ref posZ);

        if (_cells[nextX, nextZ].SetObject(_cells[posX, posZ].Player))
        {
            _cells[nextX, nextZ].ChangeMaterialColor(Color.red);
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log($"_cell[{nextX}, {nextZ}] は既にオブジェクトが入っています。");
            return false;
#endif
        }

        _cells[posX, posZ].SetObject(null);
        _cells[posX, posZ].ChangeMaterialColor(Color.white);
        return true;
    }

    private void ConvertPositionToIndex(ref int x, ref int y)
    {
        x += 2;
        y += 2;
    }

    private bool CheckOutOfRange(int x, int z)
    {
        if (x < -2 || x > 2)
        {
            return false;
        }
        if (z < -2 || z > 2)
        {
            return false;
        }

        return true;
    }
}
