using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TaskManager.WPF.Models;
using System.Net.Http.Json;

namespace TaskManager.WPF.Services
{
    public class TaskService
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "http://localhost:5048/api/tasks"; // URL da sua API

        public TaskService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<List<TaskItem>> GetTasksAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(ApiUrl);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<TaskItem>>() ?? new List<TaskItem>();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Erro ao buscar tarefas: {ex.Message}", ex);
            }
        }

        public async Task<TaskItem> AddTaskAsync(TaskItem task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task), "A tarefa não pode ser nula.");
            }

            try
            {
                var response = await _httpClient.PostAsJsonAsync(ApiUrl, task);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<TaskItem>() ?? task;
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Erro ao adicionar tarefa: {ex.Message}", ex);
            }
        }

        public async Task<TaskItem> UpdateTaskAsync(TaskItem task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task), "A tarefa não pode ser nula.");
            }

            try
            {
                var jsonContent = JsonSerializer.Serialize(task);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"{ApiUrl}/{task.Id}", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Erro na atualização: {response.StatusCode} - {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseContent))
                {
                    return task; // Se a resposta estiver vazia, retornamos a tarefa original
                }

                try
                {
                    return JsonSerializer.Deserialize<TaskItem>(responseContent) ?? task;
                }
                catch (JsonException ex)
                {
                    throw new Exception($"Erro ao desserializar resposta: {ex.Message}. Resposta: {responseContent}", ex);
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Erro ao atualizar tarefa: {ex.Message}", ex);
            }
        }

        public async Task DeleteTaskAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"{ApiUrl}/{id}");
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"Erro ao excluir tarefa: {ex.Message}", ex);
            }
        }
    }
}
