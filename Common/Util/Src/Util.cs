namespace Lockstep.Util {
    public class Utils {
        public static void StartServices(){
            Time.DoStart();
            CoroutineHelper.DoStart();
        }

        public static void UpdateServices(){
            Time.DoUpdate();
            CoroutineHelper.DoUpdate();
        }
    }
}