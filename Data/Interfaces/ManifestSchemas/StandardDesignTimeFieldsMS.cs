﻿using System.Collections.Generic;
using Data.Interfaces.DataTransferObjects;

namespace Data.Interfaces.ManifestSchemas
{
    public class StandardDesignTimeFieldsMS : ManifestSchema
    {
        public StandardDesignTimeFieldsMS()
			  :base(Constants.ManifestTypeEnum.StandardDesignTimeFields)
        {
            Fields = new List<FieldDTO>();
        }

        public List<FieldDTO> Fields { get; set; }
    }
}
