using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class DbRoot
{
    // ====== DATA LIST ======
    public List<Player> player = new();
    public List<Login> login = new();
    public List<Level> level = new();
    public List<MainSession> waktu_bermain = new();
    public List<Pause> pause = new();
    public List<KesalahanPlay> kesalahan_play = new();
    public List<Progress> progress = new();
    public List<CompletePlay> complete_play = new();
    public List<PlayerHistory> player_history = new();
    public List<MainPause> main_pause = new();
    public List<ProgressMain> progress_main = new();

    /// Format : p_{mm:ss:dd} (biar unik antar user)
    public string GeneratePlayerId(string playerName, DateTime? utc = null)
    {
        var t = (utc ?? DateTime.UtcNow);
        var tag = t.ToString("mm':'ss':'dd"); // mm:ss:dd
        return $"p_{tag}";
    }

    /// Auto-increment per player & per entity.
    /// Hasil: {prefix}_{playerId}_{index 3 digit}
    /// Contoh: login_p_34:12:07_000
    public string NextId(string entityPrefix, string playerId)
    {
        string prefix = $"{entityPrefix}_{playerId}_";

        // hitung jumlah existing id yang sudah pakai prefix ini
        int next = entityPrefix switch
        {
            "login"        => login.Count(x => x.id_login.StartsWith(prefix, StringComparison.Ordinal)),
            "progress"     => progress.Count(x => x.id_progress.StartsWith(prefix, StringComparison.Ordinal)),
            "main"         => waktu_bermain.Count(x => x.id_main.StartsWith(prefix, StringComparison.Ordinal)),
            "pause"        => pause.Count(x => x.id_pause.StartsWith(prefix, StringComparison.Ordinal)),
            "error"        => kesalahan_play.Count(x => x.id_kesalahan.StartsWith(prefix, StringComparison.Ordinal)),
            "complete"     => complete_play.Count(x => x.id_complete_level.StartsWith(prefix, StringComparison.Ordinal)),
            "history"      => player_history.Count(x => x.id_history.StartsWith(prefix, StringComparison.Ordinal)),
            "mainpause"    => main_pause.Count(x => x.id_main_pause.StartsWith(prefix, StringComparison.Ordinal)),
            "progressmain"  => progress_main.Count(x => x.id_progress_main.StartsWith(prefix, StringComparison.Ordinal)),
            _              => 0
        };

        return $"{entityPrefix}_{playerId}_{next:D5}";
    }
}
