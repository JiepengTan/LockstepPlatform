using Entitas;
using Lockstep.ECS.Game;
using NetMsg.Common;

namespace Lockstep.Game {
    public static class InputUtil {
        public static void Execute(InputCmd cmd, IEntity sentity){
            if (cmd.content == null) {
                return;
            }
            var entity = sentity as InputEntity;

            var type = cmd.content[0];
            if (EnumBitUtil.HasBit(type, EInputCmdType.Up)) entity.AddMoveDir(EDir.Up);
            if (EnumBitUtil.HasBit(type, EInputCmdType.Left)) entity.AddMoveDir(EDir.Left);
            if (EnumBitUtil.HasBit(type, EInputCmdType.Down)) entity.AddMoveDir(EDir.Down);
            if (EnumBitUtil.HasBit(type, EInputCmdType.Right)) entity.AddMoveDir(EDir.Right);
            if (EnumBitUtil.HasBit(type, EInputCmdType.Fire)) {
                entity.isFire = true;
            }
        }
    }
}