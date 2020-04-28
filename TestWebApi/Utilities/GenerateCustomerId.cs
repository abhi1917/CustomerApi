﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestWebApi.Utilities
{
    public static class GenerateCustomerId
    {
        private static Random random = new Random();
        public static string Generate(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}