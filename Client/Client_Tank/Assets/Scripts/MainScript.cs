using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using Lockstep.Game;
using UnityEngine;
using Time = Lockstep.Util.Time;

public class MainScript : MonoBehaviour {
    public Launcher launcher = new Launcher();
    private void Awake(){
        launcher.DoAwake(null);
    }

    private void Start(){
        launcher.DoStart();
    }

    private void Update(){
        var deltaTimeMs =(int)( Time.deltaTime * 1000);
        launcher.DoUpdate(deltaTimeMs);
        
    }

    private void FixedUpdate(){
        launcher.DoFixedUpdate();
    }

    private void OnDestroy(){
        launcher.DoDestroy();
        
    }
    private void OnApplicationQuit(){
        launcher.OnApplicationQuit();
    }
}