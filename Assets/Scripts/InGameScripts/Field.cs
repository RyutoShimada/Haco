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
    /// CellとなるGameObjectを登録
    /// </summary>
    /// <param name="x">座標</param>
    /// <param name="y">座標</param>
    /// <param name="obj">インスタンス化したオブジェクト</param>
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
        Debug.Log($"set player : _players[{x},{y}] = {_players[x, y]}  ({this})");
    }

    /// <summary>
    /// MAterialの変更
    /// </summary>
    /// <param name="x">座標</param>
    /// <param name="y">座標</param>
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
        Debug.Log($"remove player : _players[{x},{y}] = {_players[x, y]}  ({this})");
    }

    /// <summary>
    /// 移動できるか確認する
    /// </summary>
    /// <returns></returns>
    public bool CheckCanMove(int x, int y, PlayerData p)
    {
        int nextX = x + p.PointX;
        int nextY = y + p.PointY;

        Debug.Log($"({nextX}, {nextY}) is checking...  ({this})");

        if (nextX < -2 || nextX > 2)
        {
            Debug.Log("out of range");
            return false;
        }
        if (nextY < -2 || nextY > 2)
        {
            Debug.Log("out of range");
            return false;
        }

        //ConvertPositionToIndex(ref x, ref y);
        if (GetPlayer(nextX, nextY) != null)
        {
            Debug.Log("already exist");
            return false;
        }

        Debug.Log($"approval point : ({p.PointX}, {p.PointY}) -> ({nextX}, {nextY})  ({this})");
        SetPlayer(nextX, nextY, p);
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
