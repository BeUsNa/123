using System;
using System.Collections.Generic;
using Enemy_Related;
using UnityEngine;

namespace Tower_Related {
    public class EnemyScanner : MonoBehaviour {
        [SerializeField] private float scanRange = 5f;

        private EnemyManager _enemyManager;
        private Transform _target;

        private void Awake() {
            _enemyManager = EnemyManager.GetInstance();
        }

        private void Start() {
            float radiusScale = scanRange * 2;
            transform.localScale = new Vector2(radiusScale, radiusScale);
        }

        public void ScanEnemiesInRange() {
            Transform targetEnemy = null;
            List<Enemy> enemies = _enemyManager.GetEnemiesList();

            foreach (Enemy enemy in enemies) {
                float currDistanceFromEnemy = Vector2.Distance(transform.position, enemy.transform.position);

                if (targetEnemy == null && currDistanceFromEnemy <= scanRange) {
                    targetEnemy = enemy.transform; 
                }
            }
            _target = targetEnemy; 
        }

        public bool IsTargetFound() {
            return _target != null;
        }

        public Transform GetTarget() {
            return _target;
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, scanRange);
        }
    }
}