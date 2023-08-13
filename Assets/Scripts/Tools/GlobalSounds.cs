

using MoreMountains.Tools;
using UnityEngine;

public static class GlobalSounds
{
    //对应的Resources中的资源文件
  public const string S_BGM1 = "Audio/Music/BGM1";
   public const string S_Error = "Audio/Sound/Error";
   public const string S_ButtonWrong = "Audio/Sound/ButtonClickWrong";
   public const string S_DoorButtonIn = "Audio/Sound/DoorButtonIn";
   public const string S_DoorButtonOut = "Audio/Sound/DoorButtonOut";
   public const string S_MetalDoorOpen = "Audio/Sound/MetalDoorOpen";
   public const string S_MetalDoorClose = "Audio/Sound/MetalDoorClose";
   public const string S_SnakeHurtStart = "Audio/Sound/SnakeHurtStart";
   public const string S_SnakeHurtEnd = "Audio/Sound/SnakeHurtEnd";
   public const string S_SnakeMoveSand1 = "Audio/Sound/SandMove1";
   public const string S_SnakeMoveSand2 = "Audio/Sound/SandMove2";
   public const string S_SnakeMoveSand3 = "Audio/Sound/SandMove3";
   public const string S_SnakeMoveSand4 = "Audio/Sound/SandMove4";
   public const string S_SnakeMoveSnow1 = "Audio/Sound/SnowMove1";
   public const string S_SnakeMoveSnow2 = "Audio/Sound/SnowMove2";
   public const string S_SnakeMoveSnow3 = "Audio/Sound/SnowMove3";
   public const string S_SnakeMoveSnow4 = "Audio/Sound/SnowMove4";
   public const string S_SnakeMoveSnow5 = "Audio/Sound/SnowMove5";
   public const string S_SnakeMoveIce1 = "Audio/Sound/IceMove1";
   public const string S_SnakeMoveIce2 = "Audio/Sound/IceMove2";
   public const string S_SnakeMoveIce3 = "Audio/Sound/IceMove3";
   public const string S_SnakeMoveIce4 = "Audio/Sound/IceMove4";
   public const string S_SnakeMoveLave1 = "Audio/Sound/LavaFire";
   public static AudioClip GetSound(this string path)
   {
     return SoundManager.Instance.GetSound(path);
   }
   
}
