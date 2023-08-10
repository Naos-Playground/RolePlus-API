// -----------------------------------------------------------------------
// <copyright file="RoleBuilder.cs" company="NaoUnderscore">
// Copyright (c) NaoUnderscore. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace RolePlus.ExternModule.API.Features.CustomRoles
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Roles;
    using Exiled.Events.EventArgs.Map;
    using Exiled.Events.EventArgs.Player;

    using MEC;

    using RolePlus.ExternModule.API.Engine.Framework.Structs;
    using RolePlus.ExternModule.API.Enums;
    using RolePlus.ExternModule.API.Features.Controllers;
    using RolePlus.ExternModule.API.Features.CustomEscapes;
    using RolePlus.ExternModule.API.Features.CustomSkins;
    using RolePlus.ExternModule.API.Features.CustomTeams;
    using RolePlus.ExternModule.Events.EventArgs;
    using UnityEngine;

    /// <summary>
    /// A tool to easily handle the custom role's logic.
    /// </summary>
    public abstract class RoleBuilder : PlayerBehaviour
    {
        private Vector3 _lastPosition;
        private RoleType _fakeAppearance;
        private bool _isHuman, _wasNoClipPermitted, _nightVisionEnabled, _useCustomEscape, _wasEscaped;

        /// <summary>
        /// Gets a <see cref="HashSet{T}"/> of <see cref="Player"/> containing all players to be spawned without affecting their current position (static).
        /// </summary>
        public static List<Player> StaticPlayers { get; } = new();

        /// <summary>
        /// Gets or sets the <see cref="RoleType"/> of the fake appearance applied by this <see cref="RoleBuilder"/> component.
        /// </summary>
        protected virtual RoleType FakeAppearance
        {
            get => _fakeAppearance;
            set
            {
                _fakeAppearance = value;
                Owner.ChangeAppearance(value, false, 0);
            }
        }

        /// <summary>
        /// Gets or sets the the escape settings.
        /// </summary>
        protected virtual List<EscapeSettings> EscapeSettings { get; set; } = new();

        /// <summary>
        /// Gets or sets the <see cref="RoleSettings"/>.
        /// </summary>
        protected virtual RoleSettings Settings { get; set; }

        /// <summary>
        /// Gets the <see cref="InventoryManager"/>.
        /// </summary>
        protected virtual InventoryManager Inventory { get; }

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="EffectType"/> which should be given to the player.
        /// </summary>
        protected virtual IEnumerable<EffectType> GivenEffects { get; set; }

        /// <summary>
        /// Gets a value indicating whether <see cref="FakeAppearance"/> should be used.
        /// </summary>
        protected virtual bool UseFakeAppearance => false;

        /// <summary>
        /// Gets a value indicating whether an existing spawnpoint should be used.
        /// </summary>
        protected virtual bool UseCustomSpawnpoint => true;

        /// <summary>
        /// Gets or sets a value indicating whether the effects should always be active.
        /// </summary>
        protected virtual bool KeepEffectsAlive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="PlayerBehaviour.Owner"/> has the night vision enabled.
        /// </summary>
        protected virtual bool IsNightVisionEnabled
        {
            get => _nightVisionEnabled;
            set
            {
                _nightVisionEnabled = value;

                // Requires testing
                if (_nightVisionEnabled)
                {
                    foreach (Room room in Room.List)
                    {
                        Owner.SendFakeSyncVar(
                            room.RoomLightControllerNetIdentity,
                            typeof(RoomLightController),
                            nameof(RoomLightController.NetworkLightsEnabled),
                            true);
                    }

                    return;
                }

                foreach (Room _ in Room.List)
                    MirrorExtensions.ResyncSyncVar(Owner.NetworkIdentity, typeof(RoomLightController), nameof(RoomLightController.NetworkLightsEnabled));
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="RoleType"/> of this <see cref="RoleBuilder"/> component.
        /// </summary>
        protected RoleType Role { get; set; }

        /// <summary>
        /// Gets or sets the third person <see cref="CharacterMeshComponent"/>.
        /// </summary>
        protected CharacterMeshComponent ThirdPersonMeshComponent { get; set; }

        /// <summary>
        /// Gets the relative <see cref="CustomRoles.CustomRole"/>.
        /// </summary>
        protected CustomRole CustomRole { get; private set; }

        /// <summary>
        /// Gets the current speed of the  <see cref="PlayerBehaviour.Owner"/>.
        /// </summary>
        protected float CurrentSpeed { get; private set; }

        /// <summary>
        /// Gets the role's configs.
        /// </summary>
        protected virtual object ConfigRaw { get; private set; }

        /// <summary>
        /// Gets a random spawnpoint.
        /// </summary>
        public Vector3 RandomSpawnpoint => Settings.Spawnpoints is null || Settings.Spawnpoints.IsEmpty() ?
            RoleExtensions.GetRandomSpawnLocation(Role).Position :
            Room.Get(Settings.Spawnpoints.ElementAt(UnityEngine.Random.Range(0, Settings.Spawnpoints.Count()))).Position;

        /// <summary>
        /// Gets a value indicating whether the specified <see cref="DamageType"/> is allowed.
        /// </summary>
        /// <param name="damageType">The <see cref="DamageType"/> to check.</param>
        /// <returns><see langword="true"/> if the specified <see cref="DamageType"/> is allowed; otherwise, <see langword="false"/>.</returns>
        public bool IsAllowed(DamageType damageType) => Settings.AllowedDamageTypes.Contains(damageType);

        /// <summary>
        /// Gets a value indicating whether the specified <see cref="DamageType"/> is ignored.
        /// </summary>
        /// <param name="damageType">The <see cref="DamageType"/> to check.</param>
        /// <returns><see langword="true"/> if the specified <see cref="DamageType"/> is ignored; otherwise, <see langword="false"/>.</returns>
        public bool IsIgnored(DamageType damageType) => Settings.IgnoredDamageTypes.Contains(damageType);

        /// <summary>
        /// Loads the given config.
        /// </summary>
        /// <param name="config">The config load.</param>
        protected virtual void LoadConfigs(object config)
        {
            if (config is null)
                return;

            foreach (PropertyInfo propertyInfo in config.GetType().GetProperties())
            {
                PropertyInfo targetInfo = typeof(RoleSettings).GetProperty(propertyInfo.Name) ?? typeof(InventoryManager).GetProperty(propertyInfo.Name);
                if (targetInfo is null)
                    continue;

                targetInfo.SetValue(targetInfo.DeclaringType == typeof(RoleSettings) ? Settings : Inventory, propertyInfo.GetValue(config, null));
            }
        }

        /// <inheritdoc/>
        protected virtual void SetupThirdPersonMesh(string meshName, string objectName, FTransform transform = default, float fixedTickRate = 0.01f)
        {
            ThirdPersonMeshComponent = CreateDefaultSubobject<CharacterMeshComponent>(Owner.GameObject, $"{Name}-TPM", Owner.GameObject, meshName);
            ThirdPersonMeshComponent.Socket = transform;
            ThirdPersonMeshComponent.FixedTickRate = fixedTickRate;
            Owner.EnableEffect(EffectType.Invisible);
        }

        /// <inheritdoc/>
        protected virtual void DestroyThirdPersonMesh()
        {
            if (ThirdPersonMeshComponent is null)
                return;

            ThirdPersonMeshComponent.Destroy();
        }

        /// <summary>
        /// Fired on <see cref="PostInitialize"/>.
        /// <para>Settings should be adjusted here.</para>
        /// </summary>
        protected virtual void AdjustSettings()
        {
        }

        /// <inheritdoc/>
        protected override void PostInitialize()
        {
            base.PostInitialize();

            LoadConfigs(ConfigRaw);

            if (CustomRole.TryGet(this, out CustomRole customRole))
            {
                CustomRole = customRole;
                Settings = CustomRole.Settings;

                if (CustomRole.EscapeBuilderComponent is not null)
                {
                    Owner.AddComponent(CustomRole.EscapeBuilderComponent);
                    _useCustomEscape = true;
                }
            }

            AdjustSettings();

            if (IsNightVisionEnabled)
                IsNightVisionEnabled = true;

            _wasNoClipPermitted = Owner.IsNoclipPermitted;
            _isHuman = !CustomRole.IsScp;

            if (!Settings.IsRoleDynamic)
                Role = CustomRole.Role;

            if (Owner.Role != Role)
                Owner.SetRole(Role);

            if (!Settings.DoesLookingAffectScp173)
                Scp173Role.TurnedPlayers.Add(Owner);

            if (!Settings.DoesLookingAffectScp096)
                Scp096Role.TurnedPlayers.Add(Owner);

            if (!string.IsNullOrEmpty(Settings.RankName) || !string.IsNullOrEmpty(Settings.RankColor))
                Features.Badge.Load(Owner, Settings.RankName, Settings.RankColor);

            if (Settings.CustomInfo != string.Empty)
                Owner.CustomInfo += Settings.CustomInfo;

            if (Settings.HideInfoArea)
            {
                Owner.InfoArea &= ~PlayerInfoArea.UnitName;
                Owner.InfoArea &= ~PlayerInfoArea.Role;
            }

            Owner.Scale *= Settings.Scale;
            Owner.Health = Settings.Health;
            Owner.MaxHealth = Settings.MaxHealth;
            Owner.MaxArtificialHealth = Settings.MaxArtificialHealth;

            if (_isHuman)
            {
                Owner.AddItem(Inventory.CustomItems);
                Owner.AddItem(Inventory.Items);

                foreach (KeyValuePair<AmmoType, ushort> kvp in Inventory.AmmoBox)
                    Owner.AddAmmo(kvp.Key, kvp.Value);
            }


            if (Settings.Broadcast is not null)
                Owner.Broadcast(Settings.Broadcast, true);

        }


        /// <inheritdoc/>
        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            if (Owner is null)
            {
                Destroy();
                return;
            }

            if (Owner.Role.Cast(out FpcRole fpcRole))
                fpcRole.IsNoclipEnabled = false;

            if (Settings.ArtificialHealth > 0f)
                Owner.AddAhp(Settings.ArtificialHealth, Owner.MaxArtificialHealth, 0, 1, 0);

            if (UseFakeAppearance)
                Owner.ChangeAppearance(FakeAppearance, false);

            if (GivenEffects is not null && !GivenEffects.IsEmpty())
                Owner.EnableEffects(GivenEffects);
        }

        /// <inheritdoc/>
        protected override void Tick()
        {
            base.Tick();

            // Must be refactored (performance issues)
            if (Owner is null || (Settings.UseDefaultRoleOnly && (Owner.Role != Role)) ||
                (!Settings.AllowedRoles.IsEmpty() && !Settings.AllowedRoles.Contains(RoleType.Cast(Owner.Role))) ||
                !CustomRole.Players.Contains(Owner))
            {
                Destroy();
                return;
            }

            if (KeepEffectsAlive)
            {
                foreach (EffectType effect in GivenEffects)
                {
                    if (!Owner.GetEffect(effect).IsEnabled)
                        Owner.EnableEffect(effect);
                }
            }

            if (!_useCustomEscape && !_wasEscaped)
            {
                foreach (EscapeSettings settings in EscapeSettings)
                {
                    if (!settings.IsAllowed || Vector3.Distance(Owner.Position, settings.Position) > settings.MaxDistanceTolerance)
                        continue;

                    ProcessEscapeAction(settings);
                    _wasEscaped = true;
                    break;
                }
            }

            CurrentSpeed = (Owner.Position - _lastPosition).magnitude;
            _lastPosition = Owner.Position;
        }

        /// <inheritdoc/>
        protected override void OnEndPlay()
        {
            base.OnEndPlay();

            CustomRole.PlayersValue.Remove(Owner);

            DestroyThirdPersonMesh();

            if (!Settings.DoesLookingAffectScp173)
                Scp173Role.TurnedPlayers.Remove(Owner);

            if (!Settings.DoesLookingAffectScp096)
                Scp096Role.TurnedPlayers.Remove(Owner);

            if (!string.IsNullOrEmpty(Settings.RankName) || !string.IsNullOrEmpty(Settings.RankColor))
                Features.Badge.Unload(Owner);

            if (!string.IsNullOrEmpty(Settings.CustomInfo))
                Owner.CustomInfo = null;

            if (Settings.HideInfoArea)
            {
                Owner.InfoArea |= PlayerInfoArea.UnitName;
                Owner.InfoArea |= PlayerInfoArea.Role;
            }

            Owner.Scale = Vector3.one;
            Owner.IsNoclipPermitted = _wasNoClipPermitted;
            Owner.IsUsingStamina = true;
            Owner.DisableAllEffects();
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            Exiled.Events.Handlers.Player.ChangingItem += ChangingItemBehvaior;
            Exiled.Events.Handlers.Player.Destroying += DestroyOnLeft;
            Exiled.Events.Handlers.Player.ChangingRole += DestroyOnChangingRole;
            Exiled.Events.Handlers.Player.Escaping += PreventPlayerFromEscaping;
            Exiled.Events.Handlers.Player.SearchingPickup += PickingUpItemBehavior;
            Exiled.Events.Handlers.Player.Died += AnnounceOwnerDeath;
            Exiled.Events.Handlers.Player.Hurting += PreventDealingDamageToScps;
            Exiled.Events.Handlers.Player.Hurting += PreventTakingDamageFromScps;
            Exiled.Events.Handlers.Player.Hurting += IgnoreDamage;
            Exiled.Events.Handlers.Player.InteractingDoor += CheckpointsBehavior;
            Exiled.Events.Handlers.Player.InteractingDoor += InteractingDoorBehavior;
            Exiled.Events.Handlers.Player.IntercomSpeaking += IntercomSpeakingBehavior;
            Exiled.Events.Handlers.Player.VoiceChatting += VoiceChattingBehavior;
            Exiled.Events.Handlers.Player.ActivatingWarheadPanel += ActivatingWarheadBehavior;
            Exiled.Events.Handlers.Player.ActivatingWorkstation += ActivatingWorkstationBehavior;
            Exiled.Events.Handlers.Player.ActivatingGenerator += ActivatingGeneratorBehavior;
            Exiled.Events.Handlers.Player.InteractingElevator += InteractingElevatorBehavior;
            Exiled.Events.Handlers.Player.DroppingItem += DroppingItemBehavior;
            Exiled.Events.Handlers.Player.Handcuffing += HandcuffingBehavior;
            Exiled.Events.Handlers.Map.PlacingBlood += PlacingBloodBehavior;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            Exiled.Events.Handlers.Player.ChangingItem -= ChangingItemBehvaior;
            Exiled.Events.Handlers.Player.Destroying -= DestroyOnLeft;
            Exiled.Events.Handlers.Player.ChangingRole -= DestroyOnChangingRole;
            Exiled.Events.Handlers.Player.Escaping -= PreventPlayerFromEscaping;
            Exiled.Events.Handlers.Player.SearchingPickup -= PickingUpItemBehavior;
            Exiled.Events.Handlers.Player.Died -= AnnounceOwnerDeath;
            Exiled.Events.Handlers.Player.Hurting -= PreventDealingDamageToScps;
            Exiled.Events.Handlers.Player.Hurting -= PreventTakingDamageFromScps;
            Exiled.Events.Handlers.Player.Hurting -= IgnoreDamage;
            Exiled.Events.Handlers.Player.InteractingDoor -= CheckpointsBehavior;
            Exiled.Events.Handlers.Player.InteractingDoor -= InteractingDoorBehavior;
            Exiled.Events.Handlers.Player.IntercomSpeaking -= IntercomSpeakingBehavior;
            Exiled.Events.Handlers.Player.VoiceChatting -= VoiceChattingBehavior;
            Exiled.Events.Handlers.Player.ActivatingWarheadPanel -= ActivatingWarheadBehavior;
            Exiled.Events.Handlers.Player.ActivatingWorkstation -= ActivatingWorkstationBehavior;
            Exiled.Events.Handlers.Player.ActivatingGenerator -= ActivatingGeneratorBehavior;
            Exiled.Events.Handlers.Player.InteractingElevator -= InteractingElevatorBehavior;
            Exiled.Events.Handlers.Player.DroppingItem -= DroppingItemBehavior;
            Exiled.Events.Handlers.Player.Handcuffing -= HandcuffingBehavior;
            Exiled.Events.Handlers.Map.PlacingBlood -= PlacingBloodBehavior;
        }

        /// <summary>
        /// Tries to get a valid target based on a specified condition.
        /// </summary>
        /// <param name="predicate">The condition.</param>
        /// <param name="distance">The maximum distance to reach.</param>
        /// <param name="target">The valid target.</param>
        /// <returns><see langword="true"/> if the target was found; otherwise, <see langword="false"/>.</returns>
        private protected virtual bool TryGetValidTarget(Func<Player, bool> predicate, float distance, out Player target)
        {
            List<Player> targets = new();
            foreach (Player pl in Player.Get(predicate))
            {
                if (Vector3.Distance(pl.Position, Owner.Position) <= distance)
                    targets.Add(pl);
            }

            target = targets.FirstOrDefault();
            return target is not null;
        }

        /// <summary>
        /// Tries to get a valid target based on a specified condition.
        /// </summary>
        /// <param name="predicate">The condition.</param>
        /// <param name="distance">The maximum distance to reach.</param>
        /// <param name="players">The valid targets.</param>
        /// <returns><see langword="true"/> if targets were found; otherwise, <see langword="false"/>.</returns>
        private protected virtual bool TryGetValidTargets(Func<Player, bool> predicate, float distance, out List<Player> players)
        {
            players = new();
            foreach (Player pl in Player.Get(predicate))
            {
                if (Vector3.Distance(pl.Position, Owner.Position) <= distance)
                    players.Add(pl);
            }

            return players.Any();
        }

        /// <see cref="Exiled.Events.Handlers.Player.OnHurting(HurtingEventArgs)"/>
        private protected virtual void PreventTakingDamageFromScps(HurtingEventArgs ev)
        {
            if (!Check(ev.Player) || ev.Attacker is null ||
                !ev.Attacker.IsScp || Settings.CanBeHurtByScps)
                return;

            ev.IsAllowed = false;
        }

        /// <see cref="Exiled.Events.Handlers.Player.OnHurting(HurtingEventArgs)"/>
        private protected virtual void IgnoreDamage(HurtingEventArgs ev)
        {
            if (!Check(ev.Player) || !Settings.AllowedDamageTypes.IsEmpty() ||
                !Settings.IgnoredDamageTypes.Contains(ev.DamageHandler.Type))
                return;

            ev.Amount = 0;
            ev.IsAllowed = false;
        }

        /// <see cref="Exiled.Events.Handlers.Player.OnChangingItem(ChangingItemEventArgs)"/>
        private protected virtual void ChangingItemBehvaior(ChangingItemEventArgs ev)
        {
            if (!Check(ev.Player) || Settings.CanSelectItems)
                return;

            ev.IsAllowed = false;
        }

        /// <see cref="Exiled.Events.Handlers.Player.OnHurting(HurtingEventArgs)"/>
        private protected virtual void AllowDamage(HurtingEventArgs ev)
        {
            if (!Check(ev.Player) || !Settings.IgnoredDamageTypes.IsEmpty() ||
                !Settings.AllowedDamageTypes.Contains(ev.DamageHandler.Type))
                return;

            ev.IsAllowed = false;
        }

        /// <see cref="Exiled.Events.Handlers.Player.OnHurting(HurtingEventArgs)"/>
        private protected virtual void PreventDealingDamageToScps(HurtingEventArgs ev)
        {
            if (!Check(ev.Attacker) || Settings.CanHurtScps)
                return;

            ev.IsAllowed = false;
        }

        /// <see cref="Exiled.Events.Handlers.Player.OnDestroying(DestroyingEventArgs)"/>
        private protected virtual void DestroyOnLeft(DestroyingEventArgs ev)
        {
            if (!Check(ev.Player))
                return;

            Destroy();
        }

        /// <see cref="Exiled.Events.Handlers.Player.OnHandcuffing(HandcuffingEventArgs)"/>
        private protected virtual void HandcuffingBehavior(HandcuffingEventArgs ev)
        {
            if (!Check(ev.Target) || Settings.CanBeHandcuffed)
                return;

            ev.IsAllowed = false;
        }

        /// <see cref="Exiled.Events.Handlers.Player.OnChangingGroup(ChangingGroupEventArgs)"/>
        private protected virtual void DestroyOnChangingRole(ChangingRoleEventArgs ev)
        {
            if (!Check(ev.Player) || Settings.AllowedRoles.Contains(RoleType.Cast(ev.NewRole)))
                return;

            Destroy();
        }

        /// <see cref="Exiled.Events.Handlers.Player.OnSearchPickupRequest(SearchingPickupEventArgs)"/>
        private protected virtual void PickingUpItemBehavior(SearchingPickupEventArgs ev)
        {
            if (!Check(ev.Player) || Settings.CanPickupItems)
                return;

            ev.IsAllowed = false;
        }

        /// <see cref="Exiled.Events.Handlers.Player.OnEscaping(Exiled.Events.EventArgs.Player.EscapingEventArgs)"/>
        private protected virtual void PreventPlayerFromEscaping(Exiled.Events.EventArgs.Player.EscapingEventArgs ev)
        {
            if (!Check(ev.Player) || _useCustomEscape || _wasEscaped || !EscapeSettings.IsEmpty())
                return;

            ev.IsAllowed = false;
        }

        /// <see cref="Exiled.Events.Handlers.Player.OnDied(DiedEventArgs)"/>
        private protected virtual void AnnounceOwnerDeath(DiedEventArgs ev)
        {
            if (!Check(ev.Player) || Check(ev.Attacker) || Check(Server.Host) || !Settings.IsDeathAnnouncementEnabled)
                return;

            string announcement = string.Empty;
            if (ev.Attacker is null)
                goto Announce;

            if (CustomRole.TryGet(ev.Attacker, out CustomRole customRole))
            {
                if (CustomTeam.TryGet<CustomTeam>(customRole, out CustomTeam customTeam) &&
                    Settings.KilledByCustomTeamAnnouncements.TryGetValue(customTeam.Id, out announcement))
                    goto Announce;
                else if (Settings.KilledByCustomRoleAnnouncements.TryGetValue(customRole.Id, out announcement))
                    goto Announce;
            }
            else
            {
                if (Settings.KilledByRoleAnnouncements.TryGetValue(RoleType.Cast(ev.Attacker.Role), out announcement))
                    goto Announce;
            }

            return;
        Announce:
            if (string.IsNullOrEmpty(announcement))
                announcement = Settings.UnknownTerminationCauseAnnouncement;

            if (!string.IsNullOrEmpty(announcement))
                Cassie.Message(announcement);
        }

        /// <see cref="Exiled.Events.Handlers.Player.OnIntercomSpeaking(IntercomSpeakingEventArgs)"/>
        private protected virtual void IntercomSpeakingBehavior(IntercomSpeakingEventArgs ev)
        {
            if (!Check(ev.Player) || Settings.CanUseIntercom)
                return;

            ev.IsAllowed = false;
        }

        /// <see cref="Exiled.Events.Handlers.Player.OnEnteringPocketDimension(EnteringPocketDimensionEventArgs)"/>
        private protected virtual void EnteringPocketDimensionBehavior(EnteringPocketDimensionEventArgs ev)
        {
            if (!Check(ev.Player) || Settings.CanEnterPocketDimension)
                return;

            ev.IsAllowed = false;
        }

        /// <see cref="Exiled.Events.Handlers.Player.OnVoiceChatting(VoiceChattingEventArgs)"/>
        private protected virtual void VoiceChattingBehavior(VoiceChattingEventArgs ev)
        {
            if (!Check(ev.Player) || Settings.CanUseVoiceChat)
                return;

            ev.IsAllowed = false;
        }

        /// <see cref="Exiled.Events.Handlers.Map.OnPlacingBlood(PlacingBloodEventArgs)"/>
        private protected virtual void PlacingBloodBehavior(PlacingBloodEventArgs ev)
        {
            if (!Check(ev.Player) || Settings.CanPlaceBlood)
                return;

            ev.IsAllowed = false;
        }

        /// <see cref="Exiled.Events.Handlers.Player.OnEnteringPocketDimension(EnteringPocketDimensionEventArgs)"/>
        private protected virtual void DroppingItemBehavior(DroppingItemEventArgs ev)
        {
            if (!Check(ev.Player) || Settings.CanDropItems)
                return;

            ev.IsAllowed = false;
        }

        /// <see cref="Exiled.Events.Handlers.Player.OnActivatingWarheadPanel(ActivatingWarheadPanelEventArgs)"/>
        private protected virtual void ActivatingWarheadBehavior(ActivatingWarheadPanelEventArgs ev)
        {
            if (!Check(ev.Player) || Settings.CanActivateWarhead)
                return;

            ev.IsAllowed = false;
        }

        /// <see cref="Exiled.Events.Handlers.Player.OnActivatingGenerator(ActivatingGeneratorEventArgs)"/>
        private protected virtual void ActivatingGeneratorBehavior(ActivatingGeneratorEventArgs ev)
        {
            if (!Check(ev.Player) || Settings.CanActivateGenerators)
                return;

            ev.IsAllowed = false;
        }

        /// <see cref="Exiled.Events.Handlers.Player.OnActivatingWorkstation(ActivatingWorkstationEventArgs)"/>
        private protected virtual void ActivatingWorkstationBehavior(ActivatingWorkstationEventArgs ev)
        {
            if (!Check(ev.Player) || Settings.CanActivateWorkstations)
                return;

            ev.IsAllowed = false;
        }

        /// <see cref="Exiled.Events.Handlers.Player.OnInteractingElevator(InteractingElevatorEventArgs)"/>
        private protected virtual void InteractingElevatorBehavior(InteractingElevatorEventArgs ev)
        {
            if (!Check(ev.Player) || Settings.CanUseElevators)
                return;

            ev.IsAllowed = false;
        }

        /// <see cref="Exiled.Events.Handlers.Player.OnInteractingDoor(InteractingDoorEventArgs)"/>
        private protected virtual void CheckpointsBehavior(InteractingDoorEventArgs ev)
        {
            if (!Check(ev.Player) || !Settings.CanBypassCheckpoints || !ev.Door.IsCheckpoint)
                return;

            ev.IsAllowed = true;
        }

        /// <see cref="Exiled.Events.Handlers.Player.OnInteractingDoor(InteractingDoorEventArgs)"/>
        private protected virtual void InteractingDoorBehavior(InteractingDoorEventArgs ev)
        {
            if (!Check(ev.Player) || !Settings.BypassableDoors.Contains(ev.Door.Type))
                return;

            ev.IsAllowed = false;
        }

        private void ProcessEscapeAction(EscapeSettings settings)
        {
            Events.EventArgs.EscapingEventArgs escaping = new(Owner, settings.Role, settings.CustomRole, EscapeScenarioTypeBase.None, default);
            Events.Handlers.Player.OnEscaping(escaping);

            if (!escaping.IsAllowed)
                return;

            escaping.Player.SetRole(escaping.NewRole != RoleType.None ? escaping.NewRole : escaping.NewCustomRole);
            escaping.Hint.Show(escaping.Player);

            EscapedEventArgs escaped = new(escaping.Player);
            Events.Handlers.Player.OnEscaped(escaped);
        }
    }
}
