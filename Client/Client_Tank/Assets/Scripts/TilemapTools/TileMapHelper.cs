using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMapHelper : MonoBehaviour {
   /// <summary>
   /// 查询优先级  越大优先级越高
   /// </summary>
   public int layer;
   /// <summary>
   /// 是否是碰撞图层
   /// </summary>
   public bool isCollider = true;
   /// <summary>
   /// 是否只用于辅助
   /// </summary>
   public bool isSupper;
}
