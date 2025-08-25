using System;
[Serializable]
public class CompletePlay
{
    public string id_complete_level;
    public string fk_id_level; // reference to level.id_level
    public string fk_id_login; // reference to login.id_login
    public string fk_nama_player; // reference to player.nama_player
    public string waktu_penyelesaian; // waktu penyelesaian level
}