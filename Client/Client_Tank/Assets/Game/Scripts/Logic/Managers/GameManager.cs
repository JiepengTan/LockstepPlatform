using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Collections;
using System;
using Lockstep.Core;
using Lockstep.Math;
using Debug = Lockstep.Logging.Debug;
using Random = UnityEngine.Random;


namespace Lockstep.Game {
    public enum EGameResult {
        Playing,
        Failed,
        Win,
        PartFinished,
    }
    [System.Serializable]
    public partial class GameManager : SingletonManager<GameManager>, IUnitService {
        [Header("Transforms")] [HideInInspector]
        public Transform transParentPlayer;

        [HideInInspector] public Transform transParentEnemy;
        [HideInInspector] public Transform transParentItem;
        [HideInInspector] public Transform transParentBullet;

        public int CurLevel = 0;
        public int MAX_LEVEL_COUNT = 2;


        //大本营

        //const variables

        #region LifeCycle

        public override void DoAwake(IServiceContainer services){
            CurLevel = PlayerPrefs.GetInt("GameLevel", 0);
            Func<string, Transform> FuncCreateTrans = (name) => {
                var go = new GameObject(name);
                go.transform.SetParent(transform, false);
                return go.transform;
            };
            transParentPlayer = FuncCreateTrans("Players");
            transParentEnemy = FuncCreateTrans("Enemies");
            transParentItem = FuncCreateTrans("Items");
            transParentBullet = FuncCreateTrans("Bullets");
            _config = Resources.Load<GameConfig>(GameConfig.ConfigPath);
        }

        public void OnEvent_LevelLoadDone(object param){
            var level = (int) param;
            IsGameOver = false;
            _constStateService.curLevel = level;
        }

        private GameConfig _config;


        #region IUnitService

        public void TakeDamage(GameEntity bullet, GameEntity suffer){
            if (suffer.isDestroyed) return;
            if (suffer.unit.health <= bullet.unit.damage) {
                bullet.unit.health -= suffer.unit.health;
                suffer.unit.health = 0;
                suffer.unit.killerLocalId = bullet.bullet.ownerLocalId;
                suffer.isDestroyed = true;
            }
            else {
                suffer.unit.health -= bullet.unit.damage;
                bullet.unit.health = 0;
                bullet.isDestroyed = true;
            }
        }

        public void DropItem(LFloat rate){
            if (_randomService.value >= rate) {
                return;
            }
            var min = _constStateService.mapMin;
            var max = _constStateService.mapMax;
            var x = _randomService.Range(min.x + 4, max.x - 4);
            var y = _randomService.Range(min.y + 4, max.y - 4);
            CreateItem(new LVector2(x, y), _randomService.Range(0, _config.itemPrefabs.Count));
        }

        private void CreateItem(LVector2 createPos, int type){
            CreateUnit(createPos, _config.itemPrefabs, type, EDir.Up, transParentItem);
        }

        public void CreateCamp(LVector2 createPos, int type = 0){
            CreateUnit(createPos + GameConfig.TankBornOffset, _config.CampPrefabs, 0, EDir.Up, transParentItem);
        }

        public void CreateBullet(LVector2 pos, EDir dir, int type, GameEntity owner){
            var createPos = pos + DirUtil.GetDirLVec(dir) * TankUtil.TANK_HALF_LEN;
            
            var entity = CreateUnit(createPos, _config.bulletPrefabs, type, dir, transParentBullet);
            entity.bullet.ownerLocalId = owner.localId.value;
            entity.unit.camp = owner.unit.camp;
        }

        public void CreateEnemy(LVector2 bornPos){
            var type = _randomService.Range(0, _config.enemyPrefabs.Count);
            CreateEnemy(bornPos, type);
        }

        public void CreateEnemy(LVector2 bornPos, int type){
            var createPos = bornPos + LVector2.right;
            _resourceService.ShowBornEffect(createPos);
            _audioService.PlayClipBorn();
            EDir dir = EDir.Down;
            DelayCall(GameConfig.TankBornDelay,
                () => {
                    CreateUnit(createPos, _config.enemyPrefabs, type, dir, transParentEnemy);
                });
        }

        public void CreatePlayer(byte actorId, int type){
            var bornPos = _constStateService.playerBornPoss[actorId%_constStateService.playerBornPoss.Count];
            var createPos = bornPos + GameConfig.TankBornOffset;
            _resourceService.ShowBornEffect(createPos);
            _audioService.PlayClipBorn();
            EDir dir = EDir.Up;
            DelayCall(GameConfig.TankBornDelay, () => {
                var entity = CreateUnit(createPos, _config.playerPrefabs, type, dir, transParentPlayer);
                var actor = _actorContext.GetEntityWithId(actorId);
                if (actor != null) {
                    actor.ReplaceGameLocalId(entity.localId.value);
                    entity.ReplaceActorId(actorId);
                }
                else {
                    Debug.LogError(
                        $"can not find a actor after create a game player actorId:{actorId} localId{entity.localId.value}");
                }
            });
        }


        private GameEntity CreateUnit<T>(LVector2 createPos, List<T> prefabLst, int type, EDir dir, Transform parent)
            where T : GameConfig.Unit{
            var ecsPrefab = prefabLst[type];
            var assetId = ecsPrefab.asset.assetId;
            var prefab = Resources.Load<GameObject>(GameConfig.GetAssetPath(assetId));
            var entity = CreateGameEntity();
            if (!_constStateService.IsVideoLoading) {
                var go = GameObject.Instantiate(prefab, transform.position + createPos.ToVector3(),
                    Quaternion.identity, parent);
                go.AddComponent<PosListener>();
                go.AddComponent<DirListener>();
                _viewService.BindView(entity, _gameContext, go);
            }

            ecsPrefab.SetComponentsTo(entity);
            entity.dir.value = dir;
            entity.pos.value = createPos;
            return entity;
        }

        ///用于惟一标记 GameEntity 用于回滚
        private uint _localIdCounter;

        private GameEntity CreateGameEntity(){
            var entity = _gameContext.CreateEntity();
            entity.AddLocalId(_localIdCounter);
            _localIdCounter++;
            return entity;
        }

        public void Upgrade(GameEntity entity){
            var playerCount = _config.playerPrefabs.Count;
            var targetType = entity.unit.detailType +1;
            if (targetType >= playerCount) {
                UnityEngine.Debug.Log($"hehe already max level can not upgrade");
                return;
            }

            var ecsPrefab = _config.playerPrefabs[targetType];
            var rawPos = entity.pos.value;
            var rawDir = entity.dir.value;
            ecsPrefab.SetComponentsTo(entity);
            entity.pos.value = rawPos;
            entity.dir.value = rawDir;
            if (!_constStateService.IsVideoLoading) {
                _viewService.DeleteView(entity.localId.value);
                var prefab = Resources.Load<GameObject>(GameConfig.GetAssetPath(ecsPrefab.asset.assetId));
                var go = GameObject.Instantiate(prefab, transform.position + rawPos.ToVector3(),
                    Quaternion.Euler(0,0,DirUtil.GetDirDeg(rawDir)), transParentPlayer);
                go.AddComponent<PosListener>();
                go.AddComponent<DirListener>();
                _viewService.BindView(entity, _gameContext, go);
            }

        }
        public void DelayCall(LFloat delay, Action callback){
            var delayEntity = CreateGameEntity();
            delayEntity.AddDelayCall(delay, callback);
        }

        #endregion

        public bool IsPlaying = false;
        public bool IsGameOver;

        public override void DoUpdate(float deltaTime){
            if (IsGameOver) return;
        }

        public void OnEvent_SimulationStart(object param){
            IsPlaying = true;
        }
        #endregion


        #region GameStatus

        private void GameFalied(){
            IsGameOver = true;
            ShowMessage("Game Falied!!");
            Clear();
        }

        private void GameWin(){
            IsGameOver = true;
            if (CurLevel >= MAX_LEVEL_COUNT) {
                ShowMessage("You Win!!");
            }
            else {
                Clear();
                MapManager.Instance.LoadLevel(CurLevel + 1);
            }
        }


        private void ShowMessage(string str){ }

        private void Clear(){ }

        #endregion
    }
}