using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ReservaRussasAPI.Controllers.Base;
using RR.Core.DTOs.Requests;
using RR.Core.Services;
namespace ReservaRussasAPI.Controllers
{
    public class ReservationSeriesController: BaseControllerFYP
    {
        private readonly IReservationSeriesService _reservationSeriesService;
    
        public ReservationSeriesController(IReservationSeriesService reservationSeriesService) => _reservationSeriesService = reservationSeriesService;

        [HttpDelete]
        [Authorize (Policy = "ServantOrAbove")]
        public async Task<IActionResult> CancelSeries(int seriesId, DateTime? from = null)
        {
            var cancelledCount = await _reservationSeriesService.CancelSeries(seriesId, from);
            if (cancelledCount == 0)
                return ResponseNotFound("No reservations found to cancel in the specified series.");
    
            return ResponseOk(cancelledCount);
        }

        [HttpPost]
        [Authorize (Policy = "ServantOrAbove")]
        public async Task<IActionResult> CreateSeries([FromBody] CreateSeriesRequest req)
        {
            if (req is null) return ResponseBadRequest("O corpo da requisição é nulo.");
            if (req.WindowStart >= req.WindowEnd)
                return ResponseBadRequest("O começo da janela deve ser anterior ao fim.");
            if (req.TimeStart >= req.TimeEnd)
                return ResponseBadRequest("Ínicio da ocorrência deve ser anterior ao fim.");
            if (req.DaysOfWeek == null || !req.DaysOfWeek.Any())
                return ResponseBadRequest("Tem que ocorrer em pelo menos um dia da semana.");
           
            var seriesId = await _reservationSeriesService.CreateSeries(req);
            return ResponseOk(seriesId);
        }

        [HttpPut]
        [Authorize (Policy = "ServantOrAbove")]
        public async Task<IActionResult> EditSeries([FromBody] EditSeriesRequest req)
        {
            if (req is null) return ResponseBadRequest();
            if (req.WindowStart.HasValue && req.WindowEnd.HasValue && req.WindowStart >= req.WindowEnd)
                return ResponseBadRequest("O começo da janela deve ser anterior ao fim.");
            if (req.TimeStart.HasValue && req.TimeEnd.HasValue && req.TimeStart >= req.TimeEnd)
                return ResponseBadRequest("Ínicio da ocorrência deve ser anterior ao fim.");
            if (req.DaysOfWeek != null && !req.DaysOfWeek.Any())
                return ResponseBadRequest("Tem que ocorrer em pelo menos um dia da semana.");
           
            var updatedCount = await _reservationSeriesService.EditSeries(req);
            if (updatedCount == 0)
                return ResponseNotFound("Nenhuma reserva foi encontrada no update da série.");
    
            return ResponseOk(updatedCount);
        }

        [HttpPost("preview")]
        [Authorize (Policy = "ServantOrAbove")]
        public async Task<IActionResult> PreviewSeries([FromBody] PreviewSeriesRequest req)
        {
            if (req is null) return ResponseBadRequest("O corpo da requisição é nulo.");
            if (req.WindowStart >= req.WindowEnd)
                return ResponseBadRequest("O começo da janela deve ser anterior ao fim.");
            if (req.TimeStart >= req.TimeEnd)
                return ResponseBadRequest("Ínicio da ocorrência deve ser anterior ao fim.");
            if (req.DaysOfWeek == null || !req.DaysOfWeek.Any())
                return ResponseBadRequest("Tem que ocorrer em pelo menos um dia da semana.");
           
            var preview = await _reservationSeriesService.PreviewSeries(req);
            return ResponseOk(preview);
        }
    }
}
