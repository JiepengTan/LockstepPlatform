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
            _config = Resources.Load<GameConfig>(Define.ConfigPath);
        }

        public void OnEvent_LoadMapDone(object param){
            var level = (int) param;
            StartGame(level);
        }

        private GameConfig _config;


        #region IUnitService

        public void TakeDamage(GameEntity atker, GameEntity suffer){
            if(suffer.isDestroyed) return;
            if (suffer.unit.health <= atker.unit.damage) {
                suffer.unit.health = 0;
                suffer.unit.killerLocalId = atker.localId.value;
                suffer.isDestroyed = true;
            }
            else {
                suffer.unit.health -= atker.unit.damage;
            }
        }

        public void DropItem(LFloat rate){
            if (LRandom.value >= rate) {
                return;
            }

            var min = _globalStateService.mapMin;
            var max = _globalStateService.mapMax;
            var x = LRandom.Range(min.x + 1, max.x - 3);
            var y = LRandom.Range(min.y + 1, max.y - 3);
            CreateItem(new LVector2(x, y), LRandom.Range(0, _config.itemPrefabs.Count));
        }

        private void CreateItem(LVector2 createPos, int type){
            CreateUnit(createPos, _config.itemPrefabs, type, EDir.Up, transParentItem);
        }

        public void CreateCamp(LVector2 createPos, int type = 0){
            CreateUnit(createPos + Define.TankBornOffset, _config.CampPrefabs, 0, EDir.Up, transParentItem);
        }

        public void CreateBullet(LVector2 pos, EDir dir, int type, GameEntity owner){
            var createPos = pos + DirUtil.GetDirLVec(dir) * TankUtil.TANK_HALF_LEN;
            DelayCall(Define.TankBornDelay, () => {
                var entity = CreateUnit(createPos, _config.enemyPrefabs, type, dir, transParentEnemy);
                entity.bullet.ownerLocalId = owner.localId.value;
            });
        }

        public void CreateEnemy(LVector2 bornPos){
            var type = LRandom.Range(0, _config.enemyPrefabs.Count);
            CreateEnemy(bornPos,type);
        }

        public void CreateEnemy(LVector2 bornPos, int type){
            var createPos = bornPos + Define.TankBornOffset;
            _resourceService.ShowBornEffect(createPos);
            _audioService.PlayClipBorn();
            EDir dir = EDir.Down;
            DelayCall(Define.TankBornDelay, () => {
                CreateUnit(createPos, _config.enemyPrefabs, type, dir, transParentEnemy);
            });
        }

        public void CreatePlayer(byte actorId, int type){
            var bornPos = _globalStateService.playerBornPoss[actorId];
            var createPos = bornPos + Define.TankBornOffset;
            _resourceService.ShowBornEffect(createPos);
            _audioService.PlayClipBorn();
            EDir dir = EDir.Up;
            DelayCall(Define.TankBornDelay, () => {
                CreateUnit(createPos, _config.playerPrefabs, type, dir, transParentPlayer);
            });
        }


        private GameEntity CreateUnit<T>(LVector2 createPos, List<T> prefabLst, int type, EDir dir, Transform parent)
            where T : GameConfig.Unit{
            var ecsPrefab = prefabLst[type];
            var assetId = ecsPrefab.asset.assetId;
            var prefab = Resources.Load<GameObject>(Define.GetAssetPath(assetId));
            var go = GameObject.Instantiate(prefab, this.transform.position + createPos.ToVector3(),
                Quaternion.identity, parent);
            go.AddComponent<PosListener>();
            go.AddComponent<DirListener>();
            
            var entity = CreateGameEntity();
            _viewService.BindView(entity, _gameContext, go);
            ecsPrefab.SetComponentsTo(entity);
            if (entity.hasMove) {
                entity.pos.value = createPos;
            }
            else {
                entity.dir.value = dir;
            }

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

        public void DelayCall(LFloat delay, Action callback){
            var delayEntity = CreateGameEntity();
            delayEntity.AddDelayCall(delay, callback);
        }

        #endregion


        private bool IsGameOver;

        public override void DoUpdate(float deltaTime){
            if (IsGameOver) return;
        }

        /// <summary>
        /// 正式开始游戏
        /// </summary>
        public void StartGame(int level){
            IsGameOver = false;
            _globalStateService.curLevel = level;
            EventHelper.Trigger(EEvent.OnAllPlayerFinishedLoad, null);
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