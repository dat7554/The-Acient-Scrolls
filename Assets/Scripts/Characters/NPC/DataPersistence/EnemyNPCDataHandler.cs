using Characters.FSM.EnemyFSM;
using Characters.NPC.Enemy;
using SaveLoadSystem;
using SaveLoadSystem.Interfaces;
using UnityEngine;

namespace Characters.NPC.DataPersistence
{
    public class EnemyNPCDataHandler : MonoBehaviour, IDataPersistence
    {
        private string _id;
        private EnemyNPC _enemyNpc;
        private EnemyStateMachine _stateMachine;
        private EnemyNPCStats _enemyNpcStats;
        private EnemyNPCData _enemyNpcData;

        private void Awake()
        {
            _enemyNpc = GetComponent<EnemyNPC>();
            _stateMachine = GetComponent<EnemyStateMachine>();
            _enemyNpcStats = GetComponent<EnemyNPCStats>();
        }

        private void Start()
        {
            _id = _enemyNpc.uniqueIDGenerator.ID;
        }
        
        private void OnEnable()
        {
            SaveGameManager.Instance.OnGameDataLoaded += LoadData;
        }

        private void OnDisable()
        {
            SaveGameManager.Instance.OnGameDataLoaded -= LoadData;
        }
        
        public void SaveData(GameData gameData)
        {
            _enemyNpcData = new EnemyNPCData
                (
                    _enemyNpc.transform.position,
                    _enemyNpcStats.CurrentHealth,
                    _enemyNpcStats.MaxHealth
                );

            gameData.enemyDataDictionary[_id] = _enemyNpcData;
        }

        public void LoadData(GameData gameData)
        {
            _id = _enemyNpc.uniqueIDGenerator.ID;
            if (gameData.enemyDataDictionary.TryGetValue(_id, out var enemyNpcData))
            {
                _enemyNpc.agent.Warp(enemyNpcData.position);
                _enemyNpcStats.LoadData(enemyNpcData.currentHealth, enemyNpcData.maxHealth);
                
                _stateMachine.TransitionToState(EnemyStateMachine.EnemyStateID.Idle);
            }
        }
    }
}
