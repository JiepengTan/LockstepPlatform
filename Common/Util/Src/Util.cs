namespace Lockstep.Util {
    public class Utils {
        public static void StartServices(){
            LTime.DoStart();
            CoroutineHelper.DoStart();
        }

        public static void UpdateServices(){
            LTime.DoUpdate();
            CoroutineHelper.DoUpdate();
        }
    }
}