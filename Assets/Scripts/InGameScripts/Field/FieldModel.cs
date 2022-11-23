using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// フィールドの情報を持つクラス
/// </summary>
public class FieldModel
{
    private const byte FieldWidth = 5;
    private const byte FieldHeight = 5;
    private FieldController _controller;

    /// <summary>
    /// keyはマスの座標（-2, -2～2, 2）、valueは出目の効果
    /// </summary>
    internal Dictionary<PlayerModel, Vector2> _tilesData = new Dictionary<PlayerModel, Vector2>();

    public FieldModel(FieldController controller)
    {
        _controller = controller;
    }

    public void InitPawnPos(PlayerModel player, Vector3 pos)
    {
        _tilesData.Add(player, new Vector2(pos.x, pos.z));
        _controller.OnUpdate(_tilesData[player], player.State);
    }

    public void ChangePawnPos(PlayerModel player, Vector3 input)
    {
        _tilesData[player] += new Vector2(input.x, input.z);
        _controller.OnUpdate(_tilesData[player], player.State);
    }
}
