﻿using System;
using System.Collections.Generic;
using System.Linq;
using EquipmentGen.Core.Data.Items.Constants;

namespace EquipmentGen.Core.Data.Items
{
    public class Item
    {
        public String Name { get; set; }
        public List<String> Traits { get; set; }
        public IEnumerable<String> Attributes { get; set; }
        public Dictionary<Magic, Object> Magic { get; set; }
        public Int32 Quantity { get; set; }

        public Item()
        {
            Traits = new List<String>();
            Attributes = Enumerable.Empty<String>();
            Magic = new Dictionary<Magic, Object>();
            Quantity = 1;
        }
    }
}