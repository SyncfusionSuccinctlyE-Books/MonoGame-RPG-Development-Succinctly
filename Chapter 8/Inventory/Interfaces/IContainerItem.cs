using System;
using System.Collections.Generic;
using System.Text;

namespace MonoGameRPG
{
    public interface IContainerItem : IInventoryContainer
    {
        IInventoryContainer Content { get; set; }

        void Open();
    }
}
