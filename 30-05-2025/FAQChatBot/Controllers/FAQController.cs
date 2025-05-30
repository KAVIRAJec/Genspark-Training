using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FAQChatBot.Models;
using FAQChatBot.Models.DTOs;
using FAQChatBot.Services;
using FAQChatBot.Repositories;
using FAQChatBot.Interfaces;

namespace FAQChatBot.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FAQController : ControllerBase
    {
        private readonly IFAQService _faqService;

        public FAQController(IFAQService faqService)
        {
            _faqService = faqService;
        }

        [HttpPost("Ask")]
        public async Task<IActionResult> AskQuestion([FromBody] FAQRequestDTO request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Question))
            {
                return BadRequest("Invalid request.");
            }

            var response = await _faqService.GetAnswer(request);
            return Ok(response);
        }

        [HttpGet("AllChatLogs")]
        public async Task<IActionResult> GetChatLogs()
        {
            var logs = await _faqService.GetAllChatLogs();
            return Ok(logs);
        }
    }
}