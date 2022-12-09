using ColorPaletteGen.Bot.Services;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace ColorPaletteGen.Bot.Controllers;

[ApiController]
[Route("[controller]")]
public class UpdateController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> HandleUpdate(
        [FromBody] Update update,
        [FromServices] UpdateHandlerService handlerService,
        CancellationToken cancellationToken)
    {
        await handlerService.HandleUpdateAsync(update, cancellationToken);
        return Ok();
    }
}
