﻿namespace Fap.AspNetCore.Controls.JqGrid.DataReaders
{
    public class JsonReader
    {
        public JsonReader()
        {
            RepeatItems = false;
        }

        public bool RepeatItems { get; set; }
        public string Id { get; set; }
    }
}
