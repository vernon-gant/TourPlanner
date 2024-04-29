using System.Net;

namespace TP.Utils;

public record ApiResponse<T>(bool IsSuccess, HttpStatusCode? ResponseCode = default, T? Response = default);