using RR.Core.DTOs.Requests;
using RR.Core.Entities;

namespace RR.Core.Services
{
    public interface IReservationSeriesService
    {
        Task<int> CreateSeries(CreateSeriesRequest series);
        Task<List<Reservation>> PreviewSeries(PreviewSeriesRequest series);
        Task<int> EditSeries(EditSeriesRequest series);
        Task<int> CancelSeries(int seriesId, DateTime? from = null);
    }
}
