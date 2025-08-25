using System;
[Serializable]
public class MainSession // = Waktu Bermain
{
    public string id_main;
    public string waktu_mulai; // waktu mulai sesi permainan saat player memulai level
    public string waktu_berakhir;   // null saat belum selesai
}