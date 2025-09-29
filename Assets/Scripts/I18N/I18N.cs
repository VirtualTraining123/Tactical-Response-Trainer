namespace I18N {
  public class I18N {
    public static string Get(bool value) {
      return value ? "Sí" : "No";
    }

    public static string GetPassedText() => "Actividad aprobada. ¡Felicidades!";
    public static string GetFailedText() => "Actividad no aprobada. Revise sus errores e intente nuevamente.";
  }
}
