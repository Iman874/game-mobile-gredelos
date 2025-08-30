using System;

[Serializable]
public class ProgressMain
{
    public string id_progress_main;
    public string fk_id_progress;
    public string fk_id_login;
    public string fk_id_main;
    public int status_penyelesaian; // 0 = belum selesai, 1 = sudah selesai
}