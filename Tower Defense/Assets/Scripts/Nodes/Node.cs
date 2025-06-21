using System;
using Managers;
using Tower_Related;
using UnityEngine;

namespace Nodes
{
    public class Node : MonoBehaviour
    {
        private BuildManager _buildManager;
        private NodeStyle _nodeStyle;
        private bool _isPlaced = false;
        public Tower_Related.Tower tower;

        private void Start()
        {
            _nodeStyle = GetComponent<NodeStyle>();
            _buildManager = Managers.BuildManager.GetInstance();
        }

        public void ClearNode()
        {
            _isPlaced = false;
            tower = null;

            if (_nodeStyle == null)
                _nodeStyle = GetComponent<NodeStyle>();

            if (_nodeStyle != null)
                _nodeStyle.SetFreeColor();
        }

        private void OnMouseDown()
        {
            _buildManager.SetNodeSelected(this);

            if (!_isPlaced)
            {
                TryBuildTower();
            }
            else
            {
                _buildManager.ToggleTowerDialogOn();
            }
        }

        private void TryBuildTower()
        {
            if (_buildManager.BuildTowerOn())
            {
                _isPlaced = true;
                if (_nodeStyle != null)
                    _nodeStyle.SetPlacedColor();
            }
        }

        public void PlaceTower()
        {
            _isPlaced = true;
            if (_nodeStyle == null)
                _nodeStyle = GetComponent<NodeStyle>();

            if (_nodeStyle != null)
                _nodeStyle.SetPlacedColor();
        }

        public void SetTower(Tower_Related.Tower t)
        {
            tower = t;
        }
    }
}
