﻿using EventGen;
using TreasureGen.Coins;

namespace TreasureGen.Domain.Generators.Coins
{
    internal class CoinGeneratorEventGenDecorator : ICoinGenerator
    {
        private GenEventQueue eventQueue;
        private ICoinGenerator innerGenerator;

        public CoinGeneratorEventGenDecorator(ICoinGenerator innerGenerator, GenEventQueue eventQueue)
        {
            this.eventQueue = eventQueue;
            this.innerGenerator = innerGenerator;
        }

        public Coin GenerateAtLevel(int level)
        {
            eventQueue.Enqueue("TreasureGen", $"Beginning level {level} coin generation");
            var coin = innerGenerator.GenerateAtLevel(level);
            eventQueue.Enqueue("TreasureGen", $"Completed generation of level {level} coin: {coin.Quantity} {coin.Currency}");

            return coin;
        }
    }
}
