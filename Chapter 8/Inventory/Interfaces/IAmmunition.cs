using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGameRPG
{
    public interface IAmmunition
    {
        int Quantity { get; set; }
        List<string> Weapons { get; }
    }
}
