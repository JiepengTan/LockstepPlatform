using System;
using System.Collections.Generic;
using System.Reflection;
using DesperateDevs.Utils;
using Entitas;
using Lockstep.ECS.Game;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Lockstep.Game {
    [System.Serializable]
    public class BaseEntitySetter {
        public static Dictionary<Type, int> type2Idx = new Dictionary<Type, int>();
        public static Dictionary<string, int> name2Idx;

        public virtual void SetComponentsTo(Entity targetEntity){
            var allMemberInfos = this.GetType().GetPublicMemberInfos();
            foreach (var memberInfo in allMemberInfos) {
                int index = 0;
                var memType = memberInfo.type;
                if (type2Idx.TryGetValue(memType, out int qidx)) {
                    index = qidx;
                }
                else {
                    if (name2Idx == null) {
                        name2Idx = new Dictionary<string, int>();
                        var fileds = typeof(GameComponentsLookup).GetFields(BindingFlags.Static | BindingFlags.Public);
                        foreach (var filed in fileds) {
                            if (filed.IsLiteral && !filed.IsInitOnly && filed.FieldType == typeof(int)) {
                                name2Idx.Add(filed.Name + "Component", (int) filed.GetRawConstantValue());
                            }
                        }
                    }

                    if (name2Idx.TryGetValue(memType.Name, out int nidx)) {
                        index = nidx;
                        type2Idx.Add(memType, nidx);
                    }
                    else {
                        Debug.LogError("Do not have type" + memType.Name.ToString());
                        return;
                    }
                }

                IComponent srcComp = memberInfo.GetValue(this) as IComponent;
                if (targetEntity.HasComponent(index)) {
                    IComponent dstComp = targetEntity.GetComponent(index);
                    srcComp.CopyPublicMemberValues((object) dstComp);
                }
                else {
                    IComponent dstComp = targetEntity.CreateComponent(index, srcComp.GetType());
                    srcComp.CopyPublicMemberValues((object) dstComp);
                    targetEntity.AddComponent(index, dstComp);
                }
            }
        }
    }
}