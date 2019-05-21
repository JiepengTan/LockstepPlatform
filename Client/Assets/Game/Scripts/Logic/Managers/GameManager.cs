using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Collections;
using System;
using Lockstep.Core;
using Debug = Lockstep.Logging.Debug;
using Random = UnityEngine.Random;


namespace Lockstep.Game {
    [System.Serializable]
    public partial class GameManager : SingletonManager<GameManager> {
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
        }

        public void OnEvent_LoadMapDone(object param){
            var level = (int) param;
            StartGame(level);
        }

        private bool IsGameOver;

        public override void DoUpdate(float deltaTime){
            if (IsGameOver) return;
        }

        /// <summary>
        /// 正式开始游戏
        /// </summary>
        public void StartGame(int level){
            IsGameOver = false;
            //reset variables
            _gameStateService.curLevel = level;
            //read map info
                 // init player pos info
            //test
            EventHelper.Trigger(EEvent.OnAllPlayerFinishedLoad,null);

            // for (int i = 0; i < MAX_PLAYER_COUNT; i++) {
            //     var playerInfo = allPlayerInfos[i];
            //     if (playerInfo != null && !(playerInfo.remainPlayerLife == 0 && !playerInfo.isLiveInLastLevel)) {
            //         CreatePlayer(playerInfo, playerInfo.lastLevelTankType, false);
            //     }
            // }
//
            // //create camps
            // var pos = (campPoss[0] + Vector2.one);
            // camp = GameObject.Instantiate(CampPrefab, transform.position + (Vector3) pos, Quaternion.identity,
            //         transParentItem.parent)
            //     .GetComponent<Camp>();
            // camp.pos = pos;
            // camp.size = Vector2.one;
            // camp.radius = camp.size.magnitude;
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

        #region Create& Destroy

        /*
         public void CreateEnemy(Vector2Int pos, int type){
             //StartCoroutine(YieldCreateEnemy(pos, type));
             //
             //ShowBornEffect(pos + TankBornOffset);
             //yield return new WaitForSeconds(TankBornDelay);
             //var unit = CreateUnit(pos, tankPrefabs, type, TankBornOffset, transParentEnemy, EDir.Down, allEnmey);
             //unit.camp = Global.EnemyCamp;
             //RemainEnemyCount--;
         }
 
         public void CreatePlayer(PlayerInfo playerInfo, int type = 0, bool isConsumeLife = true){
             //StartCoroutine(YiledCreatePlayer(playerInfo.bornPos, type, playerInfo, isConsumeLife));
         }
 
 
         public IEnumerator YiledCreatePlayer(Vector2 pos, int type, PlayerInfo playerInfo, bool isConsumeLife){
             //ShowBornEffect(pos + TankBornOffset);
             //_audioMgr.PlayClipBorn();
             //yield return new WaitForSeconds(TankBornDelay);
             //DirectCreatePlayer(pos, type, playerInfo, isConsumeLife);
         }
 
         private Tank DirectCreatePlayer(Vector2 pos, int type, PlayerInfo playerInfo, bool isConsumeLife){
             var unit = CreateUnit(pos, playerPrefabs, type, TankBornOffset, transParentPlayer, EDir.Up, allPlayer);
             unit.camp = Global.PlayerCamp;
             unit.brain.enabled = false;
             unit.name = "PlayerTank";
             playerInfo.tank = unit;
             if (isConsumeLife) {
                 playerInfo.remainPlayerLife--;
             }
 
             if (OnLifeCountChanged != null) {
                 OnLifeCountChanged(playerInfo);
             }
 
             return unit;
         }
 
         public void ShowBornEffect(Vector2 pos){
             GameObject.Instantiate(BornPrefab, transform.position + new Vector3(pos.x, pos.y), Quaternion.identity);
         }
 
         public void ShowDiedEffect(Vector2 pos){
             GameObject.Instantiate(DiedPrefab, transform.position + new Vector3(pos.x, pos.y), Quaternion.identity);
         }
 
 
         public Bullet CreateBullet(Vector2 pos, EDir dir, Vector2 offset, int type){
             return CreateUnit(pos, bulletPrefabs, type, offset, transParentBullet, dir, allBullet);
         }
 
         public void CreateItem(Vector2 pos, int type){
             CreateUnit(pos, itemPrefabs, type, Vector2.one, transParentItem, EDir.Up, allItem);
         }
 
 
         private T CreateUnit<T>(Vector2 pos, List<GameObject> lst, int type,
             Vector2 offset, Transform parent, EDir dir,
             List<T> set) where T : Unit{
             Debug.Assert(type <= lst.Count, "type >= lst.Count");
             var prefab = lst[type];
             Debug.Assert(prefab != null, "prefab == null");
             Vector2 createPos = pos + offset;
 
             var deg = ((int) (dir)) * 90;
             var rotation = Quaternion.Euler(0, 0, deg);
 
             var go = GameObject.Instantiate(prefab, parent.position + (Vector3) createPos, rotation, parent);
             var unit = go.GetComponent<T>();
             unit.pos = createPos;
             unit.dir = dir;
             unit.detailType = type;
             if (unit is Tank) {
                 unit.size = Vector2.one;
                 unit.radius = unit.size.magnitude;
             }
 
             if (unit is Item) {
                 unit.size = Vector2.one;
                 unit.radius = unit.size.magnitude;
             }
 
             unit.DoStart();
             set.Add(unit);
             return unit;
         }
 
         public void DestroyUnit<T>(Unit unit, ref T rUnit) where T : Unit{
             if (unit is T) {
                 GameObject.Destroy(unit.gameObject);
                 ShowDiedEffect(unit.pos);
                 _audioMgr.PlayClipDied();
                 unit.DoDestroy();
                 rUnit = null;
             }
         }
 
         public void DestroyUnit<T>(T unit, List<T> lst) where T : Unit{
             if (lst.Remove(unit)) {
                 var tank = unit as Tank;
                 if (tank != null) {
                     ShowDiedEffect(unit.pos);
                     _audioMgr.PlayClipDied();
                     if (tank.camp == Global.EnemyCamp) {
                         var info = GetPlayerFormTank(tank.killer);
                         info.score += (tank.detailType + 1) * 100;
 
                         if (OnScoreChanged != null) {
                             OnScoreChanged(info);
                         }
 
                         if (
                             tank.detailType >= Global.ItemTankType &&
                             itemPrefabs.Count > 0) {
                             var x = Random.Range(min.x + 1.0f, max.x - 3.0f);
                             var y = Random.Range(min.y + 1.0f, max.y - 3.0f);
                             CreateItem(new Vector2(x, y), Random.Range(0, itemPrefabs.Count));
                         }
                     }
 
                     if (tank.camp == Global.PlayerCamp) {
                         var info = GetPlayerFormTank(tank);
                         Debug.Assert(info != null, " player's tank have no owner");
                         if (info.remainPlayerLife > 0) {
                             CreatePlayer(info, 0);
                         }
                     }
                 }
 
 
                 unit.DoDestroy();
             }
         }
 
 
         private void DestoryUnit<T>(ref T unit) where T : Unit{
             if (unit != null) {
                 GameObject.Destroy(unit.gameObject);
                 unit = null;
             }
         }
 */

        #endregion
    }
}