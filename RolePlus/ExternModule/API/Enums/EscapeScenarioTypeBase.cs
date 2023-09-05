// -----------------------------------------------------------------------
// <copyright file="EscapeScenarioTypeBase.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Enums
{
<<<<<<< HEAD
<<<<<<< HEAD
    using Exiled.API.Features.Core.Generic;
=======
    using RolePlus.ExternModule.API.Engine.Framework.Generic;
>>>>>>> 215601af910e7688328fea59131831fdecbf3e75
=======
    using Exiled.API.Features.Core.Generic;
>>>>>>> d56c47334965da96730d299937b03a57f2fbe373

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
