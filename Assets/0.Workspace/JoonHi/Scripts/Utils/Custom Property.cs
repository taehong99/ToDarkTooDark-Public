using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhotonHashTable = ExitGames.Client.Photon.Hashtable;

public enum Team { Red, Blue }

public static class CustomProperty
{
    static PhotonHashTable property = new PhotonHashTable();

    // Player
    public const string READY = "Ready";
    public const string LOAD = "Load";
    public const string JOB = "Job";
    public const string TEAM = "Team";
    public const string ALIVE = "Alive";

    // Room
    public const string GAMESTART = "GameStart";
    public const string GAMESTARTTIME = "GameStartTime";
    public const string GAMEMODE = "GameMode";
    public const string SEED = "Seed";
    public const string PLAYERS = "Players";

    #region Player Custom Property
    // Player Ready State
    public static bool GetReady(this Player player)
    {
        PhotonHashTable customProperty = player.CustomProperties;
        if (customProperty.TryGetValue(READY, out object value))
        {
            return (bool) value;
        }
        else
        {
            return false;
        }
    }
    public static void SetReady(this Player player, bool value)
    {
        PhotonHashTable ht = new PhotonHashTable();
        ht[READY] = value;
        player.SetCustomProperties(ht);
    }

    // Player Load State
    public static bool GetLoad(this Player player)
    {
        PhotonHashTable customProperty = player.CustomProperties;
        if (customProperty.TryGetValue(LOAD, out object value))
        {
            return (bool) value;
        }
        else
        {
            return false;
        }
    }
    public static void SetLoad(this Player player, bool value)
    {
        PhotonHashTable ht = new PhotonHashTable();
        ht[LOAD] = value;
        player.SetCustomProperties(ht);
    }

    // Player Selected Character
    public static PlayerJob GetJob(this Player player)
    {
        PhotonHashTable customProperty = player.CustomProperties;
        if (customProperty.TryGetValue(JOB, out object value))
        {
            return (PlayerJob) value;
        }
        else
        {
            return PlayerJob.Swordsman;
        }
    }
    public static void SetJob(this Player player, PlayerJob value)
    {
        PhotonHashTable ht = new PhotonHashTable();
        ht[JOB] = value;
        player.SetCustomProperties(ht);
    }

    // Player Team (Red or Blue)
    public static Team GetTeam(this Player player)
    {
        PhotonHashTable customProperty = player.CustomProperties;
        if (customProperty.TryGetValue(TEAM, out object value))
        {
            return (Team) value;
        }
        return Team.Red;
    }
    public static void SetTeam(this Player player, Team value)
    {
        PhotonHashTable ht = new PhotonHashTable();
        ht[TEAM] = value;
        player.SetCustomProperties(ht);
    }

    // Player Teammate
    public static Player GetTeammate(this Player player)
    {
        foreach(var roomPlayer in PhotonNetwork.CurrentRoom.Players)
        {
            if (roomPlayer.Value == player)
                continue;

            if (roomPlayer.Value.GetTeam() == player.GetTeam())
                return roomPlayer.Value;
        }
        return null;
    }

    // Player Alive
    public static bool GetAlive(this Player player)
    {
        PhotonHashTable customProperty = player.CustomProperties;
        if (customProperty.TryGetValue(ALIVE, out object value))
        {
            return (bool) value;
        }
        return true;
    }
    public static void SetAlive(this Player player, bool value)
    {
        PhotonHashTable ht = new PhotonHashTable();
        ht[ALIVE] = value;
        player.SetCustomProperties(ht);
    }

    #endregion

    #region Room Custom Property
    public static bool GetGameStart(this Room room)
    {
        PhotonHashTable customProperty = room.CustomProperties;
        if (customProperty.TryGetValue(GAMESTART, out object value))
        {
            return (bool) value;
        }
        else
        {
            return false;
        }
    }

    public static void SetGameStart(this Room room, bool value)
    {
        property.Clear();
        property[GAMESTART] = value;
        room.SetCustomProperties(property);
    }

    
    public static double GetGameStartTime(this Room room)
    {
        PhotonHashTable customProperty = room.CustomProperties;
        if (customProperty.TryGetValue(GAMESTARTTIME, out object value))
        {
            return (double) value;
        }
        else
        {
            return 0;
        }
    }

    public static void SetGameStartTime(this Room room, double value)
    {
        property.Clear();
        property[GAMESTARTTIME] = value;
        room.SetCustomProperties(property);
    }


    // true = 1:1:1:1
    // false = 2:2
    public static bool GetGameMode(this Room room)
    {
        PhotonHashTable customProperty = room.CustomProperties;
        if (customProperty.TryGetValue(GAMEMODE, out object value))
        {
            return (bool) value;
        }
        else
        {
            return true;
        }
    }

    public static void SetGameMode(this Room room, bool value)
    {
        property.Clear();
        property[GAMEMODE] = value;
        room.SetCustomProperties(property);
    }


    public static int GetSeed(this Room room)
    {
        PhotonHashTable customProperty = room.CustomProperties;
        if (customProperty.TryGetValue(SEED, out object value))
        {
            return (int) value;
        }
        else
        {
            return -1;
        }
    }

    public static void SetSeed(this Room room, int value)
    {
        property.Clear();
        property[SEED] = value;
        room.SetCustomProperties(property);
    }


    public static int[] GetPlayers(this Room room)
    {
        PhotonHashTable customProperty = room.CustomProperties;
        if (customProperty.TryGetValue(PLAYERS, out object value))
        {
            return (int[]) value;
        }
        else
        {
            return null;
        }
    }

    public static int GetOpenSlotIdx(this Room room)
    {
        PhotonHashTable customProperty = room.CustomProperties;
        if (customProperty.TryGetValue(PLAYERS, out object value))
        {
            int[] players = (int[]) value;
            Debug.Log("Entered try get");
            for(int i = 0; i < players.Length; i++)
            {
                if (players[i] == -1)
                    return i;
            }
        }
        return -1;
    }

    public static void RemovePlayer(this Room room, int player)
    {
        property.Clear();
        Debug.Log($"Removed Player{player}");
        PhotonHashTable customProperty = room.CustomProperties;
        if (customProperty.TryGetValue(PLAYERS, out object value))
        {
            int[] players = (int[]) value;
            for(int i = 0; i < players.Length; i++)
            {
                if (players[i] == player)
                {
                    players[i] = -1;
                    break;
                }
            }
            property[PLAYERS] = players;
            Debug.Log($"Players = {players[0]} {players[1]} {players[2]} {players[3]}");
            room.SetCustomProperties(property);
        }
    }

    public static void SetPlayers(this Room room, int[] value)
    {
        Debug.Log("Set players");
        property.Clear();
        property[PLAYERS] = value;
        room.SetCustomProperties(property);
    }
    #endregion
}
