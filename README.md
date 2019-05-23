# LockstepPlatform 
基于ECS 可回滚 帧同步解决方案，含demo  FC Tank 
A lockstep soulution base on ECS Framework for rollback ,Include a demo FCTank 

<p align="center"> <img src="https://github.com/JiepengTan/JiepengTan.github.io/blob/master/assets/img/blog/Show/game_pic.png?raw=true" width="512"/></p>

## **Done**
    - Ring network frame buffer
    - ECS rollback
    - IService rollback
    - IService autoregister
    - NetworkMsg auto parse and register
    - Event auto register


## **TODO**
    - pursue frame after reconnected
    - changed Server API to PUN API
    - using UDP for Lockstep frame msg


## **References：**
    - Inspired by UnityLockstep:[https://github.com/proepkes/UnityLockstep][1] 
    - Network using LiteNetLib: [https://github.com/RevenantX/LiteNetLib][2] 
    - Uses Entitas as ECS Framework: [https://github.com/sschmid/Entitas-CSharp][3] 
    - Deterministic Math lib: [https://github.com/JiepengTan/LockstepMath][4] 
    - Deterministic Collision2D lib  : [https://github.com/JiepengTan/LockstepCollision2D][5] 
