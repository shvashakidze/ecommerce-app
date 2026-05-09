[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly AuthService _authService;

    public AuthController(AppDbContext db, AuthService authService)
    { _db = db; _authService = authService; }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
            return BadRequest("Email უკვე გამოყენებულია");

        var user = new User
        {
            Email = dto.Email,
            PasswordHash = _authService.HashPassword(dto.Password),
            FirstName = dto.FirstName,
            LastName = dto.LastName
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var token = _authService.GenerateJwtToken(user);
        return Ok(new AuthResponseDto(token, user.Email,
            $"{user.FirstName} {user.LastName}", user.Role.ToString()));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null || user.PasswordHash != _authService.HashPassword(dto.Password))
            return Unauthorized("Email ან პაროლი არასწორია");

        var token = _authService.GenerateJwtToken(user);
        return Ok(new AuthResponseDto(token, user.Email,
            $"{user.FirstName} {user.LastName}", user.Role.ToString()));
    }
}
