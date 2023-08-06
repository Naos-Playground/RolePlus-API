// -----------------------------------------------------------------------
// <copyright file="ScreenLocation.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Enums
{
    using PlayerRoles;
    using RolePlus.ExternModule.API.Engine.Core;

    /// <summary>
    /// All available roles.
    /// </summary>
    public sealed class RoleType : EnumClass<RoleTypeId, RoleType>
    {
        private RoleType(RoleTypeId value) : base(value) { }

        /// <summary>
        /// Represents an undefined role.
        /// </summary>
        public static RoleType None { get; } = new(RoleTypeId.None);

        /// <summary>
        /// Represents the SCP-173 role.
        /// </summary>
        public static RoleType Scp173 { get; } = new(RoleTypeId.Scp173);

        /// <summary>
        /// Represents the SCP-106 role.
        /// </summary>
        public static RoleType Scp106 { get; } = new(RoleTypeId.Scp106);

        /// <summary>
        /// Represents the SCP-049 role.
        /// </summary>
        public static RoleType Scp049 { get; } = new(RoleTypeId.Scp049);

        /// <summary>
        /// Represents the SCP-079 role.
        /// </summary>
        public static RoleType Scp079 { get; } = new(RoleTypeId.Scp079);

        /// <summary>
        /// Represents the SCP-096 role.
        /// </summary>
        public static RoleType Scp096 { get; } = new(RoleTypeId.Scp096);

        /// <summary>
        /// Represents the SCP-049-2 role.
        /// </summary>
        public static RoleType Scp0492 { get; } = new(RoleTypeId.Scp0492);

        /// <summary>
        /// Represents the Scp939 role.
        /// </summary>
        public static RoleType Scp939 { get; } = new(RoleTypeId.Scp939);

        /// <summary>
        /// Represents the Scientist role.
        /// </summary>
        public static RoleType Scientist { get; } = new(RoleTypeId.Scientist);

        /// <summary>
        /// Represents the Facility Guard role.
        /// </summary>
        public static RoleType FacilityGuard { get; } = new(RoleTypeId.FacilityGuard);

        /// <summary>
        /// Represents the NTF Private role.
        /// </summary>
        public static RoleType NtfPrivate { get; } = new(RoleTypeId.NtfPrivate);

        /// <summary>
        /// Represents the NTF Specialist role.
        /// </summary>
        public static RoleType NtfSpecialist { get; } = new(RoleTypeId.NtfSpecialist);

        /// <summary>
        /// Represents the NTF Sergeant role.
        /// </summary>
        public static RoleType NtfSergeant { get; } = new(RoleTypeId.NtfSergeant);

        /// <summary>
        /// Represents the NTF Captain role.
        /// </summary>
        public static RoleType NtfCaptain { get; } = new(RoleTypeId.NtfCaptain);

        /// <summary>
        /// Represents the Class-D role.
        /// </summary>
        public static RoleType ClassD { get; } = new(RoleTypeId.ClassD);

        /// <summary>
        /// Represents the Chaos Conscript role.
        /// </summary>
        public static RoleType ChaosConscript { get; } = new(RoleTypeId.ChaosConscript);

        /// <summary>
        /// Represents the Chaos Rifleman role.
        /// </summary>
        public static RoleType ChaosRifleman { get; } = new(RoleTypeId.ChaosRifleman);

        /// <summary>
        /// Represents the Chaos Marauder role.
        /// </summary>
        public static RoleType ChaosMarauder { get; } = new(RoleTypeId.ChaosMarauder);

        /// <summary>
        /// Represents the Chaos Repressor role.
        /// </summary>
        public static RoleType ChaosRepressor { get; } = new(RoleTypeId.ChaosRepressor);

        /// <summary>
        /// Represents the Tutorial role.
        /// </summary>
        public static RoleType Tutorial { get; } = new(RoleTypeId.Tutorial);

        /// <summary>
        /// Represents the Spectator role.
        /// </summary>
        public static RoleType Spectator { get; } = new(RoleTypeId.Spectator);

        /// <summary>
        /// Represents the overwatch role.
        /// </summary>
        public static RoleType Overwatch { get; } = new(RoleTypeId.Overwatch);

        /// <summary>
        /// Represents the film maker role.
        /// </summary>
        public static RoleType Filmmaker { get; } = new(RoleTypeId.Filmmaker);

        /// <summary>
        /// Represents a Custom role.
        /// </summary>
        public static RoleType CustomRole { get; } = new(RoleTypeId.CustomRole);

    }
}
