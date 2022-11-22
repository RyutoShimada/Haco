using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    [SerializeField]
    private GameObject _modelPrefab = default;
    [SerializeField]
    private Vector3 _instancePos = default;

    private void Start()
    {
        CreateModel();
    }

    private void CreateModel()
    {
        Instantiate(_modelPrefab, _instancePos, transform.rotation, transform);
    }
}
