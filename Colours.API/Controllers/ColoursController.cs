using Colours.API.Data;
using Colours.API.Models;
using Colours.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Xml.Linq;

namespace Colours.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ColoursController : ControllerBase
    {

        private readonly ILogger<ColoursController> _logger;
        private readonly ICacheService _cacheService;
        private readonly AppDbContext _appDbContext;
 
        
        public ColoursController(ILogger<ColoursController> logger, ICacheService cacheService, AppDbContext appDbContext)
        {
            _logger = logger;
            _cacheService = cacheService;
            _appDbContext = appDbContext;
        }

        [HttpGet(Name = "Colours")]
        public async Task<IActionResult> Get()
        {
            var cacheData = _cacheService.GetData<IEnumerable<Colour>>("colours");
            if (cacheData?.Count() > 0)
            {
                var coloursList = GetColoursModel(cacheData);
                return Ok(cacheData);
            }

            var colours = await _appDbContext.Colours.ToListAsync();
            var expiryTime = DateTimeOffset.Now.AddSeconds(60);
            _cacheService.SetData("colours", colours, expiryTime);

            return Ok(GetColoursModel(colours));
        }

        [HttpPost]
        public async Task<IActionResult> Post(ColourModel model)
        {
            var newColour = new Models.Colour()
            {
                Name = model.Name,
                Code = model.Code,
                Description = model.Description,
            };

            var addedObj = await _appDbContext.Colours.AddAsync(newColour);
            await _appDbContext.SaveChangesAsync();

            var expireyTime = DateTimeOffset.Now.AddSeconds(60);
            _cacheService.SetData<Colour>($"colour{newColour.Id}", newColour, expireyTime);

            return CreatedAtAction(nameof(Get), new { id = newColour.Id }, model);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {

            var cacheData = _cacheService.GetData<Colour>($"colour{id}");
            if (cacheData is not null)
                return Ok(GetColourModel(cacheData));

            var colour = _appDbContext.Colours.FirstOrDefault(c => c.Id == id);
            if (colour is null)
                return NotFound($"Colour with Id {id} does not exist");

            var expireyTime = DateTimeOffset.Now.AddSeconds(60);
            _cacheService.SetData<Colour>($"colour{colour.Id}", colour, expireyTime);

            return Ok(GetColourModel(colour));
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var exist = await _appDbContext.Colours.FirstOrDefaultAsync(c => c.Id == id);
            if (exist is null)
                return NotFound($"Colour with Id {id} is not found");

            _appDbContext.Remove(exist);
            _cacheService.RemoveData($"colour{id}");

            await _appDbContext.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("logs")]
        public async Task<IActionResult> Logs()
        {
            var logFilePath = Path.Combine("/app/logs", "app.log");
            var allLogs = await System.IO.File.ReadAllTextAsync(logFilePath);
            return Ok(allLogs);
        }



        private List<ColourModel> GetColoursModel(IEnumerable<Colour> colours)
        {
            var colorsModelList = new List<ColourModel>();
            foreach (var colour in colours)
                colorsModelList.Add(new()
                {
                    Id = colour.Id,
                    Name = colour.Name,
                    Code = colour.Code,
                    Description = colour.Description,
                });

            return colorsModelList;
        }
        private ColourModel GetColourModel(Colour colour)
            => 
                new ColourModel()
                {
                    Id = colour.Id,
                    Name = colour.Name,
                    Code = colour.Code,
                    Description = colour.Description,
                };
        


    }
}
