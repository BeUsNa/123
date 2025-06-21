using System;
using Managers;
using Tower_Related;
using UnityEngine;

public class MouseItem : MonoBehaviour
{

    private Camera _mainCam;
    private GameObject _towerGO;

    private void Awake()
    {
        _mainCam = Camera.main;
    }

    private void Update()
    {
        if (_towerGO != null)
        {

            if (_mainCam == null)
                _mainCam = Camera.main;

            if (_mainCam == null)
                return;

            Vector2 mousePos = _mainCam.ScreenToWorldPoint(Input.mousePosition);
            _towerGO.transform.position = mousePos;

            if (Input.GetMouseButtonDown(1))
            {
                BuildManager.GetInstance().DeselectTowerToManage();
            }
        }
    }

    public void LockTowerToMouse(GameObject towerGO)
    {
        _towerGO = Instantiate(towerGO, transform.position, Quaternion.identity);
        _towerGO.GetComponent<Tower>().HoverMode(true); 
    }

    public void ReleaseTowerFromMouse()
    {
        if (_towerGO != null)
        {
            Destroy(_towerGO);
        }
    }
}
