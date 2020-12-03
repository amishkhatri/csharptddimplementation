using System;
using System.Collections.Generic;
using System.Text;


namespace DeskBooker.Core.Domain
{
    public interface IDeskBookingRepository
    {

        void Save(DeskBooker.Core.Domain.DeskBooking deskBooking);
    }
}
