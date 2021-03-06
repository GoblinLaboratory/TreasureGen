﻿using EventGen;
using Ninject;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using TreasureGen.Items;

namespace TreasureGen.Tests.Integration.Stress
{
    [TestFixture]
    public abstract class StressTests : IntegrationTests
    {
        [Inject]
        public Random Random { get; set; }
        [Inject]
        public Stopwatch Stopwatch { get; set; }
        [Inject]
        public ClientIDManager ClientIdManager { get; set; }
        [Inject]
        public GenEventQueue EventQueue { get; set; }

        private const int ConfidentIterations = 1000000;
        private const int TravisJobOutputTimeLimit = 60 * 10;
        private const int TravisJobBuildTimeLimit = 60 * 50;

        private readonly int timeLimitInSeconds;

        private int iterations;
        private Guid clientId;

        public StressTests()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes();
            var methods = types.SelectMany(t => t.GetMethods());
            var stressTestsCount = methods.Sum(m => m.GetCustomAttributes<TestAttribute>(true).Count());
            var stressTestsCasesCount = methods.Sum(m => m.GetCustomAttributes<TestCaseAttribute>(true).Count());
            var stressTestsTotal = stressTestsCount + stressTestsCasesCount;

            var perTestTimeLimit = TravisJobBuildTimeLimit / stressTestsTotal;

            //INFO: This asserts that the stress tests should have enough time to generate even the rarest of occurrances
            Assert.That(perTestTimeLimit, Is.AtLeast(15));
#if STRESS
            timeLimitInSeconds = Math.Min(perTestTimeLimit, TravisJobOutputTimeLimit - 10);
#else
            timeLimitInSeconds = 1;
#endif
        }

        [SetUp]
        public void StressSetup()
        {
            iterations = 0;
            Stopwatch.Start();

            clientId = Guid.NewGuid();
            ClientIdManager.SetClientID(clientId);
        }

        [TearDown]
        public void StressTearDown()
        {
            Stopwatch.Reset();

            var events = EventQueue.DequeueAll(clientId);

            //INFO: We want to truncate the events to just a summary per second, so last event per minute
            events = events.GroupBy(e => e.When.Second).Select(g => g.ToArray()[0]);

            foreach (var genEvent in events)
                Console.WriteLine($"[{genEvent.When.ToShortTimeString()}] {genEvent.Source}: {genEvent.Message}");
        }

        protected void Stress(Action generate)
        {
            do generate();
            while (TestShouldKeepRunning());

            Console.WriteLine($"Stress test complete after {Stopwatch.Elapsed} and {iterations} iterations");

            if (Stopwatch.Elapsed.TotalSeconds > timeLimitInSeconds + 2)
                Assert.Fail("Something took way too long");
        }

        private bool TestShouldKeepRunning()
        {
            iterations++;
            return Stopwatch.Elapsed.TotalSeconds < timeLimitInSeconds && iterations < ConfidentIterations;
        }

        protected T GenerateOrFail<T>(Func<T> generate, Func<T, bool> isValid)
        {
            T generatedObject;

            do generatedObject = generate();
            while (TestShouldKeepRunning() && isValid(generatedObject) == false);

            Console.WriteLine($"Generation complete after {Stopwatch.Elapsed} and {iterations} iterations");

            if (TestShouldKeepRunning() == false && isValid(generatedObject) == false)
                Assert.Fail($"Stress test timed out after {Stopwatch.Elapsed} and {iterations} iterations");

            return generatedObject;
        }

        protected T Generate<T>(Func<T> generate, Func<T, bool> isValid)
        {
            T generatedObject;

            do generatedObject = generate();
            while (isValid(generatedObject) == false);

            return generatedObject;
        }

        protected int GetNewLevel()
        {
            return Random.Next(1, 31);
        }

        protected string GetNewPower(bool allowMinor = true)
        {
            var limit = 2;
            if (allowMinor)
                limit = 3;

            switch (Random.Next(limit))
            {
                case 0: return PowerConstants.Major;
                case 1: return PowerConstants.Medium;
                default: return PowerConstants.Minor;
            }
        }

        protected string GetNewGearItemType()
        {
            if (Random.Next(2) == 0)
                return ItemTypeConstants.Armor;

            return ItemTypeConstants.Weapon;
        }

        protected IEnumerable<string> GetNewAttributesForGear(string itemType, bool forceMaterials)
        {
            if (itemType == ItemTypeConstants.Weapon)
                return GenerateWeaponAttributes();

            return GenerateArmorAttributes();
        }

        private IEnumerable<string> GenerateArmorAttributes()
        {
            var attributes = new List<string>();

            switch (Random.Next(3))
            {
                case 0: attributes.Add(AttributeConstants.Shield); break;
                case 1: break;
                case 2:
                    attributes.Add(AttributeConstants.Shield);
                    attributes.Add(AttributeConstants.NotTower);
                    break;
            }

            switch (Random.Next(3))
            {
                case 0: attributes.Add(AttributeConstants.Metal); break;
                case 1: attributes.Add(AttributeConstants.Wood); break;
                case 2: break;
            }

            return attributes;
        }

        private IEnumerable<string> GenerateWeaponAttributes()
        {
            var attributes = new List<string>();

            switch (Random.Next(3))
            {
                case 0: attributes.Add(AttributeConstants.Melee); break;
                case 1: attributes.Add(AttributeConstants.Ranged); break;
                case 2:
                    attributes.Add(AttributeConstants.Melee);
                    attributes.Add(AttributeConstants.Ranged);
                    break;
            }

            switch (Random.Next(4))
            {
                case 0: attributes.Add(AttributeConstants.Metal); break;
                case 1: attributes.Add(AttributeConstants.Wood); break;
                case 2:
                    attributes.Add(AttributeConstants.Metal);
                    attributes.Add(AttributeConstants.Wood);
                    break;
                case 3: break;
            }

            if (attributes.Contains(AttributeConstants.Melee) && Random.Next(2) > 0)
                attributes.Add(AttributeConstants.DoubleWeapon);
            else if (attributes.Contains(AttributeConstants.Ranged) && Random.Next(2) > 0)
                attributes.Add(AttributeConstants.Thrown);
            else if (attributes.Contains(AttributeConstants.Ranged) && Random.Next(2) > 0)
                attributes.Add(AttributeConstants.Ammunition);

            switch (Random.Next(7))
            {
                case 0: attributes.Add(AttributeConstants.Bludgeoning); break;
                case 1: attributes.Add(AttributeConstants.Piercing); break;
                case 2: attributes.Add(AttributeConstants.Slashing); break;
                case 3:
                    attributes.Add(AttributeConstants.Piercing);
                    attributes.Add(AttributeConstants.Slashing);
                    break;
                case 4:
                    attributes.Add(AttributeConstants.Bludgeoning);
                    attributes.Add(AttributeConstants.Piercing);
                    break;
                case 5:
                    attributes.Add(AttributeConstants.Piercing);
                    attributes.Add(AttributeConstants.Slashing);
                    break;
                case 6: break;
            }

            return attributes;
        }
    }
}