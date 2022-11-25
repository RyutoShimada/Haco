using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
public class Field
{
    private GameObject[,] _cells;
    private PlayerData[,] _players;

    public Field(int width, int height)
    {
        _cells = new GameObject[width, height];
        _players = new PlayerData[width, height];
    }

    /// <summary>
    /// Cell�ƂȂ�GameObject��o�^
    /// </summary>
    /// <param name="x">���W</param>
    /// <param name="y">���W</param>
    /// <param name="obj">�C���X�^���X�������I�u�W�F�N�g</param>
    public void SetCell(int x, int y, GameObject obj)
    {
        ConvertPositionToIndex(ref x,  ref y);
        _cells[x, y] = obj;
    }

    public PlayerData GetPlayer(int x, int y)
    {
        ConvertPositionToIndex(ref x, ref y);
        return _players[x, y];
    }

    public void SetPlayer(int x, int y, PlayerData p)
    {
        ConvertPositionToIndex(ref x, ref y);
        _players[x, y] = p;
    }

    /// <summary>
    /// MAterial�̕ύX
    /// </summary>
    /// <param name="x">���W</param>
    /// <param name="y">���W</param>
    /// <param name="m">Material</param>
    public void SetMaterial(int x, int y, Material m)
    {
        ConvertPositionToIndex(ref x, ref y);
        _cells[x, y].GetComponent<Renderer>().material = m;
    }

    private void RemovePlayer(int x, int y)
    {
        ConvertPositionToIndex(ref x, ref y);
        _players[x, y] = null;
    }

    /// <summary>
    /// �ړ��ł��邩�m�F����
    /// </summary>
    /// <returns></returns>
    public bool CheckCanMove(Vector2 input, PlayerData p)
    {
        int x = (int)input.x + p.PointX;
        int y = (int)input.y + p.PointY;

        if (x < -2 || x > 2)
        {
            return false;
        }
        if (y < -2 || y > 2)
        {
            return false;
        }

        //ConvertPositionToIndex(ref x, ref y);
        if (GetPlayer(x, y) != null)
        {
            return false;
        }

        //Debug.Log($"point : ({p.PointX}, {p.PointY}) -> ({newX}, {newY})");
        
        SetPlayer(x, y, p);
        RemovePlayer(p.PointX, p.PointY);
        
        return true;
    }

    private void ConvertPositionToIndex(ref int x, ref int y)
    {
        x += 2;
        y += 2;
    }

    private void ConvertIndexToPosition(ref int x, ref int y)
    {
        x -= 2;
        y -= 2;
    }
}
