using System.Threading.Tasks;

namespace Results {
  public interface IResultManager {
    public Task<EvaluationResult> LoadResult();
    public void SaveResult(EvaluationResult result);

    public void Clear();

  }
}