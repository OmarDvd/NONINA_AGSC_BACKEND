using NONINA_AGSC_APP_BACKEND.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace NONINA_AGSC_APP_BACKEND_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MunicipalitiesController : Controller
    {
        private readonly MunicipalityAPIDbContext dbContext;
        public MunicipalitiesController(MunicipalityAPIDbContext dbContext)
        {
            this.dbContext = dbContext;

        }


        [HttpGet]
        public async Task<IActionResult> GetMunicipalities()
        {
            return Ok(await dbContext.Municipalities.ToListAsync());
        }

        [HttpGet]
        [Route("{id:long}")]
        public async Task<IActionResult> GetMunicipalities([FromRoute] long id)
        {
            var municipality = await dbContext.Municipalities.FindAsync(id);
            if (municipality == null)
            {
                return NotFound();
            }
            return Ok(municipality);
        }

        [HttpPost]
        public async Task<IActionResult> AddMunicipality(AddMunicipalityRequest addMunicipalityRequest)
        {
            var municipality = new Municipality()
            {

                Name = addMunicipalityRequest.Name,
  
            };

            await dbContext.Municipalities.AddAsync(municipality);
            await dbContext.SaveChangesAsync();

            return Ok(municipality);
        }



        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> UpdateMunicipality([FromRoute] long id, UpdateMunicipalityRequest updateMunicipalityRequest)
        {
            var municipality = await dbContext.Municipalities.FindAsync(id);
            if (municipality != null)
            {

                municipality.Name = updateMunicipalityRequest.Name;
 

                await dbContext.SaveChangesAsync();
                return Ok(municipality);

            }

            return NotFound();

        }

        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> DeleteMunicipality([FromRoute] long id)
        {
            var municipality = await dbContext.Municipalities.FindAsync(id);
            if (municipality == null)
            {
                return NotFound();
            }
            dbContext.Remove(municipality);
            await dbContext.SaveChangesAsync();
            return Ok(municipality);
        }

    }


}
