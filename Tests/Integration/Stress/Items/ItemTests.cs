﻿using NUnit.Framework;
using System;
using TreasureGen.Common.Items;

namespace TreasureGen.Tests.Integration.Stress.Items
{
    [TestFixture]
    public abstract class ItemTests : StressTests
    {
        protected Item GenerateOrFail(Func<Item, Boolean> isValid)
        {
            var item = Generate(i => isValid(i) || TestShouldKeepRunning() == false);

            if (TestShouldKeepRunning() == false && isValid(item) == false)
                Assert.Fail("Item did not occur within the time span");

            return item;
        }

        protected Item Generate(Func<Item, Boolean> isValid)
        {
            Item item;

            do item = GenerateItem();
            while (isValid(item) == false);

            return item;
        }

        protected abstract Item GenerateItem();
    }
}