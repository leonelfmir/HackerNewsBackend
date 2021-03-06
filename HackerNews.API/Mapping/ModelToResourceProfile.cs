﻿using AutoMapper;
using HackerNews.API.Resources;
using HackerNews.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerNews.API.Mapping
{
    public class ModelToResourceProfile: Profile
    {
        public ModelToResourceProfile()
        {
            CreateMap<New, NewResource>()
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.By))
                .ForMember(dest => dest.Time, opt => opt.MapFrom(src => DateTimeOffset.FromUnixTimeSeconds(src.Time).UtcDateTime));
        }
    }
}
