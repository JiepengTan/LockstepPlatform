using System.Linq;       
using Entitas;        
using UnityEngine;                      

public class UnityInput : MonoBehaviour
{                              
    void Update()
    {
        //if (Input.GetMouseButton(1))
        //{   
        //    var pos = GetWorldPos(Input.mousePosition);
        //    FindObjectOfType<RTSEntitySpawner>().Spawn(pos);
        //}
//
        //if (Input.GetKeyDown(KeyCode.X))
        //{
        //    var e = Contexts.sharedInstance.game
        //        .GetEntities(GameMatcher.LocalId)
        //        .Where(entity => entity.actorId.value == RTSNetworkedSimulation.Instance.Simulation.LocalActorId)
        //        .Select(entity => entity.id.value).ToArray();      
//
        //    RTSNetworkedSimulation.Instance.Execute(new NavigateCommand
        //    {
        //        Destination = GetWorldPos(Input.mousePosition),
        //        Selection = e
        //    });
        //}
    }
}
