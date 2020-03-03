namespace LambdaSurprise.Services.OptimizelySdk
{
    public interface IOptimizelyWrapper
    {
        string GetExperimentVariant(string experimentKey, object uniqueId);
    }
}