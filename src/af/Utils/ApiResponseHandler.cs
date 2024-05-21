using AwesomeFiles.HttpModels.Responses;
using Newtonsoft.Json;

namespace af.Utils;

public static class ApiResponseHandler
{
    public static void PrintListResult(HttpResponseMessage response, string content)
    {
        if (response.IsSuccessStatusCode)
        {
            var result = JsonConvert.DeserializeObject<ApiResponse<List<FileInfoResponse>>>(content)!;

            if (result.Success)
            {
                foreach (var fileName in result.Body!)
                    Console.Write(fileName.Name + " ");
                Console.WriteLine();
                return;
            }

            Console.WriteLine("Errors: ");
            foreach (var error in result.Errors!)
                Console.Write(error + " ");

            Console.WriteLine();
            return;
        }

        Console.WriteLine(content);
    }
    
    public static void PrintCreateArchiveResult(HttpResponseMessage response, string content)
    {
        var result = JsonConvert.DeserializeObject<ApiResponse<ProcessIdResponse>>(content)!;

        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("Создана задача на архивацию, id: " + result.Body!.Id);
            return;
        }

        Console.WriteLine(content);
    }
    
    public static void PrintStatusArchivingResult(HttpResponseMessage response, string content)
    {
        if (response.IsSuccessStatusCode)
        {
            var result = JsonConvert.DeserializeObject<ApiResponse<ArchivingStatus>>(content)!;
            Console.WriteLine("Задача на архивацию находится в статусе: " + result.Body!.Status);
            return;
        }

        Console.WriteLine(content);
    }
}