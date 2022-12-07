namespace RolePlus.Data.Commands
{
    using System;
    using CommandSystem;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Permissions.Extensions;

    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    internal class BypassNoclip : ICommand
    {
        /// <inheritdoc/>
        public string Command => "bypassnoclip";

        /// <inheritdoc/>
        public string[] Aliases => new string[] { "bnoclip", "bypassn" };

        /// <inheritdoc/>
        public string Description => "Bypass hidden/unhidden function";

        /// <inheritdoc/>
        public bool ICommand.Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!((CommandSender)sender).CheckPermission("rp.bypassnoclip"))
            {
                response = "You do not have permission to use this command";
                return false;
            }

            if (arguments.Count != 0)
            {
                response = "Usage: bypassnoclip";
                return false;
            }

            Player player = Player.Get(sender);

            if (API.API.WhitelistedNoclipStaffers.Contains(player))
            {
                API.API.WhitelistedNoclipStaffers.Remove(player);
                response = "Removed from noclip-bypass";
                return true;
            }

            API.API.WhitelistedNoclipStaffers.Add(player);
            response = "Added from noclip-bypass";
            return true;
        }
    }
}
