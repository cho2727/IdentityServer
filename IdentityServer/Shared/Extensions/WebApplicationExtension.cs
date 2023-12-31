﻿using IdentityServer.Shared.Interface;

namespace IdentityServer.Shared.Extensions;

public static class WebApplicationExtension
{
    public static void MapEndPoints(this WebApplication self)
    {
        var endpoints = self.Services.GetServices<IEndPoints>();
        foreach(var endpoint in endpoints)
        {
            endpoint.AddEndPoints(self);
        }

    }
}
