namespace ResultSaver {
  public class EvaluationResult {
    public EvaluationResult(float time,
      int injuredCivilians,
      int missingEnemies,
      int extraBulletsUsed,
      float finalScore,
      bool agentDeath,
      int safetyOff
    ) {
      Time = time;
      InjuredCivilians = injuredCivilians;
      MissingEnemies = missingEnemies;
      ExtraBulletsUsed = extraBulletsUsed;
      FinalScore = finalScore;
      AgentDeath = agentDeath;
      SafetyOff = safetyOff;
    }

    public float Time { get; }
    public int InjuredCivilians { get; }
    public int MissingEnemies { get; }
    public int ExtraBulletsUsed { get; }
    public float FinalScore { get; }
    public bool AgentDeath { get; }
    public int SafetyOff { get; }

    public void Deconstruct(
      out float time,
      out int injuredCivilians,
      out int missingEnemies,
      out int extraBulletsUsed,
      out float finalScore,
      out bool agentDeath,
      out int safetyOff
    ) {
      time = Time;
      injuredCivilians = InjuredCivilians;
      missingEnemies = MissingEnemies;
      extraBulletsUsed = ExtraBulletsUsed;
      finalScore = FinalScore;
      agentDeath = AgentDeath;
      safetyOff = SafetyOff;
    }
  }
}