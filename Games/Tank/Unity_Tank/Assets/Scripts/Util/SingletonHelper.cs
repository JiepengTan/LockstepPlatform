namespace Lockstep.Game
{
    public class Singleton<T> where T : new()
    {
        private static readonly T _instance = new T();
        protected Singleton() { }
        public static T Instance => _instance;
    }
}
