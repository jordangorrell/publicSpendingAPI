using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spendingAPI.Data;
using spendingAPI.DTOs;
using spendingAPI.Exceptions;
using spendingAPI.Models;
using spendingAPI.Services;

namespace spendingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EntriesController : ControllerBase
    {
        private readonly IEntryService _entryService;
        private readonly IJWTService _jwtService;

        public EntriesController(IEntryService entryService, IJWTService jwtService)
        {
            _entryService = entryService;
            _jwtService = jwtService;
        }

        // GET: api/Entries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Entry>>> GetEntries([FromQuery] SearchOptions options, [FromHeader] string authorization)
        {
            var userId = _jwtService.ExtractUserIdFromToken(authorization);
            var entries = await _entryService.GetEntries(options, userId);
            return Ok(entries);
        }

        [HttpPost]
        public async Task<ActionResult<Entry>> AddEntry([FromBody] Entry entry, [FromHeader] string authorization)
        {
            var userId = _jwtService.ExtractUserIdFromToken(authorization);
            var createdEntry = await _entryService.AddEntry(entry, userId);
            return Ok(createdEntry);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<Entry>> UpdateEntry(int id, [FromBody] EntryForUpdateDto entryForUpdate, [FromHeader] string authorization)
        {
            var userId = _jwtService.ExtractUserIdFromToken(authorization);
            var updatedEntry = await _entryService.UpdateEntry(id, entryForUpdate, userId);
            return Ok(updatedEntry);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Entry>> DeleteEntry(int id, [FromHeader] string authorization)
        {
            var userId = _jwtService.ExtractUserIdFromToken(authorization);
            var entry = await _entryService.DeleteEntry(id, userId);
            return Ok(entry);
        }

    }
}
