using System;
using Nodes;
using Tower_Related;
using UnityEngine;

namespace Managers
{
    public class BuildManager : MonoBehaviour
    {
        private static BuildManager _instance;
        public static BuildManager GetInstance() => _instance;

        [SerializeField] private GameObject sellCanvas;
        [SerializeField] private bool rememberSelection;

        private Node _nodeSelected;
        private GameObject _towerToBuild;
        private float _offset = 0.5f;
        private bool _isSelected = false;

        private MouseItem _mouseItem;
        private GameManager Game => GameManager.GetInstance();

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            if (sellCanvas != null)
                sellCanvas.SetActive(false);

            _mouseItem = GetComponent<MouseItem>();
        }

        public void SetNodeSelected(Node node)
        {
            _nodeSelected = node;
            Debug.Log("Node selected: " + node.name);
        }

        public void ToggleTowerDialogOn()
        {
            _isSelected = !_isSelected;
            Debug.Log("TOGGLE DIALOG: " + _isSelected);

            if (_isSelected)
            {
                if (sellCanvas != null && _nodeSelected != null)
                {
                    Debug.Log("ACTIVATING SELL UI FOR NODE: " + _nodeSelected.name);
                    Vector2 nodePosition = _nodeSelected.transform.position;
                    nodePosition.y += 1.5f;
                    sellCanvas.transform.position = nodePosition;
                    sellCanvas.SetActive(true);
                }
                else
                {
                    Debug.LogWarning("SellCanvas or NodeSelected is NULL!");
                }
            }
            else
            {
                if (sellCanvas != null)
                    sellCanvas.SetActive(false);
            }
        }

        public void SetSellCanvas(GameObject canvas)
        {
            sellCanvas = canvas;
            Debug.Log("SellCanvas set via code: " + (sellCanvas != null));
        }

        public void CancelBuildMode()
        {
            _towerToBuild = null;
            _mouseItem.ReleaseTowerFromMouse();
        }

        public void DeselectTowerToManage()
        {
            _mouseItem.ReleaseTowerFromMouse();
            _towerToBuild = null;
        }

        public void SetTowerToManage(GameObject towerGO)
        {
            _mouseItem.ReleaseTowerFromMouse();
            _towerToBuild = towerGO;
            _mouseItem.LockTowerToMouse(towerGO);
            HideTowerCanvas();
        }

        public bool BuildTowerOn()
        {
            if (_towerToBuild != null)
            {
                int towerCost = _towerToBuild.GetComponent<Tower>().GetCost();
                if (Game.SpendMoney(towerCost))
                {
                    SpawnTower();
                    AudioManager.GetInstance().PlayTowerDownSfx();
                    return true;
                }
            }
            return false;
        }

        public void SellTowerOn()
        {
            Tower currTower = _nodeSelected.GetComponentInChildren<Tower>();
            if (currTower != null)
            {
                Game.AddToMoney((int)(currTower.GetCost() / 1.5f));
                currTower.RemoveTower();
                _towerToBuild = null;
                _nodeSelected.ClearNode();
                AudioManager.GetInstance().PlayTowerSoldSfx();
            }
            HideTowerCanvas();
        }

        private void HideTowerCanvas()
        {
            if (sellCanvas != null)
                sellCanvas.SetActive(false);
            _isSelected = false;
        }

        private void SpawnTower()
        {
            _towerToBuild.GetComponent<Tower>().HoverMode(false);
            _mouseItem.ReleaseTowerFromMouse();
            Vector2 wantedPos = _nodeSelected.transform.position;
            wantedPos.y += _offset;
            GameObject towerGO = Instantiate(_towerToBuild, wantedPos, Quaternion.identity);
            towerGO.transform.parent = _nodeSelected.transform;

            if (!rememberSelection)
                _towerToBuild = null;

            Tower towerScript = towerGO.GetComponent<Tower>();
            _nodeSelected.SetTower(towerScript);
            _nodeSelected.PlaceTower();
        }

        public void ClearMemory() => Destroy(gameObject);
        public void ClearAllPlacedTowers()
        {
            var towers = GameObject.FindObjectsOfType<Tower>();
            foreach (var tower in towers)
                Destroy(tower.gameObject);
        }

        public void ResetSelectedTower() => _towerToBuild = null;
    }
}
