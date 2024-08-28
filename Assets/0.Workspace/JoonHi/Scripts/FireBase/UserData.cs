using System;

[Serializable]
public class Userdata
{
    public string nickName; // 닉네임
    public int token;       // 토큰 갯수
    public int excaliver;   // 엑스칼리버 찬탈 갯수

    public bool priest;     // 프리스트 소지여부
    public bool wizard;     // 마법사 소지여부

    public bool tutorial;   // 튜토리얼 시청여부

    public Userdata()
    {
        this.nickName = "Unknown";
        this.token = 0;
        this.excaliver = 0;
        this.priest = false;
        this.wizard = false;
        this.tutorial = false;
    }

    public Userdata(string nickName, int token, int excaliver, bool priest, bool wizard, bool tutorial)
    {
        this.nickName = nickName;
        this.token = token;
        this.excaliver = excaliver;
        this.priest = priest;
        this.wizard = wizard;
        this.tutorial = tutorial;
    }
}
