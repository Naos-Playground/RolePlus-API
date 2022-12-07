// -----------------------------------------------------------------------
// <copyright file="FVirtualPrivateNetwork.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Engine.Framework.Structs
{
    /// <summary>
    /// A struct to encapsulate VPN data.
    /// </summary>
    public struct FVirtualPrivateNetwork
    {
        /// <summary>
        /// Gets or sets the VPN's IP.
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// Gets or sets the VPN's country code.
        /// </summary>
        public string CountryCode { get; set; }

        /// <summary>
        /// Gets or sets the VPN's country name.
        /// </summary>
        public string CountryName { get; set; }

        /// <summary>
        /// Gets or sets the VPN's ASN.
        /// </summary>
        public int ASN { get; set; }

        /// <summary>
        /// Gets or sets the VPN's ISP.
        /// </summary>
        public string ISP { get; set; }

        /// <summary>
        /// Gets or sets the VPN's block.
        /// </summary>
        public int Block { get; set; }

        /// <summary>
        /// Gets or sets the VPN's hostname.
        /// </summary>
        public string Hostname { get; set; }
    }
}
