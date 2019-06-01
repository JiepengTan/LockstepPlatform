using System.Collections;

namespace Server.Common {
    public class Time {
        /// The time at the beginning of this frame (Read Only). This is the time in seconds since the start of the game.
        public static extern float time { get; set; }

        /// The time this frame has started (Read Only). This is the time in seconds since the last level has been loaded.
        public static extern float timeSinceLevelLoad { get; set; }

        /// The time in seconds it took to complete the last frame (Read Only).
        public static extern float deltaTime { get; set; }

        /// The time the latest MonoBehaviour.FixedUpdate has started (Read Only). This is the time in seconds since the start of the game.
        public static extern float fixedTime { get; set; }

        /// The timeScale-independant time for this frame (Read Only). This is the time in seconds since the start of the game.
        public static extern float unscaledTime { get; set; }

        /// The TimeScale-independant time the latest MonoBehaviour.FixedUpdate has started (Read Only). This is the time in seconds since the start of the game.
        public static extern float fixedUnscaledTime { get; set; }

        /// The timeScale-independent interval in seconds from the last frame to the current one (Read Only).
        public static extern float unscaledDeltaTime { get; set; }

        /// The timeScale-independent interval in seconds from the last fixed frame to the current one (Read Only).
        public static extern float fixedUnscaledDeltaTime { get; set; }

        /// The interval in seconds at which physics and other fixed frame rate updates (like MonoBehaviour's MonoBehaviour.FixedUpdate) are performed.
        public static extern float fixedDeltaTime { get; set; }

        /// The maximum time a frame can take. Physics and other fixed frame rate updates (like MonoBehaviour's MonoBehaviour.FixedUpdate) will be performed only for this duration of time per frame.
        public static extern float maximumDeltaTime { get; set; }

        /// A smoothed out Time.deltaTime (Read Only).
        public static extern float smoothDeltaTime { get; set; }

        /// The maximum time a frame can spend on particle updates. If the frame takes longer than this, then updates are split into multiple smaller updates.
        public static extern float maximumParticleDeltaTime { get; set; }

        /// The scale at which the time is passing. This can be used for slow motion effects.
        public static extern float timeScale { get; set; }

        /// The total number of frames that have passed (Read Only).
        public static extern int frameCount { get; set; }

        public static extern int renderedFrameCount { get; set; }

        /// The real time in seconds since the game started (Read Only).
        public static extern float realtimeSinceStartup { get; set; }

        /// Slows game playback time to allow screenshots to be saved between frames.
        public static extern int captureFramerate { get; set; }

        /// Returns true if called inside a fixed time step callback (like MonoBehaviour's MonoBehaviour.FixedUpdate), otherwise returns false.
        public static extern bool inFixedTimeStep { get; set; }
    }
    public class YieldInstruction
    {
    }
    /// <summary>
    ///   <para>Suspends the coroutine execution for the given amount of seconds using scaled time.</para>
    /// </summary>
    public sealed class WaitForSeconds : YieldInstruction
    {
        internal float m_Seconds;

        /// <summary>
        ///   <para>Creates a yield instruction to wait for a given number of seconds using scaled time.</para>
        /// </summary>
        /// <param name="seconds"></param>
        public WaitForSeconds(float seconds)
        {
            this.m_Seconds = seconds;
        }
    }
    
    /// <summary>
    ///   <para>Base class for custom yield instructions to suspend coroutines.</para>
    /// </summary>
    public abstract class CustomYieldInstruction : IEnumerator
    {
        /// <summary>
        ///   <para>Indicates if coroutine should be kept suspended.</para>
        /// </summary>
        public abstract bool keepWaiting { get; }

        public object Current
        {
            get
            {
                return (object) null;
            }
        }

        public bool MoveNext()
        {
            return this.keepWaiting;
        }

        public void Reset()
        {
        }
    }
    
    /// <summary>
    ///   <para>Suspends the coroutine execution for the given amount of seconds using unscaled time.</para>
    /// </summary>
    public class WaitForSecondsRealtime : CustomYieldInstruction
    {
        private float m_WaitUntilTime = -1f;

        /// <summary>
        ///   <para>Creates a yield instruction to wait for a given number of seconds using unscaled time.</para>
        /// </summary>
        /// <param name="time"></param>
        public WaitForSecondsRealtime(float time)
        {
            this.waitTime = time;
        }

        /// <summary>
        ///   <para>The given amount of seconds that the yield instruction will wait for.</para>
        /// </summary>
        public float waitTime { get; set; }

        public override bool keepWaiting
        {
            get
            {
                if ((double) this.m_WaitUntilTime < 0.0)
                    this.m_WaitUntilTime = Time.realtimeSinceStartup + this.waitTime;
                bool flag = (double) Time.realtimeSinceStartup < (double) this.m_WaitUntilTime;
                if (!flag)
                    this.m_WaitUntilTime = -1f;
                return flag;
            }
        }
    }
}