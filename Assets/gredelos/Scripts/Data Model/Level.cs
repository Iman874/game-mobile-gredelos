using System;

[Serializable]
public class Level
{
    public string id_level;
    public string nama_level;
    public int total_koin;
    public int status_level; // 0 = belum terbuka, 1 = terbuka
}