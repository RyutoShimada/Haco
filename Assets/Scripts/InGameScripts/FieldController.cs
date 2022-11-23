using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldController : MonoBehaviour
{
    public static FieldController Instance { get; private set; }

    [SerializeField]
    PlayerController _playerController;

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //public void InitPawnPos(PlayerModel player, Vector3 pos)
    //{
    //    _tilesData.Add(player, new Vector2(pos.x, pos.z));
    //    _controller.OnUpdate(_tilesData[player], player.State);
    //}

    //public void ChangePawnPos(PlayerModel player, Vector3 input)
    //{
    //    _tilesData[player] += new Vector2(input.x, input.z);
    //    _controller.OnUpdate(_tilesData[player], player.State);
    //}

    public void OnUpdate(Vector2 tilePos, PawnState state)
    {
        Debug.Log($"値が更新されました。-> [x : {tilePos.x}, z : {tilePos.y}] [{state}]");
    }

    public bool CanMove(Vector3 pos, Vector3 input)
    {
        var x = pos.x + input.x;
        var z = pos.z + input.z;
        x = Mathf.CeilToInt(x);
        z = Mathf.CeilToInt(z);

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

    //public void ChangePos(Vector3 input, PlayerModel player)
    //{
    //    ChangePawnPos(player, input);
    //}
}
