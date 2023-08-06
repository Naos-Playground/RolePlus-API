// -----------------------------------------------------------------------
// <copyright file="ServerHandler.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------


namespace RolePlus.Internal
{
    using System.Collections.Generic;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Server;

    using MEC;

    using PlayerRoles;

    internal class ServerHandler
    {
        private CoroutineHandle _decontaminationSurviveHandle;

        internal void OnRoundStart()
        {
            _decontaminationSurviveHandle = Timing.RunCoroutine(DecontaminationDamage_Fix());
        }

        internal void OnRoundEnding(EndingRoundEventArgs ev) => ev.IsRoundEnded = !RoundManager.IsLocked;

        internal void OnRestartingRound()
        {
            Timing.KillCoroutines(_decontaminationSurviveHandle);
            Server.RunCommand("sr", new ServerConsoleSender());
        }

        internal void OnRoundEnded(RoundEndedEventArgs _) => OnRestartingRound();

        private IEnumerator<float> DecontaminationDamage_Fix()
        {
            while (Round.IsStarted)
            {
                yield return Timing.WaitForSeconds(1f);

                if (!Map.IsLczDecontaminated)
                    continue;

                foreach (Player pl in Player.Get(x => x.Zone == ZoneType.LightContainment))
                    pl.Role.Set(RoleTypeId.Spectator);

                yield break;
            }
        }
    }
}
