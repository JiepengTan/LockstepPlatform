using System.IO;
using Lockstep.Game;
using UnityEngine;

public class MainScript : MonoBehaviour {
    public Camera gameCamera;
    public Vector2Int renderTextureSize;
    [HideInInspector] public RenderTexture rt;

    public Launcher _launcher = new Launcher();
    public bool IsDebugMode = false;

    private void Awake(){
        _launcher.GameName = "Tank";
#if UNITY_EDITOR
        _launcher.IsDebugMode = IsDebugMode;
#endif
        _launcher.DoAwake(null);
        rt = new RenderTexture(renderTextureSize.x, renderTextureSize.y, 1, RenderTextureFormat.ARGB32);
        gameCamera.targetTexture = rt;
        Screen.SetResolution(1024, 768, false);
        var service = (UnityUIService) (GetService<IUIService>());
        service.rt = rt;
        service.RegisterAssembly(GetType().Assembly);
    }


    private void Start(){
        var stateService = GetService<IConstStateService>();
        string path = Application.dataPath;
#if UNITY_EDITOR
        path = Application.dataPath + "/../../../";
#elif UNITY_STANDALONE_OSX
        path = Application.dataPath + "/../../../../../";
#elif UNITY_STANDALONE_WIN
        path = Application.dataPath + "/../../../";
#endif
        Debug.Log(path);
        stateService.RelPath = path;
        _launcher.DoStart();
    }

    private void Update(){
        var deltaTimeMs = (int) (Time.deltaTime * 1000);
        _launcher.DoUpdate(deltaTimeMs);
    }

    private void FixedUpdate(){
        _launcher.DoFixedUpdate();
    }

    private void OnDestroy(){
        _launcher.DoDestroy();
    }

    private void OnApplicationQuit(){
        _launcher.OnApplicationQuit();
    }

    private T GetService<T>() where T : IService{
        return _launcher.GetService<T>();
    }
}