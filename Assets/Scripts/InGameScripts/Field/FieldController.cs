using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldController : MonoBehaviour
{
    public static FieldController Instance { get; private set; }

    public FieldModel _fieldModel;
    [SerializeField]
    PlayerController _playerController;

    private void Awake()
    {
        _fieldModel = new FieldModel(this);
        Instance = this;
    }

    void Start()
    {
        _fieldModel.InitPawnPos(_playerController._playerModel, _playerController.gameObject.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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

    public void ChangePos(Vector3 input, PlayerModel player)
    {
        _fieldModel.ChangePawnPos(player, input);
    }
}
