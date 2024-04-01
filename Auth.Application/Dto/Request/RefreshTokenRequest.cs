using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auth.Application.Dto.Request
{
    /// <summary>
    /// Represents a request to refresh an access token.
    /// </summary>
    public record RefreshTokenRequest(string RefreshToken)
    {
    }
}
