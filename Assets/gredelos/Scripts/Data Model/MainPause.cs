using System;
[Serializable]
public class MainPause
{
    public string id_main_pause;
    public string fk_id_main; // referensi ke main_session
    public string fk_id_pause; // referensi ke pause
}