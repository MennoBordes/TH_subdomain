﻿using AutoMapper;

namespace TH.Core
{
    public class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Mapper.Initialize(x =>
            {
                x.AddProfile<FormSchemaMappingProfile>();
            });
        }
    }
}
