using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : Singleton<LobbyManager>
{
    public MenuState nextMenu {get; set;}

    public enum MenuState { Main, Lobby, Room}


}
