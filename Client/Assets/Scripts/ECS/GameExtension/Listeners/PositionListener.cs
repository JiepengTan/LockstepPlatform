using UnityEngine;

public class PositionListener : MonoBehaviour, IEventListener, IPositionListener {
    private GameEntity _entity;

    public void RegisterListeners(GameEntity entity){
        _entity = entity;
        _entity.AddPositionListener(this);
    }

    public void UnregisterListeners(){
        _entity.RemovePositionListener(this);
    }

    public void OnPosition(GameEntity entity, Lockstep.Math.LVector2 newPosition){
        transform.position = new Vector3((float) newPosition.x.ToFloat(), (float) newPosition.y.ToFloat(), 0);
    }
}