using Photon.Realtime;

public class ClientManager : Singleton<FirebaseManager>
{
    private static RoomInfo roomInfo;
    public static RoomInfo RoomInfo { get { return roomInfo; } }

    private static bool signin;
    public static bool SignIn { get { return signin; } }

    protected override void Awake()
    {
        base.Awake();
        signin = false;
    }
}

