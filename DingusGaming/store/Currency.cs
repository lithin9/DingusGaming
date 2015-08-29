using Steamworks;
using System.Collections;
using System.Collections.Generic;
using DingusGaming.Party;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;

namespace DingusGaming.Store
{
    public class Currency
    {
        static readonly int startingAmount = 50;
        static Dictionary<string, int> balances;

        public static void init()
        {
            loadBalances();
            registerOnServerShutdown();
            registerPlayerOnConnected();
            registerOnPlayerDeath();
        }

        private static void loadBalances()
        {
            // TODO: Refactor this to service
            List<DictionaryEntry> temp = DGPlugin.readFromFile<List<DictionaryEntry>>("balances.xml");
            if (temp != null)
                balances = DGPlugin.convertToDictionary<string, int>(temp);
            else
                balances = new Dictionary<string, int>();
        }

        private static void saveBalances()
        {
            // TODO: Refactor this to service
            DGPlugin.writeToFile(DGPlugin.convertFromDictionary(balances), "balances.xml");
        }

        private static void registerOnPlayerDeath()
        {
            UnturnedPlayerEvents.OnPlayerDeath += delegate (UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
            {
                // Grant the killing user 5 credits + 10% of their victim's credits
                UnturnedPlayer killer = DGPlugin.getKiller(player, cause, murderer);
                if (killer != null && (Parties.getParty(player)==null || !Parties.getParty(player).isMember(killer)))
                {
                    int amount = 5 + getBalance(player)/10;
                    changeBalance(killer, amount);
                    DGPlugin.messagePlayer(killer, "You earned $" + amount + " from killing " + player.CharacterName + ".");
                    DGPlugin.messagePlayer(player, killer.CharacterName+" got $" + amount + " from killing you.");
                }
            };
        }

        private static void registerOnServerShutdown()
        {
            Steam.OnServerShutdown += delegate ()
            {
                saveBalances();
            };
        }

        private static void registerPlayerOnConnected()
        {
            U.Events.OnPlayerConnected += delegate (UnturnedPlayer player)
            {
                addPlayer(player);
            };
        }

        public static void addPlayer(UnturnedPlayer player)
        {
            if (!balances.ContainsKey(DGPlugin.getConstantID(player)))
                balances.Add(DGPlugin.getConstantID(player), startingAmount);
        }

        public static void changeBalance(UnturnedPlayer player, int amount)
        {
            balances[DGPlugin.getConstantID(player)] += amount;
        }

        public static int getBalance(UnturnedPlayer player)
        {
            return balances[DGPlugin.getConstantID(player)];
        }

        public static bool transferCredits(UnturnedPlayer from, UnturnedPlayer to, int amount)
        {
            string src = DGPlugin.getConstantID(from), dest = DGPlugin.getConstantID(to);
            if (amount > 0 && balances[src] >= amount)
            {
                balances[src] -= amount;
                balances[dest] += amount;
                return true;
            }
            return false;
        }
    }
}