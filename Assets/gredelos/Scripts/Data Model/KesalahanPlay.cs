using System;
[Serializable] public class KesalahanPlay
{
    public string id_kesalahan;
    public string fk_id_progress; // referensi ke progress
    public string tipe_kesalahan; // "click" / "swipe" / "dst"
    public int jumlah_kesalahan; // jumlah kesalahan yang terjadi
}