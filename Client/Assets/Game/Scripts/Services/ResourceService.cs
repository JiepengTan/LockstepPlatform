using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lockstep.Game {
    public partial class ResourceService {
        private GameObject prefab;
        private Transform transParent;

        public override void DoStart(){
            base.DoStart();
            transParent = new GameObject("GoParents").transform;
            prefab = GameObject.CreatePrimitive(PrimitiveType.Cube);
            prefab.SetActive(false);
            prefab.transform.SetParent(transParent, false);
            prefab.AddComponent<HashCodeSetter>();
            prefab.AddComponent<PositionListener>();
        }

        protected GameObject InstantiatePrefab(int configId){
            return UnityEngine.Object.Instantiate(prefab, transParent).gameObject;
            ;
        }
    }
}