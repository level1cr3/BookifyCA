using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Domain.Bookings;
public record DateRange
{
    private DateRange(DateOnly start,DateOnly end)
    {
        Start = start; 
        End = end;
    }

    public DateOnly Start { get; init; }
    
    public DateOnly End { get; init; }

    public int LengthInDays => End.DayNumber - Start.DayNumber;

    public static DateRange Create(DateOnly start, DateOnly end) 
    {

        if (start > end)
        {
            throw new ApplicationException("End date precedes start date");
        }

        return new DateRange(start,end);
    }

}
