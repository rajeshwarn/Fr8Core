﻿using System.Collections.Generic;

namespace pluginAzureSqlServer.Infrastructure
{
    public class Row
    {
        public Row(IEnumerable<FieldValue> values)
        {
            _values = values;
        }

        public IEnumerable<FieldValue> Values { get { return _values; } }

        
        private readonly IEnumerable<FieldValue> _values;
    }
}
