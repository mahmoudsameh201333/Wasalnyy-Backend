namespace Wasalnyy.BLL.DTO.Trip
{
    public class TripPaginationDto
    {
        public IEnumerable<TripDto>? Trips { get; set; }
        public required int CurrentPage { get; set; }
        public required int PageSize { get; set; }
        public required int TotalPages { get; set; }
    }
}
