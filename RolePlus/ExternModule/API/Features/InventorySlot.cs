// -----------------------------------------------------------------------
// <copyright file="InventorySlot.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features
{
    using System.Collections.Generic;

    using Exiled.API.Enums;

    /// <summary>
    /// A class to easily create inventory presets.
    /// </summary>
    public sealed class InventorySlot
    {
        /// <summary>
        /// Gets or sets a <see cref="IEnumerable{T}"/> of <see cref="ItemType"/> which contains all the items of this slot.
        /// </summary>
        public List<ItemType> Items { get; set; } = new();

        /// <summary>
        /// Gets or sets a <see cref="IEnumerable{T}"/> of <see cref="string"/> which contains all the custom items of this slot.
        /// </summary>
        public List<object> CustomItems { get; set; } = new();

        /// <summary>
        /// Gets or sets a <see cref="Dictionary{TKey, TValue}"/> of <see cref="AmmoType"/> and <see cref="ushort"/> which contains all the ammo types and amounts of this slot.
        /// </summary>
        public Dictionary<AmmoType, ushort> Ammo { get; set; } = new();

        /// <summary>
        /// Gets or sets the chance of this inventory slot.
        /// <br>[Optional] Useful for inventory tweaks which involve one or more probability value.</br>
        /// </summary>
        public float Chance { get; set; }
    }
}
