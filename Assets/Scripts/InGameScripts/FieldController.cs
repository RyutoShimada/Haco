using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldController : MonoBehaviour
{
    public static FieldController Instance { get; private set; }

    [SerializeField]
    PlayerController _playerController;

    [SerializeField]
    private GameObject _cellPrefab;
    /// <summary>
    /// 中央の加速マス
    /// </summary>
    [SerializeField]
    private Material material;

    private Field _field;

    private const byte FieldWidth = 5;
    private const byte FieldHeight = 5;
    private FieldController _controller;

    /// <summary>
    /// keyはマスの座標（-2, -2～2, 2）、valueは出目の効果
    /// </summary>
    //internal Dictionary<PlayerModel, Vector2> _tilesData = new Dictionary<PlayerModel, Vector2>();

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        //InitPawnPos(_playerController._playerModel, _playerController.gameObject.transform.position);
        _field = new Field(FieldWidth, FieldHeight);
        for (int x = -2; x < FieldWidth - 2; x++)
        {
            for (int y = -2; y < FieldHeight - 2; y++)
            {
                 var obj = Instantiate(_cellPrefab, new Vector3(x, 0, y), _cellPrefab.transform.rotation, transform);
                _field.SetCell(x, y, obj);
            }
        }

        _field.SetMaterial(0, 0, material);
        _playerController.CanMove += _field.CheckCanMove;
        Player.PlayerData lData = _playerController.LeftData;
        Player.PlayerData rData = _playerController.RightData;
        _field.SetPlayer(lData.PointX, lData.PointY, lData);
        _field.SetPlayer(rData.PointX, rData.PointY, rData);
    }

    private void OnDisable()
    {
        _playerController.CanMove -= _field.CheckCanMove;
    }
}
