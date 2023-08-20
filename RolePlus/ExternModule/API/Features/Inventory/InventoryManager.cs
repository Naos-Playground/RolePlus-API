// -----------------------------------------------------------------------
// <copyright file="InventoryManager.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.CustomRoles
{
    using System.Collections.Generic;

    using Exiled.API.Enums;

    /// <summary>
    /// A tool to easily handle human <see cref="CustomRole"/> settings.
    /// </summary>
    public class InventoryManager : IInventorySettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryManager"/> class.
        /// </summary>
        public InventoryManager()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryManager"/> class.
        /// </summary>
        /// <param name="inventory">The items to be given.</param>
        /// <param name="customItems">The custom items to be given.</param>
        /// <param name="ammoBox">The ammobox to be set.</param>
        public InventoryManager(
            List<ItemType> inventory,
            List<object> customItems,
            Dictionary<AmmoType, ushort> ammoBox)
        {
            Items = inventory;
            CustomItems = customItems;
            AmmoBox = ammoBox;
        }

        /// <inheritdoc/>
        public List<ItemType> Items { get; set; } = new();

        /// <inheritdoc/>
        public List<object> CustomItems { get; set; } = new();

        /// <inheritdoc/>
        public Dictionary<AmmoType, ushort> AmmoBox { get; set; } = new();

        /// <summary>
        /// Gets or sets the chance of this inventory slot.
        /// <br>[Optional] Useful for inventory tweaks which involve one or more probability value.</br>
        /// </summary>
        public float Chance { get; set; }
    }
}
