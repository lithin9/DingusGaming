using System.Collections.Generic;
using Rocket.API;
using Rocket.Unturned.Player;

namespace DingusGaming.Store
{
    public class CommandSellExperience : IRocketCommand
    {
        private const string NAME = "sellexp";
        private const string HELP = "Sell experience points.";
        private const string SYNTAX = "<amount>";
        private const bool ALLOW_FROM_CONSOLE = false;
        private const bool RUN_FROM_CONSOLE = false;

        public bool RunFromConsole
        {
            get { return RUN_FROM_CONSOLE; }
        }

        public string Name
        {
            get { return NAME; }
        }

        public string Help
        {
            get { return HELP; }
        }

        public string Syntax
        {
            get { return SYNTAX; }
        }

        public List<string> Aliases { get; } = new List<string> {"sellexperience", "sellskill", "sellskills"};

        public bool AllowFromConsole
        {
            get { return ALLOW_FROM_CONSOLE; }
        }

        public List<string> Permissions { get; } = new List<string>();

        public void Execute(IRocketPlayer caller, string[] command)
        {
            Execute((UnturnedPlayer) caller, command);
        }

        public void Execute(UnturnedPlayer caller, string[] command)
        {
            int cost = CommandBuyExperience.cost;
            if (command.Length != 1)
                DGPlugin.messagePlayer(caller, "Invalid amount of parameters. Format is \"/sellexp amount\".");
            else
            {
                uint amount;

                if (!uint.TryParse(command[0], out amount))
                    DGPlugin.messagePlayer(caller, "Invalid amount.");
                else if (caller.Experience >= amount)
                {
                    caller.Experience -= amount;
                    Currency.changeBalance(caller, cost*(int) amount);
                    DGPlugin.messagePlayer(caller, amount + " experience sold! Your new balance is $"+Currency.getBalance(caller)+".");
                }
                else
                    DGPlugin.messagePlayer(caller, "Insufficient experience(" + caller.Experience + "/" + amount + " exp).");
            }
        }
    }
}