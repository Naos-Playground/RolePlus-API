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
    using Exiled.Events.EventArgs;

    using MEC;

    using RolePlus.ExternModule.API.Engine.Core;
    using RolePlus.ExternModule.API.Engine.Framework.Structs;
    using RolePlus.ExternModule.API.Features.CustomSkins;
    using RolePlus.ExternModule.API.Features.CustomTeams;

    using UnityEngine;

    using static RolePlus.ExternModule.API.Features.CustomRoles.CustomRole.Info;

    /// <summary>
    /// A tool to easily handle the custom role's logic.
    /// </summary>
    public abstract class RoleBuilder : MonoBehaviour
    {
        private RoleType _fakeAppearance;
        private bool _canEscape;
        private bool _useEscapeRole;
        private bool _useEscapeCustomRole;
        private bool _isHuman;
        private RoleType _escapeRole;
        private uint _escapeCustomRole;
        private CoroutineHandle _escapeHandle;
        private CoroutineHandle _nightVisionHandle;
        private Vector3 _lastPosition = Vector3.zero;
        private bool _noClipEnabled;
        private bool _nightVisionEnabled;

        /// <summary>
        /// Gets a <see cref="HashSet{T}"/> of <see cref="Player"/> containing all the players to be spawned keeping the same position.
        /// </summary>
        public static List<Player> LitePlayers { get; } = new();

        /// <summary>
        /// Gets or sets <see cref="Player"/> who owns this <see cref="RoleBuilder"/> component.
        /// </summary>
        public abstract Player Owner { get; protected set; }

        /// <summary>
        /// Gets or sets the <see cref="RoleType"/> of the fake appearance applied by this <see cref="RoleBuilder"/> component.
        /// </summary>
        protected virtual RoleType FakeAppearance
        {
            get => _fakeAppearance;
            set
            {
                _fakeAppearance = value;
                Owner.ChangeAppearance(value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="FakeAppearance"/> should be used.
        /// </summary>
        protected virtual bool UseFakeAppearance { get; }

        /// <summary>
        /// Gets a value indicating whether the spawn broadcast should be shown.
        /// </summary>
        protected virtual bool ShowBroadcast => true;

        /// <summary>
        /// Gets a value indicating whether the player can look at Scp173.
        /// </summary>
        protected virtual bool IgnoreScp173 { get; }

        /// <summary>
        /// Gets a value indicating whether the player can trigger Scp096.
        /// </summary>
        protected virtual bool IgnoreScp096 { get; }

        /// <summary>
        /// Gets the player's rank name.
        /// </summary>
        protected virtual string RankName { get; }

        /// <summary>
        /// Gets the player's rank color.
        /// </summary>
        protected virtual string RankColor { get; }

        /// <summary>
        /// Gets the player's custom info.
        /// </summary>
        protected virtual string CustomInfo => string.Empty;

        /// <summary>
        /// Gets a value indicating whether the player's <see cref="PlayerInfoArea"/> should be hidden.
        /// </summary>
        protected virtual bool HideInfoArea { get; }

        /// <summary>
        /// Gets a value indicating whether the existing spawnpoint should be used.
        /// </summary>
        protected virtual bool UseCustomSpawnpoint => true;

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> of <see cref="EffectType"/> which should be given to the player.
        /// </summary>
        protected virtual IEnumerable<EffectType> GivenEffects { get; }

        /// <summary>
        /// Gets a value indicating whether the player's role should use the specified <see cref="Role"/> only.
        /// </summary>
        protected virtual bool UseDefaultRoleOnly => true;

        /// <summary>
        /// Gets a value indicating whether the effects should be kept active.
        /// </summary>
        protected virtual bool KeepEffectsActive { get; }

        /// <summary>
        /// Gets the <see cref="CustomRole.Info.InventoryManager"/>.
        /// </summary>
        protected virtual CustomRole.Info.InventoryManager InventoryInfo { get; } = new();

        /// <summary>
        /// Gets the <see cref="CustomRole"/>.<see cref="CustomRole.Info"/>.
        /// </summary>
        protected virtual CustomRole.Info Info { get; } = new();

        /// <summary>
        /// Gets a <see cref="RoleType"/>[] containing all the allowed roles.
        /// </summary>
        protected virtual RoleType[] AllowedRoles { get; } = new RoleType[] { };

        /// <summary>
        /// Gets a <see cref="DamageType"/>[] containing all the ignored damage types.
        /// </summary>
        protected virtual DamageType[] IgnoredDamageTypes { get; } = new DamageType[] { };

        /// <summary>
        /// Gets a <see cref="DamageType"/>[] containing all the allowed damage types.
        /// </summary>
        protected virtual DamageType[] AllowedDamageTypes { get; } = new DamageType[] { };

        /// <summary>
        /// Gets or sets a value indicating whether the player can pickup items.
        /// </summary>
        protected virtual bool CanPickupItems { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the player's role is dynamic.
        /// </summary>
        protected virtual bool IsRoleDynamic { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Owner"/> can hurt SCPs.
        /// </summary>
        protected virtual bool CanHurtScps { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Owner"/> can be hurted by SCPs.
        /// </summary>
        protected virtual bool CanBeHurtedByScps { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Owner"/> can enter pocket dimension.
        /// </summary>
        protected virtual bool CanEnterPocketDimension { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Owner"/> can use intercom.
        /// </summary>
        protected virtual bool CanUseIntercom { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Owner"/> can voicechat.
        /// </summary>
        protected virtual bool CanUseVoiceChat { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Owner"/> can place blood.
        /// </summary>
        protected virtual bool CanPlaceBlood { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Owner"/> can activate warhead.
        /// </summary>
        protected virtual bool CanActivateWarhead { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Owner"/> can drop items.
        /// </summary>
        protected virtual bool CanDropItems { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Owner"/> can be handcuffed.
        /// </summary>
        protected virtual bool CanBeHandcuffed { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Owner"/> can use elevators.
        /// </summary>
        protected virtual bool CanUseElevators { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Owner"/> can bypass checkpoints.
        /// </summary>
        protected virtual bool CanBypassCheckpoints { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Owner"/> can activate workstations.
        /// </summary>
        protected virtual bool CanActivateWorkstations { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Owner"/> can contain SCP-106.
        /// </summary>
        protected virtual bool CanContainScp106 { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Owner"/> can activate generators.
        /// </summary>
        protected virtual bool CanActivateGenerators { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Owner"/> can enter femur breaker.
        /// </summary>
        protected virtual bool CanEnterFemurBreaker { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Owner"/> can change items from its inventory.
        /// </summary>
        protected virtual bool CanChangeItems { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="Owner"/> has the night vision enabled.
        /// </summary>
        protected virtual bool IsNightVisionEnabled
        {
            get => _nightVisionEnabled;
            set
            {
                _nightVisionEnabled = value;

                if (_nightVisionEnabled)
                    _nightVisionHandle = Timing.RunCoroutine(NightVision());
                else
                    Timing.KillCoroutines(_nightVisionHandle);
            }
        }

        /// <summary>
        /// Gets or sets a <see cref="DoorType"/>[] containing all the bypassable doors.
        /// </summary>
        protected virtual DoorType[] BypassableDoors { get; set; } = new DoorType[] { };

        /// <summary>
        /// Gets a value indicating whether the C.A.S.S.I.E death announcement can be played when the <see cref="Owner"/> dies.
        /// </summary>
        protected virtual bool IsDeathAnnouncementEnabled => false;

        /// <summary>
        /// Gets the C.A.S.S.I.E announcement to be played when the <see cref="Owner"/> dies from an unhandled or unknown termination cause.
        /// </summary>
        protected virtual string UnknownTerminationCauseAnnouncement => string.Empty;

        /// <summary>
        /// Gets or sets a <see cref="Dictionary{TKey, TValue}"/> containing all the C.A.S.S.I.E announcements
        /// to be played when the <see cref="Owner"/> is killed by a player with the corresponding <see cref="RoleType"/>.
        /// </summary>
        protected virtual Dictionary<RoleType, string> KilledByRoleAnnouncements { get; set; } = new();

        /// <summary>
        /// Gets or sets a <see cref="Dictionary{TKey, TValue}"/> containing all the C.A.S.S.I.E announcements
        /// to be played when the <see cref="Owner"/> is killed by a player with the corresponding <see cref="object"/>.
        /// </summary>
        protected virtual Dictionary<object, string> KilledByCustomRoleAnnouncements { get; set; } = new();

        /// <summary>
        /// Gets or sets a <see cref="Dictionary{TKey, TValue}"/> containing all the C.A.S.S.I.E announcements
        /// to be played when the <see cref="Owner"/> is killed by a player belonging to the corresponding <see cref="Team"/>.
        /// </summary>
        protected virtual Dictionary<Team, string> KilledByTeamAnnouncements { get; set; } = new();

        /// <summary>
        /// Gets or sets a <see cref="Dictionary{TKey, TValue}"/> containing all the C.A.S.S.I.E announcements
        /// to be played when the <see cref="Owner"/> is killed by a player belonging to the corresponding <see cref="object"/>.
        /// </summary>
        protected virtual Dictionary<object, string> KilledByCustomTeamAnnouncements { get; set; } = new();

        /// <summary>
        /// Gets or sets the <see cref="RoleType"/> of this <see cref="RoleBuilder"/> component.
        /// </summary>
        protected RoleType Role { get; set; }

        /// <summary>
        /// Gets or sets the third person <see cref="CharacterMeshComponent"/>.
        /// </summary>
        protected CharacterMeshComponent ThirdPersonCameraMeshComponent { get; set; }

        /// <summary>
        /// Gets or sets the first person <see cref="CharacterMeshComponent"/>.
        /// </summary>
        protected CharacterMeshComponent FirstPersonCameraMeshComponent { get; set; }

        /// <summary>
        /// Gets the current speed of the <see cref="Owner"/>.
        /// </summary>
        protected float CurrentSpeed { get; private set; }

        /// <summary>
        /// Gets the role's configs.
        /// </summary>
        protected virtual object ConfigRaw { get; private set; }

        /// <summary>
        /// Gets a random spawnpoint.
        /// </summary>
        public Vector3 RandomSpawnpoint => Info.Spawnpoints is null || Info.Spawnpoints.IsEmpty() ?
            SpawnpointManager.GetRandomPosition(Role).transform.position :
            Room.Get(Info.Spawnpoints.ElementAt(UnityEngine.Random.Range(0, Info.Spawnpoints.Count()))).Position;

        /// <summary>
        /// Gets a value indicating whether the specified <see cref="Player"/> is the owner of this <see cref="RoleBuilder"/> component instance.
        /// </summary>
        /// <param name="player">The <see cref="Player"/> to check.</param>
        /// <returns><see langword="true"/> if the specified <see cref="Player"/> is the owner of this <see cref="RoleBuilder"/> component instance; otherwise, <see langword="false"/>.</returns>
        public bool Check(Player player) => player is not null && Owner == player;

        /// <summary>
        /// Gets a value indicating whether the specified <see cref="DamageType"/> is allowed.
        /// </summary>
        /// <param name="damageType">The <see cref="DamageType"/> to check.</param>
        /// <returns><see langword="true"/> if the specified <see cref="DamageType"/> is allowed; otherwise, <see langword="false"/>.</returns>
        public bool IsAllowed(DamageType damageType) => AllowedDamageTypes.Contains(damageType);

        /// <summary>
        /// Gets a value indicating whether the specified <see cref="DamageType"/> is ignored.
        /// </summary>
        /// <param name="damageType">The <see cref="DamageType"/> to check.</param>
        /// <returns><see langword="true"/> if the specified <see cref="DamageType"/> is ignored; otherwise, <see langword="false"/>.</returns>
        public bool IsIgnored(DamageType damageType) => IgnoredDamageTypes.Contains(damageType);

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
                PropertyInfo targetInfo = typeof(CustomRole.Info).GetProperty(propertyInfo.Name) ?? typeof(InventoryManager).GetProperty(propertyInfo.Name);
                if (targetInfo is null)
                    continue;

                targetInfo.SetValue(targetInfo.DeclaringType == typeof(CustomRole.Info) ? Info : InventoryInfo, propertyInfo.GetValue(config, null));
            }
        }

        /// <inheritdoc/>
        protected virtual void SetupThirdPersonCameraMesh(string meshName, string objectName, FTransform transform = default, float fixedTickRate = 0.01f)
        {
            ThirdPersonCameraMeshComponent = EObject.CreateDefaultSubobject<CharacterMeshComponent>(gameObject, $"{name}-TPC", gameObject, meshName);
            ThirdPersonCameraMeshComponent.Socket = transform;
            ThirdPersonCameraMeshComponent.FixedTickRate = fixedTickRate;
            Owner.EnableEffect(EffectType.Invisible);
        }

        /// <inheritdoc/>
        protected virtual void DestroyThirdPersonCameraMesh()
        {
            if (ThirdPersonCameraMeshComponent is null)
                return;

            ThirdPersonCameraMeshComponent.Destroy();
        }

        /// <inheritdoc/>
        protected virtual void Awake()
        {
            LoadConfigs(ConfigRaw);
            SubscribeEvents();

            if (Owner is null)
            {
                Owner = Player.Get(gameObject);
                if (Owner is null)
                {
                    Destroy();
                    return;
                }
            }

            _noClipEnabled = Owner.NoClipEnabled;

            foreach (CustomRole role in CustomRole.Registered)
            {
                if (role.RoleBuilderComponent != GetType())
                    continue;

                if (!IsRoleDynamic)
                    Role = role.Role;

                _canEscape = role.CanEscape;
                if (_canEscape)
                {
                    _useEscapeCustomRole = role.OverrideEscapeCustomRole;
                    _useEscapeRole = role.OverrideEscapeRole;
                    _escapeRole = role.EscapeRole;
                    _escapeCustomRole = role.EscapeCustomRole;
                }

                _isHuman = !role.IsScp;
            }

            if (Owner.Role != Role)
                Owner.SetRole(Role);

            if (IgnoreScp173)
                Scp173.TurnedPlayers.Add(Owner);

            if (!string.IsNullOrEmpty(RankName) || !string.IsNullOrEmpty(RankColor))
                Features.Badge.Load(Owner, RankName, RankColor);

            if (CustomInfo != string.Empty)
                Owner.CustomInfo += CustomInfo;

            if (HideInfoArea)
            {
                Owner.InfoArea &= ~PlayerInfoArea.UnitName;
                Owner.InfoArea &= ~PlayerInfoArea.Role;
            }
        }

        /// <inheritdoc/>
        protected virtual void Start()
        {
            if (Owner is null)
            {
                Destroy();
                return;
            }

            Owner.ReferenceHub.characterClassManager.NetworkNoclipEnabled = false;

            Timing.CallDelayed(2f, () => InternalProcesses(true));

            Owner.Health = Info.Health;
            Owner.MaxHealth = Info.MaxHealth;
            Owner.MaxArtificialHealth = Info.MaxArtificialHealth;

            if (Info.ArtificialHealth > 0f)
                Owner.AddAhp(Info.ArtificialHealth, Owner.MaxArtificialHealth, 0, 1, 0);

            Owner.Scale *= Info.Scale;

            if (ShowBroadcast)
                Owner.Broadcast(Info.Broadcast, true);

            if (UseFakeAppearance)
                Owner.ChangeAppearance(FakeAppearance);

            if (GivenEffects is not null && !GivenEffects.IsEmpty())
                Owner.EnableEffects(GivenEffects);

            if (_isHuman)
            {
                Timing.CallDelayed(1f, () => Owner.AddItem(InventoryInfo.CustomItems));
                Owner.AddItem(InventoryInfo.Items);

                foreach (KeyValuePair<AmmoType, ushort> kvp in InventoryInfo.AmmoBox)
                    Owner.AddAmmo(kvp.Key, kvp.Value);
            }
        }

        /// <inheritdoc/>
        protected virtual void FixedUpdate()
        {
            if (Owner is null ||
                (UseDefaultRoleOnly && (Owner.Role != Role)) ||
                (!AllowedRoles.IsEmpty() && !AllowedRoles.Contains(Owner.Role)) ||
                !CustomRole.Manager.Contains(Owner))
            {
                Destroy();
                return;
            }

            if (KeepEffectsActive)
            {
                foreach (EffectType effect in GivenEffects)
                {
                    if (!Owner.GetEffect(effect).IsEnabled)
                        Owner.EnableEffect(effect);
                }
            }

            CurrentSpeed = (Owner.Position - _lastPosition).magnitude;
            _lastPosition = Owner.Position;
        }

        /// <summary>
        /// Fired before the <see cref="CustomRole"/> instance gets destroyed.
        /// </summary>
        protected virtual void PartiallyDestroy()
        {
            InternalProcesses(false);
            UnsubscribeEvents();

            if (Owner is null)
                return;

            CustomRole._playerValues.Remove(Owner);

            if (IgnoreScp173)
                Scp173.TurnedPlayers.Remove(Owner);

            if (!string.IsNullOrEmpty(RankName) || !string.IsNullOrEmpty(RankColor))
                Features.Badge.Unload(Owner);

            if (!string.IsNullOrEmpty(CustomInfo))
                Owner.CustomInfo = null;

            if (HideInfoArea)
            {
                Owner.InfoArea |= PlayerInfoArea.UnitName;
                Owner.InfoArea |= PlayerInfoArea.Role;
            }

            Owner.Scale = Vector3.one;
            Owner.NoClipEnabled = _noClipEnabled;
            Owner.IsUsingStamina = true;
            Owner.DisableAllEffects();
            DestroyThirdPersonCameraMesh();
        }

        /// <summary>
        /// Destroys this <see cref="RoleBuilder"/> instance.
        /// </summary>
        public virtual void Destroy()
        {
            try
            {
                Destroy(this);
            }
            catch (Exception e)
            {
                Log.Error($"Exception: {e}\n Couldn't destroy: {this}\nIs ReferenceHub null? {Owner is null}");
            }
        }

        private protected virtual void OnDestroy() => PartiallyDestroy();

        /// <summary>
        /// Subscribes all the specified events.
        /// </summary>
        protected virtual void SubscribeEvents()
        {
        }

        /// <summary>
        /// Unsubscribes all the specified events.
        /// </summary>
        protected virtual void UnsubscribeEvents()
        {
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
            if (!Check(ev.Target) || ev.Attacker is null ||
                !ev.Attacker.IsScp || CanBeHurtedByScps)
                return;

            ev.IsAllowed = false;
        }

        /// <see cref="Exiled.Events.Handlers.Player.OnHurting(HurtingEventArgs)"/>
        private protected virtual void IgnoreDamage(HurtingEventArgs ev)
        {
            if (!Check(ev.Target) || !AllowedDamageTypes.IsEmpty() || !IgnoredDamageTypes.Contains(ev.Handler.Type))
                return;

            ev.Amount = 0;
            ev.IsAllowed = false;
        }

        /// <see cref="Exiled.Events.Handlers.Player.OnChangingItem(ChangingItemEventArgs)"/>
        private protected virtual void ChangingItemBehvaior(ChangingItemEventArgs ev)
        {
            if (!Check(ev.Player) || CanChangeItems)
                return;

            ev.IsAllowed = false;
        }

        /// <see cref="Exiled.Events.Handlers.Player.OnHurting(HurtingEventArgs)"/>
        private protected virtual void AllowDamage(HurtingEventArgs ev)
        {
            if (!Check(ev.Target) || !IgnoredDamageTypes.IsEmpty() || !AllowedDamageTypes.Contains(ev.Handler.Type))
                return;

            ev.IsAllowed = false;
        }

        /// <see cref="Exiled.Events.Handlers.Player.OnHurting(HurtingEventArgs)"/>
        private protected virtual void PreventDealingDamageToScps(HurtingEventArgs ev)
        {
            if (!Check(ev.Attacker) || CanHurtScps)
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
            if (!Check(ev.Target) || CanBeHandcuffed)
                return;

            ev.IsAllowed = false;
        }

        /// <see cref="Exiled.Events.Handlers.Player.OnChangingGroup(ChangingGroupEventArgs)"/>
        private protected virtual void DestroyOnChangingRole(ChangingRoleEventArgs ev)
        {
            if (!Check(ev.Player) || AllowedRoles.Contains(ev.NewRole))
                return;

            Destroy();
        }

        /// <see cref="Exiled.Events.Handlers.Player.OnSearchPickupRequest(SearchingPickupEventArgs)"/>
        private protected virtual void PickingUpItemBehavior(SearchingPickupEventArgs ev)
        {
            if (!Check(ev.Player) || CanPickupItems)
                return;

            ev.IsAllowed = false;
        }

        /// <see cref="Exiled.Events.Handlers.Scp096.OnAddingTarget(AddingTargetEventArgs)"/>
        private protected virtual void PreventPlayerFromTriggeringScp096(AddingTargetEventArgs ev)
        {
            if (!Check(ev.Target) || !IgnoreScp096)
                return;

            ev.IsAllowed = false;
        }

        /// <see cref="Exiled.Events.Handlers.Player.OnEscaping(EscapingEventArgs)"/>
        private protected virtual void PreventPlayerFromEscaping(EscapingEventArgs ev)
        {
            if (!Check(ev.Player) || (!_useEscapeRole && !_useEscapeCustomRole))
                return;

            ev.IsAllowed = false;
        }

        /// <see cref="Exiled.Events.Handlers.Player.OnDied(DiedEventArgs)"/>
        private protected virtual void AnnounceOwnerDeath(DiedEventArgs ev)
        {
            if (!Check(ev.Target) || Check(ev.Killer) || Check(Server.Host) || !IsDeathAnnouncementEnabled)
                return;

            string announcement = string.Empty;
            if (ev.Killer is null)
                goto Announce;

            if (CustomRole.TryGet(ev.Killer, out CustomRole customRole))
            {
                if (CustomTeam.TryGet<CustomTeam>(customRole, out CustomTeam customTeam) &&
                    KilledByCustomTeamAnnouncements.TryGetValue(customTeam.Id, out announcement))
                    goto Announce;
                else if (KilledByCustomRoleAnnouncements.TryGetValue(customRole.Id, out announcement))
                    goto Announce;
            }
            else
            {
                if (KilledByRoleAnnouncements.TryGetValue(ev.Killer.Role, out announcement))
                    goto Announce;
            }

            return;
        Announce:
            if (string.IsNullOrEmpty(announcement))
                announcement = UnknownTerminationCauseAnnouncement;

            if (!string.IsNullOrEmpty(announcement))
                Cassie.Message(announcement);
        }

        /// <see cref="Exiled.Events.Handlers.Scp106.OnContaining(ContainingEventArgs)"/>
        private protected virtual void ContainingBehavior(ContainingEventArgs ev)
        {
            if (!Check(ev.ButtonPresser) || CanContainScp106)
                return;

            ev.IsAllowed = false;
        }

        /// <see cref="Exiled.Events.Handlers.Player.OnIntercomSpeaking(IntercomSpeakingEventArgs)"/>
        private protected virtual void IntercomSpeakingBehavior(IntercomSpeakingEventArgs ev)
        {
            if (!Check(ev.Player) || CanUseIntercom)
                return;

            ev.IsAllowed = false;
        }

        /// <see cref="Exiled.Events.Handlers.Player.OnEnteringPocketDimension(EnteringPocketDimensionEventArgs)"/>
        private protected virtual void EnteringPocketDimensionBehavior(EnteringPocketDimensionEventArgs ev)
        {
            if (!Check(ev.Player) || CanEnterPocketDimension)
                return;

            ev.IsAllowed = false;
        }

        /// <see cref="Exiled.Events.Handlers.Player.OnVoiceChatting(VoiceChattingEventArgs)"/>
        private protected virtual void VoiceChattingBehavior(VoiceChattingEventArgs ev)
        {
            if (!Check(ev.Player) || CanUseVoiceChat)
                return;

            ev.IsAllowed = false;
        }

        /// <see cref="Exiled.Events.Handlers.Map.OnPlacingBlood(PlacingBloodEventArgs)"/>
        private protected virtual void PlacingBloodBehavior(PlacingBloodEventArgs ev)
        {
            if (!Check(ev.Player) || CanPlaceBlood)
                return;

            ev.IsAllowed = false;
        }

        /// <see cref="Exiled.Events.Handlers.Player.OnEnteringPocketDimension(EnteringPocketDimensionEventArgs)"/>
        private protected virtual void DroppingItemBehavior(DroppingItemEventArgs ev)
        {
            if (!Check(ev.Player) || CanDropItems)
                return;

            ev.IsAllowed = false;
        }

        /// <see cref="Exiled.Events.Handlers.Player.OnActivatingWarheadPanel(ActivatingWarheadPanelEventArgs)"/>
        private protected virtual void ActivatingWarheadBehavior(ActivatingWarheadPanelEventArgs ev)
        {
            if (!Check(ev.Player) || CanActivateWarhead)
                return;

            ev.IsAllowed = false;
        }

        /// <see cref="Exiled.Events.Handlers.Player.OnActivatingGenerator(ActivatingGeneratorEventArgs)"/>
        private protected virtual void ActivatingGeneratorBehavior(ActivatingGeneratorEventArgs ev)
        {
            if (!Check(ev.Player) || CanActivateGenerators)
                return;

            ev.IsAllowed = false;
        }

        /// <see cref="Exiled.Events.Handlers.Player.OnActivatingWorkstation(ActivatingWorkstationEventArgs)"/>
        private protected virtual void ActivatingWorkstationBehavior(ActivatingWorkstationEventArgs ev)
        {
            if (!Check(ev.Player) || CanActivateWorkstations)
                return;

            ev.IsAllowed = false;
        }

        /// <see cref="Exiled.Events.Handlers.Player.OnInteractingElevator(InteractingElevatorEventArgs)"/>
        private protected virtual void InteractingElevatorBehavior(InteractingElevatorEventArgs ev)
        {
            if (!Check(ev.Player) || CanUseElevators)
                return;

            ev.IsAllowed = false;
        }

        /// <see cref="Exiled.Events.Handlers.Player.OnInteractingDoor(InteractingDoorEventArgs)"/>
        private protected virtual void CheckpointsBehavior(InteractingDoorEventArgs ev)
        {
            if (!Check(ev.Player) || !CanBypassCheckpoints ||
                ev.Door.Type is not DoorType.CheckpointEntrance and
                not DoorType.CheckpointLczA and
                not DoorType.CheckpointLczB)
                return;

            ev.IsAllowed = true;
        }

        /// <see cref="Exiled.Events.Handlers.Player.OnInteractingDoor(InteractingDoorEventArgs)"/>
        private protected virtual void InteractingDoorBehavior(InteractingDoorEventArgs ev)
        {
            if (!Check(ev.Player) || !BypassableDoors.Contains(ev.Door.Type))
                return;

            ev.IsAllowed = false;
        }

        /// <see cref="Exiled.Events.Handlers.Player.OnEnteringFemurBreaker(EnteringFemurBreakerEventArgs)"/>
        private protected virtual void EnteringFemurBreakerBehavior(EnteringFemurBreakerEventArgs ev)
        {
            if (!Check(ev.Player) || CanEnterFemurBreaker)
                return;

            ev.IsAllowed = false;
        }

        private void InternalProcesses(bool status)
        {
            if (status)
            {
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
                Exiled.Events.Handlers.Player.EnteringFemurBreaker += EnteringFemurBreakerBehavior;
                Exiled.Events.Handlers.Player.Handcuffing += HandcuffingBehavior;
                Exiled.Events.Handlers.Scp096.AddingTarget += PreventPlayerFromTriggeringScp096;
                Exiled.Events.Handlers.Scp106.Containing += ContainingBehavior;
                Exiled.Events.Handlers.Map.PlacingBlood += PlacingBloodBehavior;

                _escapeHandle = Timing.RunCoroutine(WorldEscapeCheck());
                if (IsNightVisionEnabled && !Timing.IsRunning(_nightVisionHandle))
                    _nightVisionHandle = Timing.RunCoroutine(NightVision());
            }
            else
            {
                Timing.KillCoroutines(_escapeHandle);
                Timing.KillCoroutines(_nightVisionHandle);

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
                Exiled.Events.Handlers.Player.EnteringFemurBreaker -= EnteringFemurBreakerBehavior;
                Exiled.Events.Handlers.Player.Handcuffing -= HandcuffingBehavior;
                Exiled.Events.Handlers.Scp096.AddingTarget -= PreventPlayerFromTriggeringScp096;
                Exiled.Events.Handlers.Scp106.Containing -= ContainingBehavior;
                Exiled.Events.Handlers.Map.PlacingBlood -= PlacingBloodBehavior;
            }
        }

        private IEnumerator<float> WorldEscapeCheck()
        {
            if (!_canEscape || (!_useEscapeRole && !_useEscapeCustomRole))
                yield break;

            while (Round.IsStarted)
            {
                yield return Timing.WaitForOneFrame;

                if (Vector3.Distance(Owner.Position, HLAPI.WorldEscapePosition) > HLAPI.WorldEscapeRadius)
                    continue;

                if (_useEscapeRole)
                    Owner.SetRole(_escapeRole, SpawnReason.Escaped, false);
                else if (_useEscapeCustomRole)
                    Owner.SetRole(_escapeCustomRole);
            }
        }

        private IEnumerator<float> NightVision()
        {
            while (IsNightVisionEnabled)
            {
                yield return Timing.WaitForOneFrame;

                if (Owner.CurrentRoom is null || Owner.CurrentRoom.LightsOn)
                    continue;

                Owner.SendFakeSyncVar(
                    Owner.CurrentRoom.FlickerableLightControllerNetIdentity,
                    typeof(FlickerableLightController),
                    nameof(FlickerableLightController.NetworkLightsEnabled),
                    true);
            }
        }
    }
}
