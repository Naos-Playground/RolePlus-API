// -----------------------------------------------------------------------
// <copyright file="ScreenLocation.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Enums
{
    using RolePlus.ExternModule.API.Engine.Core;

    /// <summary>
    /// All available escape scenarios.
    /// </summary>
    public class EscapeScenarioTypeBase : UnmanagedEnumClass<sbyte, EscapeScenarioTypeBase>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EscapeScenarioTypeBase"/> class.
        /// </summary>
        /// <param name="value">The <see cref="sbyte"/> value.</param>
        protected EscapeScenarioTypeBase(sbyte value) : base(value)
        {
        }

        /// <summary>
        /// Represents an invalid scenario.
        /// </summary>
        public static EscapeScenarioTypeBase None { get; } = new(0);
    }
}
