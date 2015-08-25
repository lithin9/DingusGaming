using Steamworks;
using System.Collections.Generic;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using DingusGaming.DingusGaming.data;

namespace DingusGaming.Store
{
    public class Currency
    {
        private static readonly int startingAmount = 50;
        private static Dictionary<string, int> balances;
        private static DataAccess dataAccessor = DataAccessFactory.getDataAccessor(); 

        public static void init()
        {
            loadBalances();
            registerOnServerShutdown();
            registerPlayerOnConnected();
            registerOnPlayerDeath();
        }

        private static void loadBalances()
        {
            balances = dataAccessor.getBalances();
        }

        private static void saveBalances()
        {
            dataAccessor.setBalances(balances);  
        }

        private static void registerOnPlayerDeath()
        {
            UnturnedPlayerEvents.OnPlayerDeath += delegate (UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
            {
                // Grant the killing user 5 credits + 10% of their victim's credits
                UnturnedPlayer killer = DGPlugin.getKiller(player, cause, murderer);
                if (killer != null)
                    changeBalance(killer, 5 + getBalance(player) / 10);
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