using System;
using System.Collections;

namespace Lockstep.Util {
    public class Time {
        /// The total number of frames that have passed (Read Only).
        public static int frameCount { get; private set; }

        /// The time in seconds it took to complete the last frame (Read Only).
        public static float deltaTime { get; private set; }

        /// The time this frame has started (Read Only). This is the time in seconds since the last level has been loaded.
        public static float timeSinceLevelLoad { get; private set; }

        /// The real time in seconds since the game started (Read Only).
        public static float realtimeSinceStartup => (float) (DateTime.Now - _initTime).TotalSeconds;

        private static DateTime _initTime;
        private static DateTime lastFrameTime;

        public static void DoStart(){
            _initTime = DateTime.Now;
        }

        public static void DoUpdate(){
            var now = DateTime.Now;
            deltaTime = (float) ((now - lastFrameTime).TotalSeconds);
            timeSinceLevelLoad = (float) ((now - _initTime).TotalSeconds);
            frameCount++;
            lastFrameTime = now;
        }
    }
}