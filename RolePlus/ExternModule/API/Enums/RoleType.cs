// -----------------------------------------------------------------------
// <copyright file="RoleType.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Enums
{
    using Exiled.API.Features.Core.Generic;
    using PlayerRoles;
<<<<<<< HEAD
=======
    using RolePlus.ExternModule.API.Engine.Framework.Generic;
>>>>>>> 215601af910e7688328fea59131831fdecbf3e75

    /// <summary>
    /// All available roles.
    /// </summary>
    public sealed class RoleType : EnumClass<RoleTypeId, RoleType>
    {
        /// <summary>
        /// Represents an undefined role.
        /// </summary>
        public static readonly RoleType None = new(RoleTypeId.None);

        /// <summary>
        /// Represents the SCP-173 role.
        /// </summary>
        public static readonly RoleType Scp173 = new(RoleTypeId.Scp173);

        /// <summary>
        /// Represents the SCP-106 role.
        /// </summary>
        public static readonly RoleType Scp106 = new(RoleTypeId.Scp106);

        /// <summary>
        /// Represents the SCP-049 role.
        /// </summary>
        public static readonly RoleType Scp049 = new(RoleTypeId.Scp049);

        /// <summary>
        /// Represents the SCP-079 role.
        /// </summary>
        public static readonly RoleType Scp079 = new(RoleTypeId.Scp079);

        /// <summary>
        /// Represents the SCP-096 role.
        /// </summary>
        public static readonly RoleType Scp096 = new(RoleTypeId.Scp096);

        /// <summary>
        /// Represents the SCP-049-2 role.
        /// </summary>
        public static readonly RoleType Scp0492 = new(RoleTypeId.Scp0492);

        /// <summary>
        /// Represents the Scp939 role.
        /// </summary>
        public static readonly RoleType Scp939 = new(RoleTypeId.Scp939);

        /// <summary>
        /// Represents the Scientist role.
        /// </summary>
        public static readonly RoleType Scientist = new(RoleTypeId.Scientist);

        /// <summary>
        /// Represents the Facility Guard role.
        /// </summary>
        public static readonly RoleType FacilityGuard = new(RoleTypeId.FacilityGuard);

        /// <summary>
        /// Represents the NTF Private role.
        /// </summary>
        public static readonly RoleType NtfPrivate = new(RoleTypeId.NtfPrivate);

        /// <summary>
        /// Represents the NTF Specialist role.
        /// </summary>
        public static readonly RoleType NtfSpecialist = new(RoleTypeId.NtfSpecialist);

        /// <summary>
        /// Represents the NTF Sergeant role.
        /// </summary>
        public static readonly RoleType NtfSergeant = new(RoleTypeId.NtfSergeant);

        /// <summary>
        /// Represents the NTF Captain role.
        /// </summary>
        public static readonly RoleType NtfCaptain = new(RoleTypeId.NtfCaptain);

        /// <summary>
        /// Represents the Class-D role.
        /// </summary>
        public static readonly RoleType ClassD = new(RoleTypeId.ClassD);

        /// <summary>
        /// Represents the Chaos Conscript role.
        /// </summary>
        public static readonly RoleType ChaosConscript = new(RoleTypeId.ChaosConscript);

        /// <summary>
        /// Represents the Chaos Rifleman role.
        /// </summary>
        public static readonly RoleType ChaosRifleman = new(RoleTypeId.ChaosRifleman);

        /// <summary>
        /// Represents the Chaos Marauder role.
        /// </summary>
        public static readonly RoleType ChaosMarauder = new(RoleTypeId.ChaosMarauder);

        /// <summary>
        /// Represents the Chaos Repressor role.
        /// </summary>
        public static readonly RoleType ChaosRepressor = new(RoleTypeId.ChaosRepressor);

        /// <summary>
        /// Represents the Tutorial role.
        /// </summary>
        public static readonly RoleType Tutorial = new(RoleTypeId.Tutorial);

        /// <summary>
        /// Represents the Spectator role.
        /// </summary>
        public static readonly RoleType Spectator = new(RoleTypeId.Spectator);

        /// <summary>
        /// Represents the overwatch role.
        /// </summary>
        public static readonly RoleType Overwatch = new(RoleTypeId.Overwatch);

        /// <summary>
        /// Represents the film maker role.
        /// </summary>
        public static readonly RoleType Filmmaker = new(RoleTypeId.Filmmaker);

        /// <summary>
        /// Represents a custom role.
        /// </summary>
        public static readonly RoleType CustomRole = new(RoleTypeId.CustomRole);

        private RoleType(RoleTypeId value) : base(value)
        {
        }
    }
}
