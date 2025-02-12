using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using StorkDorkMain.DAL.Abstract;
using StorkDorkMain.DAL.Concrete;
using StorkDorkMain.Data;
using StorkDorkMain.Models;

namespace StorkDork.Controllers
{
    [Route("api/bird")]
    [ApiController]
    public class BirdApiController : ControllerBase
    {
        private readonly IBirdRepository _birdRepo;

        public BirdApiController(IBirdRepository birdRepo)
        {
            _birdRepo = birdRepo;
        }

        // GET: api/bird
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Bird>>> GetBirds()
        {
            return await _birdRepo.GetAll().ToListAsync();
        }

        // GET: api/bird/{id}
        [HttpGet("{id}")]
        public Task<ActionResult<Bird>> GetBird(int id)
        {
            var bird = _birdRepo.FindById(id);

            if (bird == null)
            {
                return Task.FromResult<ActionResult<Bird>>(NotFound());
            }

            return Task.FromResult<ActionResult<Bird>>(bird);
        }

        // GET: api/bird/search/{name}
        [HttpGet("search/{name}")]
        public async Task<ActionResult<IEnumerable<Bird>>> GetBirdsByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("Search term cannot be empty");
            }

            var birds = await _birdRepo.GetBirdsByName(name);
            if (!birds.Any())
            {
                return NotFound();
            }

            return Ok(birds);
        }
    }
}
