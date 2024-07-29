using Bookify.Domain.Apartments;
using Bookify.Domain.Shared;

namespace Bookify.Domain.Bookings;
public class PricingService
{
    public PricingDetails CalculatePrice(Apartment apartment, DateRange period)
    {
        var currency = apartment.Price.Currency;

        var priceForPeriod = new Money(apartment.Price.Amount * period.LengthInDays, currency);


        decimal percentageUpCharge = 0;

        foreach (var amenity in apartment.Amenities)
        {
            percentageUpCharge += amenity switch
            {
                Amenity.MountainView or Amenity.MountainView => 0.5m, // 5% up charge
                Amenity.AirConditioning => 0.01m, // 1% up charge.
                Amenity.Parking => 0.01m,
                _ => 0
            };
        }


        var amenitiesUpCharge = Money.Zero(currency);

        if (percentageUpCharge > 0)
        {
            amenitiesUpCharge = new Money(priceForPeriod.Amount * percentageUpCharge, currency);
        }


        var totalPrice = priceForPeriod + amenitiesUpCharge;

        if (!apartment.CleaningFee.IsZero())
        {
            totalPrice += apartment.CleaningFee;
        }

        return new PricingDetails(priceForPeriod, apartment.CleaningFee, amenitiesUpCharge, totalPrice);
    }
}
