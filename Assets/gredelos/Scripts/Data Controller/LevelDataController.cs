using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class LevelDataController : MonoBehaviour
{
    public static LevelDataController I;

    [Header("Data Game (JSON)")]
    [Note("Note: Perubahan hanya bisa dilakukan di script, bukan pada Inspector")]

    public int dummy = 0;

    private string fileName = "game_data.json";

    public bool UseMood = false;

    string FilePath => Path.Combine(Application.persistentDataPath, fileName);

    public string[] namaProgressList = new string[]
    {
        "level_1_01_membuka_gorden",
        "level_1_02_merapikan_bantal",
        "level_1_03_merapikan_selimut",
        "level_2_01_sikat_gigi",
        "level_2_02_mandi",
        "level_2_03_mengeringkan_rambut",
        "level_3_01_memakai_pakaian",
        "level_3_02_menyisir_rambut",
        "level_4_01_merapikan_mainan",
        "level_4_02_membersihkan_ruangan",
        "level_5_01_makan_dan_minum",
        "level_5_02_mencuci_piring"
    };

    // Gunakan DbRoot dari Data Model
    [SerializeField] public DbRoot db = new();

   
    void Start()
    {
        
        // Load volume level dari player prefs
        float savedVolume = PlayerPrefs.GetFloat("Volume", 0.75f);
        ManagerAudio.instance.SetVolume(savedVolume);

    }


    // ---------- LIFECYCLE ----------
    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
        LoadDataLevel();

        if (!PlayerPrefs.HasKey("Version"))
        {
            // Kalau belum ada, buat baru
            PlayerPrefs.SetString("Version", "1.2");
        }
        else
        {
            Debug.Log("Version App Saat ini: " + PlayerPrefs.GetString("Version"));
        }

        if (!PlayerPrefs.HasKey("StatusPembaruan_1.2"))
        {
            // Kalau belum ada, buat baru
            PlayerPrefs.SetString("StatusPembaruan_1.2", "false");
            PlayerPrefs.Save();
        }
        else
        {
            // Kalau sudah ada, biarin aja (gak diubah)
            Debug.Log("StatusPembaruan sudah ada: " + PlayerPrefs.GetString("StatusPembaruan_1.2"));
        }

        // Apakah Mood diaktifkan?
        if (UseMood)
        {
            // Cek apakah mood sudah pernah digunakan
            if (PlayerPrefs.GetInt("MoodUsed", 0) == 0) // default 0 = belum pernah dipakai
            {
                var player = db.player.Count > 0 ? db.player[0] : null;
                if (player != null)
                {
                    player.jumlah_koin += 10000;
                    Save();
                    Debug.Log("Mood aktif: 10000 koin ditambahkan ke player.");
                }

                // Simpan bahwa mood sudah digunakan
                PlayerPrefs.SetInt("MoodUsed", 1);
                PlayerPrefs.Save();
            }
            else
            {
                Debug.Log("Mood sudah pernah digunakan, tidak bisa lagi.");
            }

            // Reset UseMood supaya tidak repeat di frame berikutnya
            UseMood = false;
        }

        // Selalu update database
        
        // Update database sesuai versi
        UpdateDatabase("1.2"); // versi db saat ini 1.2 sesuai dengan versi aplikasi
    }

   
    void OnApplicationQuit() => Save();
    void OnDestroy() { if (I == this) Save(); }

    // ---------- Fungsi Load data ----------
    public void LoadDataLevel()
    {
        if (File.Exists(FilePath))
        {
            db = JsonUtility.FromJson<DbRoot>(File.ReadAllText(FilePath));
        }
        else
        {
            db = new DbRoot(); // buat baru kalau file belum ada
            Save(); // Simpan data level
        }

        // Pastikan ada player
        if (db.player.Count == 0)
        {
            Debug.LogWarning("Data player belum ada.");
            string namaPlayer = $"player_{DateTime.Now:yyyy_dd_HH_mm_ss_fff}";
            EnsurePlayer(namaPlayer);
        }

        // Pastikan ada level
        if (db.level.Count == 0)
        {
            Debug.LogWarning("Data level belum ada.");
            string namaPlayer = db.player[0].nama_player; // selalu ambil player pertama
            for (int i = 1; i <= 5; i++)
            {
                db.level.Add(new Level
                {
                    id_level = $"level_{i}_{namaPlayer}",
                    nama_level = $"level_{i}",
                    total_koin = 500 * i, // setiap level butuh 500 koin kelipatan level
                    status_level = (i == 1) ? 1 : 0 // level 1 terbuka
                });
            }
        }

        // Load data progress
        LoadDataProgress();

        // Cek data playerprefs jika ada ataupun tidak ada
        if ((PlayerPrefs.GetString("Version") == "1.2" && PlayerPrefs.GetString("StatusPembaruan_1.2") == "false") || !PlayerPrefs.HasKey("Version")) 
        {
            // Update database sesuai versi
            UpdateDatabase("1.2"); // versi db saat ini 1.2 sesuai dengan versi aplikasi

            // Update status pembaruan
            PlayerPrefs.SetString("StatusPembaruan_1.2", "true");
            PlayerPrefs.Save();
        }
    }

    public int GetLevelCoinCost(int nomorLevel)
    {
        var level = db.level.Find(x => x.nama_level == $"level_{nomorLevel}");
        if (level != null)
        {
            return level.total_koin;
        }
        return 1000; // default 1000
    }

    void UpdateDatabase(string versiDb)
    {
        // versi db saat ini 1.2
        if (versiDb == "1.2")
        {
            UpdateTable("level", "1.2");
            UpdateTable("progress", "1.2");
        }
    }

    // fungsi update db level dan progress
    void UpdateTable(string namaTable, string versiTable)
    {
        if (namaTable == "level" && versiTable == "1.2")
        {
            // level 2 → 1000
            var level2 = db.level.Find(x => x.nama_level == "level_2");
            if (level2 != null) level2.total_koin = 1000;

            // level 3 → 1500
            var level3 = db.level.Find(x => x.nama_level == "level_3");
            if (level3 != null) level3.total_koin = 1500;

            // level 4 → 2000
            var level4 = db.level.Find(x => x.nama_level == "level_4");
            if (level4 != null) level4.total_koin = 2000;

            // level 5 → 2500
            var level5 = db.level.Find(x => x.nama_level == "level_5");
            if (level5 != null) level5.total_koin = 2500;

            Debug.Log("Update table level ke versi 1.2 selesai.");
        }

        // Update table progress ke versi 1.2
        if (namaTable == "progress" && versiTable == "1.2")
        {
            // mapping manual nama_progress → jumlah_hadiah_koin
            var updateKoin = new Dictionary<string, int>
            {
                { "level_1_01_membuka_gorden", 10 },
                { "level_1_02_merapikan_bantal", 10 },
                { "level_1_03_merapikan_selimut", 10 },
                { "level_2_01_sikat_gigi", 20 },
                { "level_2_02_mandi", 20 },
                { "level_2_03_mengeringkan_rambut", 15 },
                { "level_3_01_memakai_pakaian", 35 },
                { "level_3_02_menyisir_rambut", 35 },
                { "level_4_01_merapikan_mainan", 40 },
                { "level_4_02_membersihkan_ruangan", 45 },
                { "level_5_01_makan_dan_minum", 50 },
                { "level_5_02_mencuci_piring", 55 },
            };

            foreach (var kv in updateKoin)
            {
                var progres = db.progress.FirstOrDefault(x => x.nama_progress == kv.Key);
                if (progres != null)
                {
                    progres.jumlah_hadiah_koin = kv.Value;
                    Debug.Log($"Update {progres.nama_progress} → {kv.Value} koin");
                }
                else
                {
                    Debug.LogWarning($"Progress '{kv.Key}' tidak ditemukan di db");
                }
            }
        }
    }

    public void LoadDataProgress()
    {
        // Pastikan ada data progress
        if (db.progress.Count == 0)
        {
            // Pastikan player sudah ada
            if (db.player.Count == 0)
            {
                Debug.LogWarning("Data player belum ada, progress tidak bisa dibuat.");
                return;
            }

            // Pastikan level sudah ada
            if (db.level.Count == 0)
            {
                Debug.LogWarning("Data level belum ada, progress tidak bisa dibuat.");
                return;
            }

            Debug.LogWarning("Data progress belum ada. Membuat data progress");

            // Group progress by level
            var progressByLevel = namaProgressList
                .GroupBy(n => n.Split('_')[0] + "_" + n.Split('_')[1]); // contoh: level_1, level_2

            foreach (var group in progressByLevel)
            {
                string levelKey = group.Key; // contoh "level_1"
                string fkLevel = $"{levelKey}_{db.player[0].nama_player}";
                var progressNames = group.ToList();

                int jumlahProgress = progressNames.Count;

                for (int i = 0; i < jumlahProgress; i++)
                {
                    string playerId = db.player[0].id_player; // selalu ambil player pertama
                    var prog = new Progress
                    {
                        id_progress = db.NextId("progress", playerId),
                        fk_id_level = fkLevel,
                        nama_progress = progressNames[i],
                        jumlah_hadiah_koin = 10, // setiap progress beri hadiah 10 koin
                        is_main = (i == 0) // progress pertama di setiap level selalu bernilai true
                    };
                    db.progress.Add(prog);
                }
            }

            Save(); // Simpan data progress
        }
    }

    public bool HasLevelData()
    {
        return db.level.Count > 0;
    }
    public string GetJenisKelamin()
    {
        return db.player.Count > 0 ? db.player[0].jenis_kelamin : null;
    }

    public void Save()
    {
        var json = JsonUtility.ToJson(db, true);
        File.WriteAllText(FilePath, json);
        #if UNITY_EDITOR
            Debug.Log($"[LevelDataController] Saved -> {FilePath}");
        #endif
    }

    // Cek apakah level sudah dibuka
    public bool IsLevelUnlocked(int levelNumber)
    {
        var player = db.player.Count > 0 ? db.player[0] : null;
        if (player == null) return false;

        var level = GetLevelData(levelNumber);
        if (level == null) return false;

        return level.status_level == 1;
    }

    // Set last level yang dimainkan player
    public void SetLastLevel(int nomorLevel)
    {
        var player = db.player.Count > 0 ? db.player[0] : null;
        if (player == null)
        {
            Debug.LogError("Tidak ada player terdaftar!");
            return;
        }

        var level = GetLevelData(nomorLevel);
        if (level == null)
        {
            Debug.LogWarning($"Level {nomorLevel} tidak ditemukan.");
            return;
        }

        player.fk_id_level = level.id_level;

        // buat Player History juga
        CreatePlayerHistory(player.id_player, level.id_level);

        Save();
    }

    public void CreatePlayerHistory(string playerId, string levelId)
    {
        var phId = db.NextId("history", playerId);
        var ph = new PlayerHistory
        {
            id_history = phId,
            fk_nama_player = playerId,
            fk_id_level = levelId,
        };
        db.player_history.Add(ph);
        Save();
    }

    // Kurangi koin player
    public void KurangiKoin(int jumlah)
    {
        // Jika koin 0 atau mines, tidak perlu dikurangi
        var player = db.player.Count > 0 ? db.player[0] : null;

        if (player != null && player.jumlah_koin > 0)
        {
            player.jumlah_koin = Mathf.Max(0, player.jumlah_koin - jumlah);
            Save();
        }
    }

    // Add Error pada database
    public void AddErrorToDatabase(string id_progress, string tipe, int amount = 1)
    {
        var player = db.player.Count > 0 ? db.player[0] : null;
        if (player == null)
        {
            Debug.LogWarning("Tidak ada player terdaftar, tidak bisa menambahkan error.");
            return;
        }

        AddError(id_progress, tipe, player.id_player, amount);
    }

    public void AddErrorToDatabaseLevel(int nomorLevel, string tipe, int amount = 1)
    {
        var player = db.player.Count > 0 ? db.player[0] : null;
        if (player == null)
        {
            Debug.LogWarning("Tidak ada player terdaftar, tidak bisa menambahkan error.");
            return;
        }

        var id_progress = GetIdProgressIsMainByLevel(nomorLevel);
        if (id_progress == null)
        {
            Debug.LogWarning($"Tidak ditemukan progress utama untuk level {nomorLevel}.");
            return;
        }

        AddError(id_progress, tipe, player.id_player, amount);
    }

    // Get id_progress yang is_main = true pada level tertentu
    public string GetIdProgressIsMainByLevel(int nomorLevel)
    {
        string fkLevel = GetLevelIDByNomor(nomorLevel);
        var progress = db.progress
                        .FirstOrDefault(x => x.fk_id_level == fkLevel && x.is_main);

        if (progress != null)
        {
            return progress.id_progress;
        }
        return null;
    }

    // ---------- HELPER WAKTU ----------
    static string NowIso() => DateTime.UtcNow.ToString("o"); // ISO 8601 UTC
    public string UtcToWib(string utcDateTime)
    {
        DateTime utcTime;
        if (!DateTime.TryParse(utcDateTime, null, System.Globalization.DateTimeStyles.RoundtripKind, out utcTime))
            utcTime = DateTime.UtcNow;

        // WIB = UTC + 7
        DateTime wibTime = utcTime.AddHours(7);

        return wibTime.ToString("yyyy-MM-dd-HH:mm:ss");
    }

    // ---------- API YANG DIPAKAI GAMEPLAY ----------
    // Player dan Login
    public string EnsurePlayer(string nama_player)
    {
        // Jika sudah ada player, langsung return id_player
        if (db.player.Count > 0)
            return db.player[0].id_player;

        // Format id: p_detik:menit:jam:tanggal:tahun
        string waktu = DateTime.Now.ToString("yyyy_dd_HH_mm_ss");
        string pid = $"p_{waktu}";

        var p = new Player
        {
            id_player = pid,
            nama_player = nama_player,
            fk_id_level = null,
            jumlah_koin = 0
        };
        db.player.Add(p);
        Save();
        return p.id_player;
    }

    // Fungsi untuk memulai login
    public string StartLogin(string nama_player)
    {
        string pid = EnsurePlayer(nama_player);
        var loginId = db.NextId("login", pid);
        var lg = new Login
        {
            id_login = loginId,
            fk_nama_player = pid,
            tanggal_login = UtcToWib(NowIso())
        };
        db.login.Add(lg);
        Save();
        return lg.id_login;
    }

    // Fungsi untuk memulai sesi utama
    public string StartMainSession(string playerId)
    {
        var mainId = db.NextId("main", playerId);
        var m = new MainSession
        {
            id_main = mainId,
            waktu_mulai = UtcToWib(NowIso()),
            waktu_berakhir = null
        };
        db.waktu_bermain.Add(m);
        Save();
        return m.id_main;
    }
    // Fungsi untuk mengakhiri sesi utama
    public void EndMainSession(string id_main)
    {
        var m = db.waktu_bermain.Find(x => x.id_main == id_main);
        if (m == null) return;
        m.waktu_berakhir = UtcToWib(NowIso());
        Save();
    }
    // Fungsi untuk memulai pause
    public string StartPause(string playerId, string id_main)
    {
        var pauseId = db.NextId("pause", playerId);
        var ps = new Pause
        {
            id_pause = pauseId,
            waktu_mulai_pause = UtcToWib(NowIso()),
            waktu_akhir_pause = null
        };
        db.pause.Add(ps);
        Save();
        return ps.id_pause;
    }
    // Fungsi untuk mengakhiri pause
    public void EndPause(string id_pause)
    {
        var ps = db.pause.Find(x => x.id_pause == id_pause);
        if (ps == null) return;
        ps.waktu_akhir_pause = UtcToWib(NowIso());
        Save();
    }

    // Membuat progress untuk sesi utama
    public void CreateProgressMain(string playerId, string namaProgress, string loginId, string mainId)
    {
        var progressMainId = db.NextId("progressmain", playerId);
        var progressMain = new ProgressMain
        {
            id_progress_main = progressMainId,
            fk_id_progress = GetProgressIDByName(namaProgress),
            fk_id_login = loginId,
            fk_id_main = mainId,
            status_penyelesaian = 0 // default 0 == belum selesai
        };
        db.progress_main.Add(progressMain);
        Save();
    }

    // Fungsi untuk mendapatkan id player
    public string GetPlayerID()
    {
        return db.player.Count > 0 ? db.player[0].id_player : null;
    }

    // Fungsi untuk set Complete Level
    public bool IsLevelCompleted(string playerId, int nomorGameplay)
    {
        if (nomorGameplay == 3)
        {
            // Jika nomorGameplay 1, maka level 1 progress 1
            var level1 = db.level.Find(x => x.nama_level == "level_1");
            if (level1 == null) return false;

            // Ambil id_login terbaru
            var login = GetLatestLoginData(playerId);

            // Tambahkan complete play
            CompleteLevel(level1.id_level, login.id_login, playerId);
            return true;
        } 

        if (nomorGameplay == 6)
        {
            // Jika nomorGameplay 2, maka level 2 progress 1
            var level2 = db.level.Find(x => x.nama_level == "level_2");
            if (level2 == null) return false;

            // Ambil id_login terbaru
            var login = GetLatestLoginData(playerId);

            // Tambahkan complete play
            CompleteLevel(level2.id_level, login.id_login, playerId);
            return true;
        }

        if (nomorGameplay == 8)
        {
            // Jika nomorGameplay 3, maka level 3 progress 1
            var level3 = db.level.Find(x => x.nama_level == "level_3");
            if (level3 == null) return false;

            // Ambil id_login terbaru
            var login = GetLatestLoginData(playerId);

            // Tambahkan complete play
            CompleteLevel(level3.id_level, login.id_login, playerId);
            return true;
        }

        if (nomorGameplay == 10)
        {
            // Jika nomorGameplay 4, maka level 4 progress 1
            var level4 = db.level.Find(x => x.nama_level == "level_4");
            if (level4 == null) return false;

            // Ambil id_login terbaru
            var login = GetLatestLoginData(playerId);

            // Tambahkan complete play
            CompleteLevel(level4.id_level, login.id_login, playerId);
            return true;
        }

        if (nomorGameplay == 12)
        {
            // Jika nomorGameplay 5, maka level 5 progress 1
            var level5 = db.level.Find(x => x.nama_level == "level_5");
            if (level5 == null) return false;

            // Ambil id_login terbaru
            var login = GetLatestLoginData(playerId);

            // Tambahkan complete play
            CompleteLevel(level5.id_level, login.id_login, playerId);
            return true;
        }

        return false;
    }

    // Fungsi untuk menandai level telah selesai
    public void CompleteLevel(string id_level, string id_login, string playerId)
    {
        var pr = db.progress.Find(x => x.fk_id_level == id_level);

        var completeId = db.NextId("complete", playerId);
        var cp = new CompletePlay
        {
            id_complete_level = completeId,
            fk_id_level = id_level,
            fk_nama_player = playerId,
            fk_id_login = id_login,
            waktu_penyelesaian = UtcToWib(NowIso())
        };
        db.complete_play.Add(cp);

        Save();
    }

    // Kesalahan (per progress, per tipe)
    public KesalahanPlay GetOrCreateKesalahan(string id_progress, string tipe, string playerId)
    {
        var k = db.kesalahan_play.Find(
            x =>
            x.fk_id_progress == id_progress &&
            x.tipe_kesalahan == tipe
        );
        if (k != null) return k;

        var errorId = db.NextId("error", playerId);
        k = new KesalahanPlay
        {
            id_kesalahan = errorId,
            fk_id_progress = id_progress,
            tipe_kesalahan = tipe,
            jumlah_kesalahan = 0
        };
        db.kesalahan_play.Add(k);
        Save();
        return k;
    }

    public void AddError(string id_progress, string tipe, string playerId, int amount = 1)
    {
        var k = GetOrCreateKesalahan(id_progress, tipe, playerId);
        k.jumlah_kesalahan += Mathf.Max(1, amount);
        Save();
    }

    // Get nama player
    public string GetPlayerName()
    {
        var player = db.player.Count > 0 ? db.player[0] : null;
        return player != null ? player.nama_player : "Unknown Player";
    }

    // Get data level sesuai level
    public Level GetLevelData(int nomorLevel)
    {
        if (nomorLevel < 1 || nomorLevel > db.level.Count)
        {
            Debug.LogWarning($"Level {nomorLevel} tidak ditemukan.");
            return null;
        }
        return db.level[nomorLevel - 1]; // karena nomor level mulai dari 1
    }

    // Ambil data login terbaru
    public Login GetLatestLoginData(string playerId)
    {
        return db.login
            .Where(x => x.fk_nama_player == playerId)
            .OrderByDescending(x => x.tanggal_login)
            .FirstOrDefault();
    }

    // Ambil data progress berdasarkan
    public string GetProgressIDByName(string nama_progress)
    {
        return db.progress
            .Where
            (x =>
                x.nama_progress == nama_progress
            )
            .Select(x => x.id_progress)
            .FirstOrDefault();
    }

    // Ambil data progress berdasarkan nama_progress
    public List<Progress> GetProgressDataByLevel(int level)
    {
        return db.progress
            .Where(x => x.fk_id_level == GetLevelIDByNomor(level))
            .ToList();
    }

    public string GetLevelIDByNomor(int nomorLevel)
    {
        var level = GetLevelData(nomorLevel);

        if (level == null || string.IsNullOrEmpty(level.id_level))
        {
            Debug.LogWarning($"Level {nomorLevel} tidak ditemukan atau id_level kosong.");
            return null;
        }

        return level.id_level;
    }

    // Ambil ID paling terbaru
    public MainSession GetLatestMainSessionData()
    {
        return db.waktu_bermain
            .OrderByDescending(x => x.waktu_mulai)
            .FirstOrDefault();
    }

    // Get Haduah koin progress
    public int GetHadiahKoinProgress(string id_progress)
    {
        var prog = db.progress.Find(x => x.id_progress == id_progress);
        return prog != null ? prog.jumlah_hadiah_koin : 10;
    }

    // Get status penyelesaian progress main
    public int GetStatusPenyelesaianProgressMain(string id_progress)
    {
        var pm = db.progress_main.Find(x => x.fk_id_progress == id_progress);
        return pm != null ? pm.status_penyelesaian : 0;
    }

    // Fungsi Update status penyelesaian progress main
    public void UpdateStatusPenyelesaianProgressMain(string id_progress, string id_main, int status)
    {
        var pm = db.progress_main.Find(x => x.fk_id_progress == id_progress && x.fk_id_main == id_main);
        if (pm != null)
        {
            pm.status_penyelesaian = status;
            Save();
        }
    }

    // Get MainSession per level
    public List<MainSession> GetMainSessionDataByLevel(int level)
    {
        // Dapatkan data progress main dari level tertentu
        List<ProgressMain> progressMain = GetProgressMainDataByLevel(level);

        // Dapatkan daftar id main dari progressMain
        List<string> mainIds = progressMain
            .Select(pm => pm.fk_id_main)
            .Distinct()
            .ToList();

        // Kembalikan semua MainSession (lebih dari 1)
        return db.waktu_bermain
            .Where(x => mainIds.Contains(x.id_main))
            .ToList();
    }

    // Get ProgressMain per level
    public List<ProgressMain> GetProgressMainDataByLevel(int level)
    {
        // Ambil data progress per level
        List<Progress> progressList = GetProgressDataByLevel(level);

        // Ambil data progress main dari progressList
        return db.progress_main
            .Where(x => progressList.Select(p => p.id_progress).Contains(x.fk_id_progress))
            .ToList();
    }

    // Fungsi Update koin player
    public void UpdateKoinPlayer(int jumlah_koin)
    {
        var player = db.player.Count > 0 ? db.player[0] : null;
        if (player != null)
        {
            player.jumlah_koin = jumlah_koin;
            Save();
        }
    }

    // Fungsi untuk ke progress berikutnya update is_main pada progress
    public void SetProgressMainIsMain(string id_progress, bool isMain)
    {
        var prog = db.progress.Find(x => x.id_progress == id_progress);
        if (prog != null)
        {
            prog.is_main = isMain;

            // update progress selanjutnya dari yang indeks prog yang telah di set is_main = false
            if (!isMain)
            {
                int currentIndex = db.progress.IndexOf(prog);
                if (currentIndex >= 0 && currentIndex < db.progress.Count - 1)
                {
                    db.progress[currentIndex + 1].is_main = true;
                }
            }

            Save();
        }
    }

    // Fugnsi set progress main is_main = true
    public void SetProgressMain(string id_progress, bool isMain)
    {
        var prog = db.progress.Find(x => x.id_progress == id_progress);
        if (prog != null)
        {
            prog.is_main = isMain;
            Save();
        }
    }

    // fungsi untuk mendapatkan progress yang sedang is_main = true
    public bool GetProgressIsMainByLevelAndProgress(int nomorLevel, string namaProgress)
    {
        string fkLevel = GetLevelIDByNomor(nomorLevel);
        var progress = db.progress
                        .FirstOrDefault(x => x.fk_id_level == fkLevel && x.nama_progress == namaProgress);

        if (progress != null)
        {
            return progress.is_main;
        }

        // Jika tidak ditemukan, bisa return false atau throw exception sesuai kebutuhan
        return false;
    }

    // Ambil status is_main berdasarkan level dan nama progress
    public bool GetIsMainByLevelAndProgress(int nomorLevel, string namaProgress)
    {
        string fkLevel = GetLevelIDByNomor(nomorLevel);
        var progress = db.progress
                        .FirstOrDefault(x => x.fk_id_level == fkLevel && x.nama_progress == namaProgress);

        if (progress != null)
        {
            return progress.is_main;
        }

        // Jika tidak ditemukan, bisa return false atau throw exception sesuai kebutuhan
        return false;
    }

    // Fungsi get koin player
    public int GetKoinPlayer()
    {
        var player = db.player.Count > 0 ? db.player[0] : null;
        return player != null ? player.jumlah_koin : 0;
    }

    // Fungsi untuk mendapatkan total koin di level
    public int GetHargaUnlockLevel(int nomorLevel)
    {
        var level = GetLevelData(nomorLevel);
        return level != null ? level.total_koin : 1000;
    }

    // Fungsi untuk membuka level
    public void UnlockLevel(int nomorLevel)
    {
        var level = GetLevelData(nomorLevel);
        if (level != null && level.status_level == 0)
        {
            level.status_level = 1; // buka level
            Save();
        }
    }

    // fungsi set is_main pada progress di level tertentu
    public void SetIsMainLevel(int nomorLevel)
    {
        var level = GetLevelData(nomorLevel);
        if (level == null)
        {
            Debug.LogWarning($"Level {nomorLevel} tidak ditemukan.");
            return;
        }

        // Set semua progress di level ini is_main = false kecuali yang pertama
        foreach (var prog in db.progress.Where(x => x.fk_id_level == level.id_level))
        {
            prog.is_main = false;
        }

        // Set progress pertama di level ini is_main = true
        var firstProg = db.progress.FirstOrDefault(x => x.fk_id_level == level.id_level);
        if (firstProg != null)
        {
            firstProg.is_main = true;
        }

        Save();
    }

    // Update waktu selesai progress main
    public void UpdateWaktuSelesaiProgressMain(string id_progress)
    {
        // Cari progress main berdasarkan id_progress yang diberikan
        // Cari id_progress_main yang paling baru (terakhir) untuk id_progress tersebut
        var progressMain = db.progress_main
            .Where(pm => pm.fk_id_progress == id_progress)
            .OrderByDescending(pm => pm.id_progress_main) // Asumsi id_progress_main mengandung informasi urutan
            .FirstOrDefault();

        // Cari Main Session terkait
        var mainSession = progressMain != null ? db.waktu_bermain.Find(
            wm => wm.id_main == progressMain.fk_id_main) : null;

        if (mainSession != null && progressMain != null)
        {
            // Update waktu selesai progress main
            mainSession.waktu_berakhir = UtcToWib(NowIso());
            Save();
        }
        else
        {
            Debug.LogWarning($"ProgressMain untuk id_progress '{id_progress}' tidak ditemukan.");
        }
    }

    // ---------- UTIL UNTUK LEVEL ----------
    // Start level 
    public void StartLevel_OnClick(int nomorLevel, string namaProgress)
    {
        // Ambil data player
        var player = db.player.Count > 0 ? db.player[0] : null;
        if (player == null)
        {
            Debug.LogError("Tidak ada player terdaftar!");
            return;
        }

        // Ambil data level
        var lvl = GetLevelData(nomorLevel);
        if (lvl == null)
        {
            Debug.LogWarning($"Data Level {nomorLevel} tidak ditemukan.");
            return;
        }

        // Ambil data login terbaru
        var login = GetLatestLoginData(player.id_player);
        if (login == null)
        {
            Debug.LogError("Belum ada login session untuk player ini!");
            return;
        }

        // Buat main session baru (bukan nama, tapi id player)
        string mainId = StartMainSession(player.id_player);
        var main = db.waktu_bermain.Find(x => x.id_main == mainId);

        // Buat Relasi antara Progress dan Main Session
        CreateProgressMain(player.id_player, namaProgress, login.id_login, mainId);
    }

#if UNITY_EDITOR
    [ContextMenu("Load Data")]
    public void LoadDataLevelEditor()
    {
        if (File.Exists(FilePath))
        {
            db = JsonUtility.FromJson<DbRoot>(File.ReadAllText(FilePath));
            Debug.Log("Data level berhasil di-load di editor.");
        }
        else
        {
            Debug.LogWarning("File data level belum ada.");
        }
    }
    [ContextMenu("Save Data (From Inspector)")]
    public void SaveDataFromInspector()
    {
        Save();
        Debug.Log("Data dari inspector berhasil disimpan ke file: " + FilePath);
    }
    [ContextMenu("Clear Data")]
    public void ClearData()
    {
        db = new DbRoot();
        Save();
        Debug.Log("Data level telah dihapus.");
    }
#endif
}
