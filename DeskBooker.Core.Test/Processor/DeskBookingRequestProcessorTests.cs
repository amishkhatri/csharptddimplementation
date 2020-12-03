using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using DeskBooker.Core.Domain;
using Moq;
using DeskBooker.Core.DataInterface;


namespace DeskBooker.Core.Processor
{
   public class DeskBookingRequestProcessorTests
    {
        private readonly DeskBookingRequest _request;
        private readonly List<Desk> _availableDesks;
        private readonly Mock<IDeskBookingRepository> _deskBookingRepositoryMock;
        private readonly Mock<IDeskRepository> _deskRepositoryMock;
        private readonly DeskBooker.Core.Processor.DeskBookingRequestProcessor _processor;

        public DeskBookingRequestProcessorTests()
        {
             _request = new DeskBookingRequest
            {
                FirstName = "Amish",
                LastName = "Khatri", 
                Email = "a@a.com",
                Date = new DateTime(2020, 12, 1)
            };

            _availableDesks = new List<Desk> { new Desk { Id = 2 } };

            _deskBookingRepositoryMock = new Mock<IDeskBookingRepository>();

            _deskRepositoryMock = new Mock<IDeskRepository>();

            _deskRepositoryMock.Setup(x=> x.GetAvailableDesk(_request.Date))
                .Returns(_availableDesks);
            
            _processor = new DeskBookingRequestProcessor(
                _deskBookingRepositoryMock.Object, _deskRepositoryMock.Object);
        }

        [Fact]
        public void ShouldReturnDeskBookingResultWithRequiredValues()
        {
            //Arrnage
            

           //var Processor = new DeskBookingRequestProcessor();

            //Act

                       
            DeskBookingResult result = _processor.BookDesk(_request);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(_request.FirstName, result.FirstName);
            Assert.Equal(_request.LastName,result.LastName);
            Assert.Equal(_request.Email, result.Email);
            Assert.Equal(_request.Date, result.Date);

        }

        [Fact]
        public void ShouldThrowExceptionIfRequestisNull()
        {            
            var ex =  Assert.Throws<ArgumentNullException>(() => _processor.BookDesk(null));
            Assert.Equal("request", ex.ParamName);
        }

        [Fact]
        public void ShouldSaveBooking()
        {
            DeskBooking savedDeskBooking = null;
            _deskBookingRepositoryMock.Setup(x => x.Save(It.IsAny<DeskBooking>()))
                .Callback<DeskBooking>(deskBooking =>
                {
                    savedDeskBooking = deskBooking;
                }
                );
            _processor.BookDesk(_request);

            _deskBookingRepositoryMock.Verify(x=>x.Save(It.IsAny<DeskBooking>()),Times.Once);


            Assert.NotNull(savedDeskBooking);
            Assert.Equal(_request.FirstName, savedDeskBooking.FirstName);
            Assert.Equal(_request.LastName, savedDeskBooking.LastName);
            Assert.Equal(_request.Email, savedDeskBooking.Email);
            Assert.Equal(_request.Date, savedDeskBooking.Date);
            Assert.Equal(_availableDesks.First().Id, savedDeskBooking.DeskId);
        }

        [Fact]
        public void ShouldNotSaveDeskBookingIfNoDeskIsAvailable()
        {
            _availableDesks.Clear();

            _processor.BookDesk(_request);

            _deskBookingRepositoryMock.Verify(x => x.Save(It.IsAny<DeskBooking>()), Times.Never);
        }

        [Theory]
        [InlineData(DeskBookingResultCode.Success,true)]
        [InlineData(DeskBookingResultCode.NoDeskAvailable, false)]
        public void ShouldReturnExpectedResultCode
            (DeskBookingResultCode expectedResultCode,bool IsDeskAvailable)
        {
            if(!IsDeskAvailable)
            {
                _availableDesks.Clear();
            }

           var result= _processor.BookDesk(_request);

            Assert.Equal(expectedResultCode, result.Code);
        }

        [Theory]
        [InlineData(5, true)]
        [InlineData(null, false)]
        public void ShouldReturnExpectedDeskBookingId
         (int? expectedDeskBookingId, bool IsDeskAvailable)
        { 
            if (!IsDeskAvailable)
            {
                _availableDesks.Clear();
            }
            else
            {
                _deskBookingRepositoryMock.Setup(x => x.Save(It.IsAny<DeskBooking>()))
                   .Callback<DeskBooking>(deskBooking =>
                   {
                       deskBooking.Id = expectedDeskBookingId.Value;
                   }
                   );
            }

            var result = _processor.BookDesk(_request);

            Assert.Equal(expectedDeskBookingId, result.DeskBookingId);
        }
    }
}
