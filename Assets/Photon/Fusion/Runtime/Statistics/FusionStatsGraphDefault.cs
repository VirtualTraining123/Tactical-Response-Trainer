using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Fusion.Statistics {
  public class FusionStatsGraphDefault : FusionStatsGraphBase {
    [SerializeField] private Text _descriptionText;

    private readonly Dictionary<RenderSimStats, string> _statsAdditionalInfo = new() {
      { RenderSimStats.InPackets, "(Per second)" },
      { RenderSimStats.OutPackets, "(Per second)" },
      { RenderSimStats.InObjectUpdates, "(Per second)" },
      { RenderSimStats.OutObjectUpdates, "(Per second)" },
      { RenderSimStats.InBandwidth, "(Per second)" },
      { RenderSimStats.OutBandwidth, "(Per second)" },
      { RenderSimStats.InputInBandwidth, "(Per second)" },
      { RenderSimStats.InputOutBandwidth, "(Per second)" },
      { RenderSimStats.StateReceiveDelta, "(Per second)" },
      { RenderSimStats.WordsWrittenSize, "(Per second)" },
      { RenderSimStats.WordsWrittenCount, "(Per second)" },
      { RenderSimStats.WordsReadCount, "(Per second)" },
      { RenderSimStats.WordsReadSize, "(Per second)" }
    };

    internal RenderSimStats Stat { get; private set; }

    protected override void Initialize(int accumulateTimeMs) {
      base.Initialize(accumulateTimeMs);
      _descriptionText.text = Stat.ToString();
      if (_statsAdditionalInfo.TryGetValue(Stat, out var info)) _descriptionText.text += $" {info}";
    }

    public override void UpdateGraph(NetworkRunner runner, FusionStatisticsManager statisticsManager,
      ref DateTime now) {
      var value = FusionStatisticsHelper.GetStatDataFromSnapshot(Stat, statisticsManager.CompleteSnapshot);
      AddValueToBuffer(value, ref now);
    }

    public virtual void ApplyCustomStatsConfig(FusionStatistics.FusionStatisticsStatCustomConfig config) {
      SetThresholds(config.Threshold1, config.Threshold2, config.Threshold3);
      SetIgnoreZeroValues(config.IgnoreZeroOnAverageCalculation, config.IgnoreZeroOnBuffer);
      SetAccumulateTime(config.AccumulateTimeMs);
    }

    internal void SetupDefaultGraph(RenderSimStats stat) {
      Stat = stat;

      FusionStatisticsHelper.GetStatGraphDefaultSettings(
        Stat,
        out var valueTextFormat,
        out var valueTextMultiplier,
        out var ignoreZeroOnAverage,
        out var ignoreZeroOnBuffer,
        out var bufferTimeSpan
      );

      SetValueTextFormat(valueTextFormat);
      SetValueTextMultiplier(valueTextMultiplier);
      SetIgnoreZeroValues(ignoreZeroOnAverage, ignoreZeroOnBuffer);
      Initialize(bufferTimeSpan);
    }
  }
}