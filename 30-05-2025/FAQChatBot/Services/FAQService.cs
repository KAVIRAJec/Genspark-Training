using FAQChatBot.Models;
using FAQChatBot.Models.DTOs;
using FAQChatBot.Repositories;
using FAQChatBot.Interfaces;
using FAQChatBot.Misc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace FAQChatBot.Services
{
    public class FAQService : IFAQService
    {
        private readonly IRepository<int, ChatLog> _chatLogRepository;
        private readonly IRepository<int, FAQ> _faqRepository;
        private readonly IRepository<int, User> _userRepository;
        private readonly ChatlogResponseDataMapping _chatlogResponseDataMapping;

        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public FAQService(IRepository<int, ChatLog> chatLogRepository,
                          IRepository<int, FAQ> faqRepository,
                          IRepository<int, User> userRepository,
                          HttpClient httpClient,
                          IConfiguration configuration)
        {
            _chatLogRepository = chatLogRepository;
            _faqRepository = faqRepository;
            _userRepository = userRepository;
            _chatlogResponseDataMapping = new();
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<FAQResponseDTO> GetAnswer(FAQRequestDTO request)
        {
            try
            {
                if(string.IsNullOrEmpty(request.Question)) throw new ArgumentException("Question cannot be null or empty.");

                var user = await _userRepository.Get(request.UserId);
                if (user == null)
                    throw new Exception($"User with ID {request.UserId} does not exist.");
                
                var answer = await GetAnswerAsync(request.Question);
                var faqResponse = new FAQResponseDTO
                {
                    Question = request.Question,
                    Answer = answer,
                    UserId = request.UserId
                };
                var chatLog = new ChatLog
                {
                    Question = request.Question,
                    Answer = answer,
                    UserId = request.UserId,
                    CreatedAt = DateTime.UtcNow
                };
                await _chatLogRepository.Create(chatLog);
                return faqResponse;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<ChatLogResponseDTO>> GetAllChatLogs()
        {
            try{
                var logs = await _chatLogRepository.GetAll();
                if (logs == null || logs.Count() == 0)
                {
                    throw new KeyNotFoundException("No chat logs found.");
                }
                var response = new List<ChatLogResponseDTO>();
                foreach (var log in logs)
                {
                    var chatLogResponse = _chatlogResponseDataMapping.ToChatLogResponseDTO(log);
                    response.Add(chatLogResponse);
                }
                return response;
            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<string> GetAnswerAsync(string userQuestion)
        {
            var apiKey = _configuration["HuggingFace:ApiKey"];
            var modelUrl = "https://api-inference.huggingface.co/models/distilbert-base-uncased-distilled-squad";

            var payload = new{inputs = userQuestion};

            var request = new HttpRequestMessage(HttpMethod.Post, modelUrl);
            request.Headers.Add("Authorization", $"Bearer {apiKey}");
            request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error: {response.StatusCode}");
                return "Sorry, I couldn't get an answer right now.";
            }
            var result = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(result);
            var generatedText = jsonDoc.RootElement[0].GetProperty("generated_text").GetString();

            return generatedText;
        }
    }
}