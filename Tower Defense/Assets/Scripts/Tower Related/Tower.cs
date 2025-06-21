using System;
using UnityEngine;

namespace Tower_Related
{
    public class Tower : MonoBehaviour
    {

        [SerializeField] private int cost = 100;
        [SerializeField] private int level = 1;
        [SerializeField] private string russianName = "Башня";

        public int Level
        {
            get => level;
            set => level = value;
        }

        public string GetRussianName() => russianName;

        private EnemyScanner _enemyScanner;
        private Weapon _weapon;
        private bool _isHover = false;

        private void Awake()
        {
            _enemyScanner = GetComponentInChildren<EnemyScanner>();
            _weapon = GetComponent<Weapon>();
        }

        private void Start()
        {
            InvokeRepeating("ScanEnemies", 0f, 0.1f);
        }

        private void Update()
        {
            if (!_isHover && _enemyScanner.IsTargetFound())
            {
                _weapon.Shoot(_enemyScanner.GetTarget());
            }
        }

        private void ScanEnemies()
        {
            _enemyScanner.ScanEnemiesInRange();
        }

        public void RemoveTower()
        {
            Destroy(gameObject);
        }

        public int GetCost()
        {
            return cost;
        }

        public void HoverMode(bool hover)
        {
            _isHover = hover;
        }
    }
}
