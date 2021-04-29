using CQRSMeetup.ReadApi.Features.Stock.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CQRSMeetup.ReadApi.Controllers
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

        [HttpGet()]
        [ProducesResponseType(200)]
        public async Task<ActionResult> GetAll()
        {
            var response = await _mediator.Send(new GetStocksQuery());
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(long id)
        {
            var response = await _mediator.Send(new GetStockQuery(id));
            return Ok(response);
        }
    }
}
