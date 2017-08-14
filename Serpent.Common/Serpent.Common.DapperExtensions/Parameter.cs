using System.Collections.Generic;
using System.Data;
using Dapper;

namespace Serpent.Common.DapperExtensions
{
    public struct Parameter
    {
        public Parameter(string name, object value, DbType? dbType, ParameterDirection? direction, int? size, byte? precision, byte? scale)
        {
            this.Name = name;
            this.Value = value;
            this.DbType = dbType;
            this.Direction = direction;
            this.Size = size;
            this.Precision = precision;
            this.Scale = scale;
        }

        public Parameter(string name, object value)
        {
            this.Name = name;
            this.Value = value;
            this.Value = null;
            this.DbType = null;
            this.Direction = null;
            this.Size = null;
            this.Precision = null;
            this.Scale = null;
        }

        public string Name { get; set; }

        public object Value { get; set; }

        public DbType? DbType { get; set; }

        public ParameterDirection? Direction { get; set; }

        public int? Size { get; set; }

        public byte? Precision { get; set; }

        public byte? Scale { get; set; }

    }
}
