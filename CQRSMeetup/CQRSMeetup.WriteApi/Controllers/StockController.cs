using CQRSMeetup.WriteApi.Features.Stock.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CQRSMeetup.WriteApi.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/stocks")]
    public class StockController : Controller
    {
        private readonly IMediator _mediator;

        public StockController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPatch("change-stock-status")]
        [ProducesResponseType( 200)]
        public async Task<ActionResult> ChangeStockStatus([FromBody] ChangeStockStatusCommand model)
        {
            await _mediator.Send(model);
            return Ok();
        }

        [HttpPut("entry-product")]
        public async Task<ActionResult> EntryProduct([FromBody] EntryProductCommand model)
        {
            var response = await _mediator.Send(model);
            return Ok(response);
        }

        [HttpPatch("sale-product")]
        public async Task<ActionResult> SaleProduct([FromBody] SaleProductCommand model)
        {
            await _mediator.Send(model);
            return Ok();
        }
    }
}
