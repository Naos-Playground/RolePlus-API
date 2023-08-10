// -----------------------------------------------------------------------
// <copyright file="CustomRole.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.CustomRoles
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Core;
    using Exiled.Events.EventArgs.Player;
    using MEC;
    using PlayerRoles;
    using RolePlus.ExternModule.API.Enums;
    using RolePlus.ExternModule.Events.EventArgs;
    using RolePlus.Internal;

    using UnityEngine;

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
