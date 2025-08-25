using System;
[Serializable]
public class Progress
{
    public string id_progress;
    public string fk_id_level; // referensi ke level
    public string nama_progress; // nama progress
    public int jumlah_hadiah_koin; // reward jumlah koin yang didapat
    public bool is_main = false;

    public bool Get_is_main()
    {
        return is_main;
    }
}