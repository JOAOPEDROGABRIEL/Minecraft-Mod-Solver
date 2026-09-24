using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Minecraft_Mod_Solver.Services;

public class ModVerifierService
{
    private readonly HttpClient _httpClient;
    private readonly string _curseForgeApiKey;

    public ModVerifierService(string curseForgeApiKey)
    {
        _curseForgeApiKey = curseForgeApiKey;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "ModVerifier-CSharp/1.0");
    }

    // 1. Geração de Hashes
    public string GetSha1(string filePath)
    {
        using var sha1 = SHA1.Create();
        using var stream = File.OpenRead(filePath);
        var hashBytes = sha1.ComputeHash(stream);
        return Convert.ToHexString(hashBytes).ToLower();
    }

    public uint GetCurseForgeMurmur2(string filePath)
    {
        byte[] data = File.ReadAllBytes(filePath);
        var filteredList = new List<byte>(data.Length);
        
        // Remove espaços, tabs e quebras de linha conforme exigência da API
        foreach (var b in data)
        {
            if (b != 9 && b != 10 && b != 13 && b != 32)
                filteredList.Add(b);
        }

        byte[] filteredData = filteredList.ToArray();
        int length = filteredData.Length;
        uint m = 0x5bd1e995;
        int r = 24;
        uint seed = 1;
        uint h = seed ^ (uint)length;
        int i = 0;

        while (length >= 4)
        {
            uint k = BitConverter.ToUInt32(filteredData, i);
            k *= m;
            k ^= k >> r;
            k *= m;
            h *= m;
            h ^= k;
            i += 4;
            length -= 4;
        }

        if (length == 3) h ^= (uint)(filteredData[i + 2] << 16);
        if (length >= 2) h ^= (uint)(filteredData[i + 1] << 8);
        if (length >= 1)
        {
            h ^= filteredData[i];
            h *= m;
        }

        h ^= h >> 13;
        h *= m;
        h ^= h >> 15;

        return h;
    }

    // 2. Consultas nas APIs
    public async Task<string?> CheckModrinthAsync(string filePath)
    {
        try
        {
            string sha1 = GetSha1(filePath);
            string url = $"https://api.modrinth.com/v2/version_file/{sha1}?algorithm=sha1";
            
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;

            string jsonResponse = await response.Content.ReadAsStringAsync();
            var node = JsonNode.Parse(jsonResponse);
            string? projectId = node?["project_id"]?.ToString();

            if (projectId == null) return null;

            string projectUrl = $"https://api.modrinth.com/v2/project/{projectId}";
            var projectResponse = await _httpClient.GetAsync(projectUrl);
            if (!projectResponse.IsSuccessStatusCode) return null;

            string projectJson = await projectResponse.Content.ReadAsStringAsync();
            var projectNode = JsonNode.Parse(projectJson);
            
            return projectNode?["server_side"]?.ToString();
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> TestCurseForgeConnectionAsync()
    {
        if (string.IsNullOrWhiteSpace(_curseForgeApiKey)) return false;
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.curseforge.com/v1/games/432");
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("x-api-key", _curseForgeApiKey);
            
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<string?> CheckCurseForgeAsync(string filePath)
    {
        if (string.IsNullOrWhiteSpace(_curseForgeApiKey)) return null;

        try
        {
            uint murmur = GetCurseForgeMurmur2(filePath);
            string url = "https://api.curseforge.com/v1/fingerprints";

            var payload = new { fingerprints = new[] { murmur } };
            string jsonPayload = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };
            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("x-api-key", _curseForgeApiKey);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            string jsonResponse = await response.Content.ReadAsStringAsync();
            var node = JsonNode.Parse(jsonResponse);
            var exactMatches = node?["data"]?["exactMatches"]?.AsArray();

            if (exactMatches != null && exactMatches.Count > 0)
            {
                return "required"; // Padrão adotado no script Python
            }
            return null;
        }
        catch
        {
            return null;
        }
    }
}