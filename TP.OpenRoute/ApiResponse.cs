﻿using System.Net;

namespace TP.OpenRoute;

public record ApiResponse<T>(bool IsSuccess, HttpStatusCode? ResponseCode = default, T? Response = default, ErrorResponse? ErrorResponse = null);