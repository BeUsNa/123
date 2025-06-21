using System.Collections;
using System.Collections.Generic;
using Enemy_Related;
using Scenes;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Wave_Spawning;
using System.Reflection;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;
        public static GameManager GetInstance() => _instance;

        [Header("Player Stats")]
        [SerializeField] private int maxHealth = 15;
        [SerializeField] private int money;

        private int _health;
        private int _money;

        private DisplayUI _displayUI;
        private EnemyManager _enemyManager;
        private WaveSpawner _waveSpawner;
        private DialogManager _dialogManager;
        private float _gameSpeed;

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
                return;
            }

            _enemyManager = EnemyManager.GetInstance();
            _waveSpawner = FindObjectOfType<WaveSpawner>();
            _dialogManager = FindObjectOfType<DialogManager>();
            _displayUI = GetComponent<DisplayUI>();
        }

        private void OnEnable()
        {
            if (_enemyManager != null)
            {
                _enemyManager.OnEnemyKilledEvent += AddToMoney;
                _enemyManager.OnAllEnemiesDeadEvent += WaveFinished;
            }

            if (_waveSpawner != null)
            {
                _waveSpawner.OnWaveComplete += WaveFinished;
                _waveSpawner.OnAllWavesComplete += WinLevel;
            }
        }

        private void OnDisable()
        {
            if (_enemyManager != null)
            {
                _enemyManager.OnEnemyKilledEvent -= AddToMoney;
                _enemyManager.OnAllEnemiesDeadEvent -= WaveFinished;
            }

            if (_waveSpawner != null)
            {
                _waveSpawner.OnWaveComplete -= WaveFinished;
                _waveSpawner.OnAllWavesComplete -= WinLevel;
            }
        }

        private void Start()
        {
            _gameSpeed = 1f;
            _money = money;
            _health = maxHealth;

            if (SaveSystem.shouldLoadFromSave && SaveSystem.SaveExists())
            {
                LoadGameState();
            }

            SetupUI();
        }


        private void SetupUI()
        {
            _displayUI.UpdateHealthUI(_health);
            _displayUI.UpdateMoneyUI(_money);
            _displayUI.UpdateMaxWaveCountUI(_waveSpawner.GetMaxWaveCount(), _waveSpawner.GetCurrentWaveIndex());
        }

        private void Update()
        {
            ManageKeyboardInput();
        }

        private void ManageKeyboardInput()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                OnPlayButtonPress();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnPausePress();
            }
        }

        public void OnPausePress() => _dialogManager.TogglePauseLabel(_gameSpeed);
        public void OnPlayButtonPress() => _waveSpawner.SpawnCurrentWave();

        public void TakeDamage(int dmg)
        {
            _health -= dmg;
            if (_health <= 0)
            {
                _health = 0;
                AudioManager.GetInstance().PlayPlayerDeathSfx();
                LooseLevel();
            }

            _displayUI.UpdateHealthUI(_health);
            AudioManager.GetInstance().PlayPlayerHitSfx();
        }

        public void AddToMoney(int amount)
        {
            _money += amount;
            _displayUI.UpdateMoneyUI(_money);
        }

        public bool SpendMoney(int amount)
        {
            if (amount > _money) return false;
            _money -= amount;
            _displayUI.UpdateMoneyUI(_money);
            return true;
        }

        public int GetCurrentMoney() => _money;
        public int GetCurrentHealth() => _health;

        public void SetCurrentWaveIndex(int currWaveIndex)
        {
            _displayUI.UpdateCurrentWaveCountUI(currWaveIndex);
        }

        public void NextLevel()
        {
            CleanLevel();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            Destroy(gameObject);
        }

        public void ExitToMainMenu()
        {
            SaveGameState();
            FindObjectOfType<LevelLoader>().LoadMainMenuScene();
            Destroy(gameObject);
        }

        public void RestartLevel()
        {
            FindObjectOfType<LevelLoader>().LoadCurrentScene();
            CleanLevel();
            Destroy(gameObject);
        }

        public void SetGameSpeed(float speed)
        {
            _gameSpeed = speed;
            Time.timeScale = speed;
        }

        private void WaveFinished()
        {
            if (_waveSpawner.GetCurrentState() == SpawnState.Waiting && _enemyManager.GetEnemyCount() == 0)
            {
                ActivateWaveFinishDialog();
            }
        }

        private void ActivateWaveFinishDialog()
        {
            AddToMoney(150);
            _dialogManager.ActivateWaveFinishLabel();
        }

        private void WinLevel()
        {
            _gameSpeed = 0;
            SetGameSpeed(_gameSpeed);
            UnlockNextLevel();
            _dialogManager.ActivateWinLabel();
        }

        private void UnlockNextLevel()
        {
            int currLvl = Preferences.GetCurrentLvl();
            Preferences.SetMaxLvl(currLvl + 1);
        }

        private void LooseLevel()
        {
            _gameSpeed = 0;
            SetGameSpeed(_gameSpeed);
            _dialogManager.ActivateLooseLabel();
        }

        private void CleanLevel()
        {
            _health = maxHealth;
            _money = money;
            _enemyManager.ClearEnemiesList();
            BuildManager.GetInstance().ClearMemory();
            _gameSpeed = 1f;
            SetGameSpeed(_gameSpeed);
        }

        public void SaveGameState()
        {
            string sceneName = SceneManager.GetActiveScene().name;
            int wave = _waveSpawner.GetCurrentWaveIndex();

            var towers = FindObjectsOfType<Tower_Related.Tower>();
            List<TowerData> towerList = new List<TowerData>();

            foreach (var t in towers)
            {
                if (t.transform.parent == null) continue;
                towerList.Add(new TowerData
                {
                    type = t.gameObject.name.Replace("(Clone)", ""),
                    position = t.transform.parent.position,
                    level = t.Level
                });
            }

            SaveData save = new SaveData
            {
                sceneName = sceneName,
                wave = wave,
                money = _money,
                lives = _health,
                towers = towerList
            };

            SaveSystem.SaveGame(save);
            Debug.Log($"[SAVE] Сохранено: сцена={sceneName}, волна={wave}, деньги={_money}, жизни={_health}, башни={towerList.Count}");
        }

        public void LoadGameState()
        {
            SaveData data = SaveSystem.LoadGame();
            if (data == null) return;

            _health = data.lives;
            _money = data.money;
            _waveSpawner.SetCurrentWaveIndex(data.wave);
            _displayUI.UpdateCurrentWaveCountUI(data.wave);
            SetupUI();

            var nodes = GameObject.FindObjectsOfType<Nodes.Node>();

            foreach (var node in nodes)
            {
                var existingTower = node.GetComponentInChildren<Tower_Related.Tower>();
                if (existingTower != null)
                    Destroy(existingTower.gameObject);

                node.ClearNode();
            }

            foreach (var td in data.towers)
            {
                GameObject prefab = Resources.Load<GameObject>("Towers/" + td.type);
                if (prefab == null) continue;

                Nodes.Node targetNode = null;
                foreach (var node in nodes)
                {
                    if (Vector2.Distance(node.transform.position, td.position) < 0.6f)
                    {
                        targetNode = node;
                        break;
                    }
                }

                if (targetNode == null) continue;

                Vector3 spawnPos = targetNode.transform.position + new Vector3(0, 0.5f, 0);
                GameObject towerGO = GameObject.Instantiate(prefab, spawnPos, Quaternion.identity);
                towerGO.transform.parent = targetNode.transform;

                var towerScript = towerGO.GetComponent<Tower_Related.Tower>();
                if (towerScript != null)
                {
                    towerScript.Level = td.level;
                    towerScript.HoverMode(false);
                }

                targetNode.SetTower(towerScript);
                targetNode.PlaceTower();
            }

            StartCoroutine(DelayedCanvasAssignment());
            Debug.Log($"[LOAD] Восстановлено: сцена={data.sceneName}, волна={data.wave}, башен={data.towers.Count}");
        }


        private IEnumerator DelayedCanvasAssignment()
        {
            GameObject canvas = null;
            while (canvas == null)
            {
                canvas = GameObject.Find("NodeUI");
                yield return null;
            }

            var buildManager = BuildManager.GetInstance();
            if (buildManager != null)
            {
                buildManager.SetSellCanvas(canvas);
                Debug.Log(" SellCanvas надёжно установлен через ожидание появления.");
            }
            else
            {
                Debug.LogWarning(" BuildManager всё ещё null.");
            }
        }

    }
}