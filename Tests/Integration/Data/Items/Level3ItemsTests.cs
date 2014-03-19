﻿using System;
using EquipmentGen.Core.Data.Items.Constants;
using EquipmentGen.Tests.Integration.Tables.Attributes;
using NUnit.Framework;

namespace EquipmentGen.Tests.Integration.Tables.Items
{
    [TestFixture, PercentileTable("Level3Items")]
    public class Level3ItemsTests : PercentileTests
    {
        [Test]
        public void Level3ItemsEmptyPercentile()
        {
            AssertEmpty(1, 49);
        }

        [Test]
        public void Level3ItemsMundanePercentile()
        {
            var content = String.Format("{0},1d3", PowerConstants.Mundane);
            AssertContent(content, 50, 79);
        }

        [Test]
        public void Level3ItemsMinorPercentile()
        {
            var content = String.Format("{0},1", PowerConstants.Minor);
            AssertContent(content, 80, 100);
        }
    }
}