# LockstepPlatform 
- A lockstep solution base on ECS Framework,Include a demo FCTank 
- 基于ECS 可回滚 帧同步解决方案，含demo  FC Tank 


<p align="center"> <img src="https://github.com/JiepengTan/JiepengTan.github.io/blob/master/assets/img/blog/Show/game_pic.png?raw=true" width="512"/></p>


### ** Basic lockstep and rollback file**
<p align="center"><img src="https://github.com/JiepengTan/JiepengTan.github.io/blob/master/assets/img/blog/Show/lockstepgifbig.gif?raw=true" width="512"></p>

### ** Replay record file**
<p align="center"><img src="https://github.com/JiepengTan/JiepengTan.github.io/blob/master/assets/img/blog/Show/lsp_recode_file2.gif?raw=true" width="940"></p>

## **Done**
    - Ring network frame buffer
    - ECS rollback
    - IService rollback
    - IService autoregister
    - Add a demo "Tank"
    - NetworkMsg auto parse and register
    - Event auto register
    - Pursue frame after reconnected
    - Record game and replay it offline
    - Code Generation for the IComponent to reduce GC when backup game status
    - Add a Demo "BombMan"

## **TODO**
    - Behavior Tree 
    - Collision3D
    - MeshNavigation & A* 
    - using UDP for Lockstep frame msg
    - A ARPG Demo

## **How to run it**
0. goto [https://github.com/sschmid/Entitas-CSharp/releases][7] download Entitas 1.12.2 and import it into Client
1. goto release page  [https://github.com/JiepengTan/LockstepPlatform/releases][6] down load the source code 
2. uncompress it into "your_dir"

### Mac User
3. do command "cd your_dir"   eg:your_dir = /Users/xxx/LockstepPlatform-0.2.0 
4. do command "./Tools/Rebuild"
5. open unity to build the a client and save it to dir: client_dir/Build/    and rename it "LPClient"  eg:client_dir = /Users/xxx/LockstepPlatform-0.2.0/Client/Client_Tank
6. do command "client_dir/CopyLibs"
7. do command "client_dir/RunServerAndOneClient"
8. open unity and hit play button "|>"

### Win user
3. do command "cd your_dir"   eg:your_dir = D:\xxx\LockstepPlatform-0.2.0 
4. open your_dir/Server/Server.sln and rebuild and run
5. open unity to build the a client and save it to dir: client_dir/Build/    and rename it "LPClient"  eg:client_dir = D:\xxx\LockstepPlatform-0.2.0\Client\Client_Tank
6. copy all dlls in dir "your_dir/Libs/"  to dir "client_dir/Assets/Plugins/Libs"
7. run client_dir/Build/LPClient.exe
8. open unity and hit play button "|>"


## **References：** 
- Inspired by UnityLockstep:[https://github.com/proepkes/UnityLockstep][1] 
- Network using LiteNetLib: [https://github.com/RevenantX/LiteNetLib][2] 
- Uses Entitas as ECS Framework: [https://github.com/sschmid/Entitas-CSharp][3] 
- Deterministic Math lib: [https://github.com/JiepengTan/LockstepMath][4] 
- Deterministic Collision lib  : [https://github.com/JiepengTan/LockstepCollision][5] 



 [1]: https://github.com/proepkes/UnityLockstep
 [2]: https://github.com/RevenantX/LiteNetLib
 [3]: https://github.com/sschmid/Entitas-CSharp
 [4]: https://github.com/JiepengTan/LockstepMath
 [5]: https://github.com/JiepengTan/LockstepCollision
 [6]: https://github.com/JiepengTan/LockstepPlatform/releases
 [7]: https://github.com/sschmid/Entitas-CSharp/releases
