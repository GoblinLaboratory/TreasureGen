﻿using NUnit.Framework;
using System;
using TreasureGen.Domain.Tables;

namespace TreasureGen.Tests.Integration.Tables.Items.Magical.Intelligence
{
    [TestFixture]
    public class IntelligenceCommunicationTests : CollectionsTests
    {
        protected override string tableName
        {
            get { return TableNameConstants.Collections.Set.IntelligenceCommunication; }
        }

        [TestCase(12,
            "Empathy")]
        [TestCase(13,
            "Empathy")]
        [TestCase(14,
            "Speech")]
        [TestCase(15,
            "Speech")]
        [TestCase(16,
            "Speech",
            "Read")]
        [TestCase(17, "Speech",
            "Read",
            "Telepathy")]
        [TestCase(18,
            "Speech",
            "Read all languages",
            "Read magic",
            "Telepathy")]
        [TestCase(19,
            "Speech",
            "Read all languages",
            "Read magic",
            "Telepathy")]
        public void Attributes(int highStat, params string[] attributes)
        {
            var name = Convert.ToString(highStat);
            base.Collections(name, attributes);
        }
    }
}