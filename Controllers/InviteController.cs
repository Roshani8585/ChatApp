using Microsoft.AspNetCore.Mvc;

public class InviteController : Controller
{
    private readonly EmailService _emailService;

    public InviteController(EmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost]
    public async Task<IActionResult> SendInvite([FromBody] string email)
    {
        var inviteLink = "https://localhost:5001/chat"; // your app link

        await _emailService.SendInviteEmail(email, inviteLink);

        return Ok("Invitation sent!");
    }
}