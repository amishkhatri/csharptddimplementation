using DeskBooker.Core.Domain;
using DeskBooker.Core.DataInterface;
using System;
using System.Linq;

namespace DeskBooker.Core.Processor
{
    public class DeskBookingRequestProcessor
    {
        private readonly IDeskBookingRepository _deskBookingRepository;
        private readonly IDeskRepository _deskRepository;

        public DeskBookingRequestProcessor()
        {

        } 

        public DeskBookingRequestProcessor(IDeskBookingRepository deskBookingRepository, 
            DataInterface.IDeskRepository deskRepository)
        {
            this._deskBookingRepository = deskBookingRepository;
            this._deskRepository = deskRepository;
        }

        public DeskBookingResult BookDesk(DeskBookingRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var availableDesks = _deskRepository.GetAvailableDesk(request.Date);

            var result = Create<DeskBookingResult>(request);

            if (availableDesks.FirstOrDefault() is Desk availbleDesk)
            { 
              //  var availableDesk = availableDesks.First();
                var deskBooking = Create<DeskBooking>(request);
                deskBooking.DeskId = availbleDesk.Id;
                _deskBookingRepository.Save(deskBooking);
                result.Code = DeskBookingResultCode.Success;
            }
            else
            {
                result.Code = DeskBookingResultCode.NoDeskAvailable;
            }

            //throw new NotImplementedException();
            return result;
            
        }

        private static T Create<T>(DeskBookingRequest request) where T : DeskBookingBase, new()
        {
            return new T
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Date = request.Date,
            };
        }
    }
}