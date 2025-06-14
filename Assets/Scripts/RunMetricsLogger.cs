using System.IO;
using UnityEngine;

/// <summary>
/// Utility for writing RunMetrics to a CSV file in Application.persistentDataPath.
/// </summary>
public class RunMetricsLogger : MonoBehaviour
{
    private static string CsvPath => Path.Combine(Application.persistentDataPath, "Metrics.csv");

    /// <summary>
    /// Appends one line of run metrics to the CSV file.
    /// </summary>
    public static void WriteMetrics(int pupEarned)
    {
        // Ensure latest metrics are recorded
        RunMetrics.Instance?.FinalizeMetrics();
        var m = RunMetrics.Instance;
        if (m == null)
            return;

        string header = "Kills,DamageTaken,TimeInChase,TimeInAttack,ShotsFired,ShotsHit,AvgKillTime,PUPEarned";
        string line = $"{m.kills},{m.damageTaken},{m.timeInChase:F2},{m.timeInAttack:F2},{m.shotsFired},{m.shotsHit},{m.AverageKillTime:F2},{pupEarned}";

        bool exists = File.Exists(CsvPath);
        using (var writer = new StreamWriter(CsvPath, append: true))
        {
            if (!exists)
                writer.WriteLine(header);
            writer.WriteLine(line);
        }
    }
}
