namespace Wasalnyy.BLL.Validators
{
    public class RouteServiceValidator
    {
        public async Task ValidateCalculateDistanceAndDuration(Coordinates start, Coordinates end)
        {
            ArgumentNullException.ThrowIfNull(start);
            ArgumentNullException.ThrowIfNull(end);
        }
    }
}
