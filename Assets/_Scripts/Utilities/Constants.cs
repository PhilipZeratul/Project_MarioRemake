public static class Constants
{
    public static class LayerNames
    {
        public const string Player = "Player";
        public const string Enemy = "Enemy";
        public const string Ground = "Ground";
        public const string Item = "Item";
    }

    public static class TagNames
    {
        public const string Player = "Player";
    }

    public enum HitDirection
    {
        Left,
        Right,
        Top,
        Bottom
    }

    public enum InjectIDs
    {
        Player
    }

    public enum EnemyMoveDireciton
    {
        Left,
        Right,
        Up,
        Down,
        Stop
    }
}
