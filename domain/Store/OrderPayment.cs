﻿using System;
using System.Collections.Generic;

namespace Store
{
    public class OrderPayment
    {
        public string UniqueCode { get; }

        public string Description { get; }

        public IReadOnlyDictionary<string, string> Parameters { get; }

        public OrderPayment(string uniqueCode,
                             string description,
                             IReadOnlyDictionary<string, string> parameters)
        {
            if (string.IsNullOrWhiteSpace(uniqueCode))
                throw new ArgumentException(nameof(uniqueCode));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException(nameof(description));

            UniqueCode = uniqueCode;
            Description = description;
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            //TODO Write validation tests
        }

    }
}
