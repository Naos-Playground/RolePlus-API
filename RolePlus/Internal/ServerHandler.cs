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

    using MEC;

    internal class ServerHandler
    {
        private CoroutineHandle _decontaminationSurviveHandle;

        internal void OnWaitingForPlayers()
        {
            Server.Host.Role.Type = global::RoleType.Tutorial;

            AudioController.Comms.OnPlayerJoinedSession += AudioController.OnPlayerJoinedSession;
            AudioController.Comms.OnPlayerLeftSession += AudioController.OnPlayerLeftSession;

            Server.Host.ReferenceHub.nicknameSync.Network_myNickSync = "Radio";

            Server.Host.Radio.Network_syncPrimaryVoicechatButton = true;
            Server.Host.DissonanceUserSetup.NetworkspeakingFlags = SpeakingFlags.IntercomAsHuman;
        }

        internal void OnRoundStart()
        {
            _decontaminationSurviveHandle = Timing.RunCoroutine(DecontaminationDamage_Fix());
        }

        internal void OnRoundEnding(EndingRoundEventArgs ev) => ev.IsAllowed = !RoundManager.IsLocked;

        internal void OnRestartingRound()
        {
            Timing.KillCoroutines(_decontaminationSurviveHandle);
            Server.RunCommand("sr", new ConsoleCommandSender());
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
                    pl.Role.Type = global::RoleType.Spectator;

                yield break;
            }
        }
    }
}
