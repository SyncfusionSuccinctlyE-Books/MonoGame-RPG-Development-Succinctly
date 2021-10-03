using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGameRPG
{
    public interface IWeapon
    {
        string Damage { get; set; }
        int Range { get; set; }
    }
}
