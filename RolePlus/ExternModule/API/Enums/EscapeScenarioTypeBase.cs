// -----------------------------------------------------------------------
// <copyright file="EscapeScenarioTypeBase.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Enums
{
    using Exiled.API.Features.Core.Generic;

    /// <summary>
    /// All available escape scenarios.
    /// </summary>
    public class EscapeScenarioTypeBase : UnmanagedEnumClass<sbyte, EscapeScenarioTypeBase>
    {
        /// <summary>
        /// Represents an invalid scenario.
        /// </summary>
        public static readonly EscapeScenarioTypeBase None = new(0);

        /// <summary>
        /// Initializes a new instance of the <see cref="EscapeScenarioTypeBase"/> class.
        /// </summary>
        /// <param name="value">The <see cref="sbyte"/> value.</param>
        protected EscapeScenarioTypeBase(sbyte value) : base(value)
        {
        }
    }
}
