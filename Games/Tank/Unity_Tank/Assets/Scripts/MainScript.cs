using Lockstep.Game;
using UnityEngine;

public class MainScript : MonoBehaviour {
    public Camera gameCamera;
    public Vector2Int renderTextureSize;
    [HideInInspector] public RenderTexture rt;
    
    public Launcher _launcher = new Launcher();
    private void Awake(){         
        _launcher.DoAwake(null);
        rt = new RenderTexture(renderTextureSize.x, renderTextureSize.y, 1, RenderTextureFormat.ARGB32);
        gameCamera.targetTexture = rt;
        Screen.SetResolution(1024, 768, false);
        var service = (UnityUIService) (_launcher.GetService<IUIService>());
        service.rt = rt;
    }

    private void Start(){
        _launcher.DoStart();
    }

    private void Update(){
        var deltaTimeMs =(int)( Time.deltaTime * 1000);
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
}