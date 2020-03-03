namespace LambdaSurprise.Services.OptimizelySdk
{
    public class Variant
    {
        public string Value { get; set; }

        public static implicit operator string(Variant v) => v.Value; 
        
        public override string ToString()
        {
            return Value; 
        }
    }
}