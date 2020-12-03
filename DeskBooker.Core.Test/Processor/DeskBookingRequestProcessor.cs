using System;
using DeskBooker.Core.Domain;

namespace DeskBooker.Core.Processor
{
    public class DeskBookingRequestProcessor
    {
        public DeskBookingRequestProcessor(IDeskBookingRepository @object)
        {
        }

        public DeskBookingResult BookDesk(DeskBookingRequest request)
        {
            //throw new NotImplementedException();
            return new DeskBookingResult
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Date = request.Date,
            };
        }
    }
}