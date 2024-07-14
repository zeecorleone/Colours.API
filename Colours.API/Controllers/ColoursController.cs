using Microsoft.AspNetCore.Mvc;

namespace Colours.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ColoursController : ControllerBase
    {

        private readonly ILogger<ColoursController> _logger;
        private static List<ColourModel> colours = [

                new() { Name = "Red", Code = "#D32F2F"},
                new() { Name = "Green", Code = "#2E7D32"},
                new() { Name = "Blue", Code = "#283593"},
                new() { Name = "Yellow", Code = "#AB47BC"},
                new() { Name = "Voilet", Code = "#AB47BC"},
                new() { Name = "Orange", Code = "#FF5722"},
                new() { Name = "Grey", Code = "#78909C"},


                ];

        public ColoursController(ILogger<ColoursController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetColours")]
        public IEnumerable<ColourModel> Get()
        {
            return colours;
        }


        [HttpPost]
        public async Task<IActionResult> Post(ColourModel model)
        {
            colours.Add(model);
            return CreatedAtAction(nameof(GetByName), new { name = model.Name }, model);
        }

        [HttpGet("GetByName")]
        public async Task<IActionResult> GetByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Name parameter cannot be null or empty");
            var data = colours.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (data is null)
                return NotFound();
            return Ok(data);

        }

    }
}
