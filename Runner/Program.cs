﻿using System;
using BenchmarkDotNet.Running;

namespace Runner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
            Console.Read();
        }
    }
}
