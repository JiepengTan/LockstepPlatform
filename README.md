# LockstepPlatform 
 
##### 1.项目目标    

    1. 基于ECS快照回滚帧同步技术，提供一个游戏双端解决方案。
    2. 服务端：实现一个游戏平台，同时支持多个小游戏的运营，类似qq游戏平台
    3. 客户端：实现一个小引擎(除Audio,以及渲染器模块以及Editor，其他的都会基于确定性计算（整型）重写)

##### 目标用户  
   - 独立游戏开发者
   - 中小型公司
   - 喜欢学技术的朋友
   - 想使用部分模块的朋友

##### **当前版本(v0.3.0)**
    1. Excel 数据& 代码生成库 （done）
    2. 基于消息回调的网络消息库（done v0.6.0~v0.7.0 会增强为c# 基于await 异步回调 以及actor 模型）
    3. 客户端编程范式正式改为面向接口编程
    4. 服务器支持分布式(目前不成熟 仅用于测试双端游戏流程，v0.6.0~v0.7.0 会增强)
##### 版本（v0.2.0）
    1. ECS 目前采用Entitas (后续版本会改为基于c#指针&struct的版本。以及兼容Unity ECS 开发模式)
    2. (确定性)碰撞检测库 3D
    3. 序列化库(done)
##### 版本（v0.1.0）
    1. 基于整形的数学库 (done)
    2. (确定性)碰撞检测库 2D

##### 后续版本
###### v0.4.0(开发中)
    1. 基于c# 指针和struct实现的 内存紧凑版 行为树 (done)
    2. (确定性)NavMesh 库 (done) (TODO指针版本)
    3. ECL（a domain-specific language(DSL) for Entity component design）(解释器正在开发中)
    4. AssetBundleLoader (TODO)

###### v0.5.0（TODO）
    1. BehaviorTree 添加 Eidtor 支持
    2. ECL 
        - 兼容Unity ECS 
        - 开始支持基于c# 指针&struct 的版本
        - 支持导出为Excel 方便配制
    3. 2D 物理库
    4. NavMesh 指针化&struct 化
    5. AssetBundleLoader Editor


#### **References：** 
- ECS 回滚原型来自 UnityLockstep:[https://github.com/proepkes/UnityLockstep][1] 
- 当前网络库使用 LiteNetLib: [https://github.com/RevenantX/LiteNetLib][2] 
- ECS Framework Entitas: [https://github.com/sschmid/Entitas-CSharp][3] 
- 确定性 数学库: [https://github.com/JiepengTan/LockstepMath][4]  
- 确定性 碰撞检测库: [https://github.com/JiepengTan/LockstepCollision][5] 
- 确定性 NavMesh 库: [https://github.com/JiepengTan/LockstepPathFinding][8] 
- 内存紧凑版 行为树库: [https://github.com/JiepengTan/LockstepBehaviorTree][9] 
- ECL解释器: [https://github.com/JiepengTan/LockstepECL][10] 
- 其他的库(Serialization，Logging,ExcelParser，Network)没有独立出项目来，但是库是作为单独的子模块位于本项目中[https://github.com/JiepengTan/LockstepPlatform][11]


#### 2D Demo(FC Tank)
    
- v0.3.0 项目演示 [https://www.bilibili.com/video/av55450233][12] 

<p align="center"> <img src="https://github.com/JiepengTan/JiepengTan.github.io/blob/master/assets/img/blog/Show/game_pic.png?raw=true" width="512"/></p>


### ** Basic lockstep and rollback file**
<p align="center"><img src="https://github.com/JiepengTan/JiepengTan.github.io/blob/master/assets/img/blog/Show/lockstepgifbig.gif?raw=true" width="512"></p>

### ** Replay record file**
<p align="center"><img src="https://github.com/JiepengTan/JiepengTan.github.io/blob/master/assets/img/blog/Show/lsp_recode_file2.gif?raw=true" width="940"></p>




#### **How to run it**
//TODO


 [1]: https://github.com/proepkes/UnityLockstep
 [2]: https://github.com/RevenantX/LiteNetLib
 [3]: https://github.com/sschmid/Entitas-CSharp
 [4]: https://github.com/JiepengTan/LockstepMath
 [5]: https://github.com/JiepengTan/LockstepCollision
 [6]: https://github.com/JiepengTan/LockstepPlatform/releases
 [7]: https://github.com/sschmid/Entitas-CSharp/releases
 [8]: https://github.com/JiepengTan/LockstepPathFinding
 [9]: https://github.com/JiepengTan/LockstepBehaviorTree
 [10]: https://github.com/JiepengTan/LockstepECL
 [11]: https://github.com/JiepengTan/LockstepPlatform
 [12]: https://www.bilibili.com/video/av55450233