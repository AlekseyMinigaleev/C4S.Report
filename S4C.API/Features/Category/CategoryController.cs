﻿using C4S.Helpers.ApiHeplers.Controllers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using С4S.API.Features.Category.Actions;

namespace С4S.API.Features.Category
{
    public class CategoryController : BaseApiController
    {
        public CategoryController(IMediator mediator) : base(mediator)
        { }

        /// <summary>
        /// Запускает синхронизацию категорий игрк с Яндекса
        /// </summary>
        /// <param name="cancellationToken"></param>
        [HttpGet("Sync")]
        public async Task<ActionResult> SyncCategories(CancellationToken cancellationToken)
        {
            var command = new SyncCategories.Command();

            await Mediator.Send(command,cancellationToken);

            return Ok();
        }
    }
}
