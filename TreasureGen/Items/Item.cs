﻿using System.Collections.Generic;
using System.Linq;
using TreasureGen.Items.Magical;

namespace TreasureGen.Items
{
    public class Item
    {
        public string Name { get; set; }
        public IEnumerable<string> BaseNames { get; set; }
        public string ItemType { get; set; }
        public HashSet<string> Traits { get; set; }
        public IEnumerable<string> Attributes { get; set; }
        public Magic Magic { get; set; }
        public int Quantity { get; set; }
        public List<string> Contents { get; set; }

        public bool IsMagical
        {
            get
            {
                return isMagical
                       || Magic.Bonus > 0
                       || Magic.Charges > 0
                       || !string.IsNullOrEmpty(Magic.Curse)
                       || Magic.Intelligence.Ego > 0
                       || Magic.SpecialAbilities.Any();
            }
            set
            {
                isMagical = value;
            }
        }

        public bool CanBeUsedAsWeaponOrArmor
        {
            get
            {
                var allWeapons = WeaponConstants.GetAllWeapons();
                var allArmors = ArmorConstants.GetAllArmors(true);

                return ItemType == ItemTypeConstants.Weapon
                    || ItemType == ItemTypeConstants.Armor
                    || allWeapons.Contains(Name)
                    || allWeapons.Intersect(BaseNames).Any()
                    || allArmors.Contains(Name)
                    || allArmors.Intersect(BaseNames).Any();
            }
        }

        private bool isMagical;

        public Item()
        {
            Traits = new HashSet<string>();
            Attributes = Enumerable.Empty<string>();
            Magic = new Magic();
            Quantity = 1;
            Name = string.Empty;
            BaseNames = Enumerable.Empty<string>();
            Contents = new List<string>();
            ItemType = string.Empty;
        }

        public Item Clone()
        {
            var clone = MundaneClone();
            clone.Magic.Bonus = Magic.Bonus;
            clone.Magic.Charges = Magic.Charges;
            clone.Magic.Curse = Magic.Curse;
            clone.Magic.Intelligence = Magic.Intelligence.Clone();
            clone.Magic.SpecialAbilities = Magic.SpecialAbilities.ToArray();

            return clone;
        }

        public Item SmartClone()
        {
            var clone = MundaneClone();

            clone.Magic.Curse = Magic.Curse;

            if (clone.CanBeUsedAsWeaponOrArmor)
                clone.Magic.SpecialAbilities = Magic.SpecialAbilities.ToArray();

            if (ItemType == ItemTypeConstants.Wand)
                clone.Contents.Clear();

            if (ItemType != ItemTypeConstants.Potion && ItemType != ItemTypeConstants.Scroll)
                clone.Magic.Charges = Magic.Charges;

            if (ItemType != ItemTypeConstants.Wand && ItemType != ItemTypeConstants.Scroll)
                clone.Magic.Bonus = Magic.Bonus;

            var nonIntelligenceItems = new[] { AttributeConstants.OneTimeUse, AttributeConstants.Ammunition };
            if (Attributes.Intersect(nonIntelligenceItems).Any() == false)
                clone.Magic.Intelligence = Magic.Intelligence.Clone();

            return clone;
        }

        public Item MundaneClone()
        {
            var clone = new Item();
            clone.Attributes = Attributes.ToArray();
            clone.Contents.AddRange(Contents);
            clone.ItemType = ItemType;
            clone.Name = Name;
            clone.BaseNames = BaseNames.ToArray();
            clone.Quantity = Quantity;

            foreach (var trait in Traits)
            {
                clone.Traits.Add(trait);
            }

            return clone;
        }

        public bool NameMatches(string name)
        {
            return Name == name || BaseNames.Contains(name);
        }
    }
}