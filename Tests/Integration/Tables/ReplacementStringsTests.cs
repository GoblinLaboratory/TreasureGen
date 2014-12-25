﻿using System;
using EquipmentGen.Selectors.Decorators;
using EquipmentGen.Tables.Interfaces;
using NUnit.Framework;

namespace EquipmentGen.Tests.Integration.Tables
{
    [TestFixture]
    public class ReplacementStringsTests : AttributesTests
    {
        protected override String tableName
        {
            get { return TableNameConstants.Attributes.Set.ReplacementStrings; }
        }

        [TestCase(TableNameConstants.Attributes.Set.ReplacementStrings,
            ReplacementStringConstants.DesignatedFoe,
            ReplacementStringConstants.Gender,
            ReplacementStringConstants.Height,
            ReplacementStringConstants.KnowledgeCategory,
            ReplacementStringConstants.Energy,
            ReplacementStringConstants.ProtectionAlignment)]
        [TestCase(ReplacementStringConstants.DesignatedFoe,
            TableNameConstants.Percentiles.Set.DesignatedFoes)]
        [TestCase(ReplacementStringConstants.Gender,
            TableNameConstants.Percentiles.Set.Gender)]
        [TestCase(ReplacementStringConstants.Height,
            TableNameConstants.Percentiles.Set.CurseHeightChanges)]
        [TestCase(ReplacementStringConstants.KnowledgeCategory,
            TableNameConstants.Percentiles.Set.KnowledgeCategories)]
        [TestCase(ReplacementStringConstants.Energy,
            TableNameConstants.Percentiles.Set.Elements)]
        [TestCase(ReplacementStringConstants.ProtectionAlignment,
            TableNameConstants.Percentiles.Set.ProtectionAlignments)]
        public override void Attributes(String name, params String[] attributes)
        {
            base.Attributes(name, attributes);
        }
    }
}