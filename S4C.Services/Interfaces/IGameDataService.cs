﻿using C4S.DB.Models;

namespace C4S.Services.Interfaces
{
    public interface IGameDataService
    {
        public Task GetAllGameDataAsync();
    }
}