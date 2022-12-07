// -----------------------------------------------------------------------
// <copyright file="CustomTeamAttribute.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.CustomTeams
{
    using System;

    /// <summary>
    /// This attribute determines whether the class which is being applied to should be treated as <see cref="CustomTeam"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomTeamAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomTeamAttribute"/> class.
        /// </summary>
        public CustomTeamAttribute()
        {
        }
    }
}
