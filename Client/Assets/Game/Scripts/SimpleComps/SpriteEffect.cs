using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteEffect : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Sprite> sprites = new List<Sprite>();
    public SpriteRenderer renderer;
    public float interval = 1;
    public bool isDestroyAfterFinishPlay = true;
    private float timer;

    private void Start(){
        renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update(){
        timer += Time.deltaTime;
        var idx = (int)((timer / interval ) * sprites.Count) %sprites.Count;
        if (isDestroyAfterFinishPlay) {
            if (idx == 0 && timer > interval) {
                GameObject.Destroy(gameObject);
            } 
        }

        renderer.sprite = sprites[idx];
    }
}
