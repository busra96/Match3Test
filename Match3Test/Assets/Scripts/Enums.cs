namespace Enums
{
   public enum Direction
   {
      Right,
      Up,
      Left,
      Down,
      None,
   }

   public enum InputState
   {
      None,
      Swipe,
      Match,
   }
   
   public enum TileState
   {
      None,
      Destroy,
      Falling
   }

   public enum TileCanDropSpawn
   {
      CantDropSpawn,
      CanDropSpawn
   }

   public enum TileEmptyOrFull
   {
      Empty,
      Full
   }

   public enum DropItemColor
   {
      Green,
      Red,
      Yellow,
      Blue
   }
   
}

