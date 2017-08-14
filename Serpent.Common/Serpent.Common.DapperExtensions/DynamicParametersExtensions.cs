namespace Serpent.Common.DapperExtensions
{
    public static class DynamicParametersExtensions
    {
        public static Dapper.DynamicParameters AddEx(this Dapper.DynamicParameters dynamicParameters, string name, object value)
        {
            dynamicParameters.Add(name, value);
            return dynamicParameters;
        }
    }
}