using Dapper;
using System.Data;

namespace Bookify.Infrastructure.Data;
internal sealed class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
{
    public override DateOnly Parse(object value) => DateOnly.FromDateTime((DateTime)value);

    public override void SetValue(IDbDataParameter parameter, DateOnly value)
    {
        parameter.DbType = DbType.Date;
        parameter.Value = value;    
    }
}


// because we are using DateOnly types in DateRange value object. We need to tell dapper how to be able to map this type. Because it doesn't support it 
// out of the box.