using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// フィールドの情報を持つクラス
/// </summary>
public class FieldModel : MonoBehaviour
{
    [SerializeField]
    private GameObject _tilePrefab = default;
    [SerializeField]
    private Material _wingTileMaterial = default;

    private GameObject[,] _tiles = new GameObject[5, 5];

    private void Start()
    {
        CreateField();
    }

    private void CreateField()
    {
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                _tiles[x,y] = Instantiate(_tilePrefab, new Vector3(x - 2, 0, y - 2), _tilePrefab.gameObject.transform.rotation, transform);
            }
        }

        _tiles[2,2].GetComponent<Renderer>().material = _wingTileMaterial;
    }
}
