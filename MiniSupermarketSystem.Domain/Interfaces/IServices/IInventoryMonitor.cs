﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniSupermarketSystem.Domain.Interfaces.IServices
{
    public interface IInventoryMonitor
    {
        Task CheckLowInventoryAsync(int threshold);
    }
}
