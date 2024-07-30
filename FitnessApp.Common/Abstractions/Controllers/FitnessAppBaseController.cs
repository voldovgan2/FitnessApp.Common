using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessApp.Common.Abstractions.Controllers;

[ExcludeFromCodeCoverage]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public abstract class FitnessAppBaseController : Controller;