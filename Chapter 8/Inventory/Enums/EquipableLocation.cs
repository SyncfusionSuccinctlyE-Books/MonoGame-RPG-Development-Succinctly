
namespace MonoGameRPG
{
    public enum EquipableLocation : int
    {
        None = 0,
        Head = 2,
        Neck = 4,
        // both chest and abdomen
        Body = 8,
        Chest = 16,
        Abdomen = 32,
        Left_Arm = 64,
        Right_Arm = 128,
        Left_Leg = 256,
        Right_Leg = 512,
        Feet = 1024,
        TwoHanded = 2048,
        Hand = 4096,
        Left_Hand = 8192,
        Right_Hand = 16384,
    }
}
