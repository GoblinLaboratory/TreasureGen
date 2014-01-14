﻿using System;
using System.Collections.Generic;
using EquipmentGen.Core.Data.Goods;

namespace EquipmentGen.Core.Generation.Factories.Interfaces
{
    public interface IGoodsFactory
    {
        IEnumerable<Good> CreateAtLevel(Int32 level);
    }
}