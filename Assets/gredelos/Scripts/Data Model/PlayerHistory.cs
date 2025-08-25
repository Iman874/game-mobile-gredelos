using System;

[Serializable]
public class PlayerHistory
{
    public string id_history;
    public string fk_id_level;     // relasi ke Level
    public string fk_nama_player;  // relasi ke Player
}
