namespace BibliotecaApi.Dtos;

public class ResponseApi<T>
{
    public string Status {get; set;} = string.Empty;

    public T? Data {get; set;}


    public static ResponseApi<T> Success(T data) => new() {
        Status = "SUCCESS",
        Data = data
    };
}