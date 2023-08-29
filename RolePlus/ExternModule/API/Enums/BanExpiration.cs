// -----------------------------------------------------------------------
// <copyright file="BanExpiration.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Enums
{
    using RolePlus.ExternModule.API.Engine.Framework.Generic;

    /// <summary>
    /// All available ban expirations.
    /// </summary>
    public class BanExpiration : UnmanagedEnumClass<int, BanExpiration>
    {
        /// <summary>
        /// One minute.
        /// </summary>
        public static readonly BanExpiration OneMinute = new(0x3C);

        /// <summary>
        /// Five minutes.
        /// </summary>
        public static readonly BanExpiration FiveMinutes = new(0x12C);

        /// <summary>
        /// Ten minutes.
        /// </summary>
        public static readonly BanExpiration TenMinutes = new(0x258);

        /// <summary>
        /// Fifteen minutes.
        /// </summary>
        public static readonly BanExpiration FifteenMinutes = new(0x384);

        /// <summary>
        /// Twenty minutes.
        /// </summary>
        public static readonly BanExpiration TwentyMinutes = new(0x4B0);

        /// <summary>
        /// Thirty minutes.
        /// </summary>
        public static readonly BanExpiration ThirtyMinutes = new(0x708);

        /// <summary>
        /// Forty minutes.
        /// </summary>
        public static readonly BanExpiration FortyMinutes = new(0x960);

        /// <summary>
        /// Fortyfive minutes.
        /// </summary>
        public static readonly BanExpiration FortyFiveMinutes = new(0xA8C);

        /// <summary>
        /// Fifty minutes.
        /// </summary>
        public static readonly BanExpiration FiftyMinutes = new(0xBB8);

        /// <summary>
        /// One hour.
        /// </summary>
        public static readonly BanExpiration OneHour = new(0xE10);

        /// <summary>
        /// Two hours.
        /// </summary>
        public static readonly BanExpiration TwoHours = new(0x1C20);

        /// <summary>
        /// Three hours.
        /// </summary>
        public static readonly BanExpiration ThreeHours = new(0x2A30);

        /// <summary>
        /// Four hours.
        /// </summary>
        public static readonly BanExpiration FourHours = new(0x3840);

        /// <summary>
        /// Five hours.
        /// </summary>
        public static readonly BanExpiration FiveHours = new(0x4650);

        /// <summary>
        /// Six hours.
        /// </summary>
        public static readonly BanExpiration SixHours = new(0x5460);

        /// <summary>
        /// Twelve hours.
        /// </summary>
        public static readonly BanExpiration TwelveHours = new(0xA8C0);

        /// <summary>
        /// Sixteen minutes.
        /// </summary>
        public static readonly BanExpiration SixteenHours = new(0xE100);

        /// <summary>
        /// One day.
        /// </summary>
        public static readonly BanExpiration OneDay = new(0x15180);

        /// <summary>
        /// Two days.
        /// </summary>
        public static readonly BanExpiration TwoDays = new(0x2A300);

        /// <summary>
        /// Three days.
        /// </summary>
        public static readonly BanExpiration ThreeDays = new(0x3F480);

        /// <summary>
        /// One week.
        /// </summary>
        public static readonly BanExpiration OneWeek = new(0x93A80);

        /// <summary>
        /// One month.
        /// </summary>
        public static readonly BanExpiration OneMonth = new(0x2819A0);

        /// <summary>
        /// One year.
        /// </summary>
        public static readonly BanExpiration OneYear = new(0x1E13380);

        /// <summary>
        /// Permanent.
        /// </summary>
        public static readonly BanExpiration Permanent = new(0x5DFC0F00);

        private BanExpiration(int value) : base(value)
        {
        }
    }
}
