using System;
using Lockstep.Math;

namespace Lockstep.Math {
//TODO 查gcc random 源码 替换之
    public class LRandom {
        public static int count = 0;

        ulong randSeed = 1;

        public LRandom(uint seed){
            randSeed = seed;
        }

        public uint Next(){
            randSeed = randSeed * 1103515245 + 36153;
            return (uint) (randSeed / 65536);
        }

        // range:[0 ~(max-1)]
        public uint Next(uint max){
            return Next() % max;
        }

        // range:[min~(max-1)]
        public uint Range(uint min, uint max){
            if (min > max)
                throw new ArgumentOutOfRangeException("minValue",
                    string.Format("'{0}' cannot be greater than {1}.", min, max));

            uint num = max - min;
            return Next(num) + min;
        }

        public int Next(int max){
            return (int) (Next() % max);
        }

        public int Range(int min, int max){
            count++;

            if (min > max)
                throw new ArgumentOutOfRangeException("minValue",
                    string.Format("'{0}' cannot be greater than {1}.", min, max));

            int num = max - min;

            return Next(num) + min;
        }

        public LFloat Range(LFloat min, LFloat max){
            if (min > max)
                throw new ArgumentOutOfRangeException("minValue",
                    string.Format("'{0}' cannot be greater than {1}.", min, max));

            uint num = (uint) (max._val - min._val);
            return new LFloat(true,Next(num) + min._val);
        }
    }
}