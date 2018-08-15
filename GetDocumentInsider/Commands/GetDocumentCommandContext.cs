using System;

namespace GetDocument.Commands
{
    [Serializable]
    public class GetDocumentCommandContext
    {
        public string AssemblyDirectory { get; set; }

        public string AssemblyName { get; set; }

        public string AssemblyPath { get; set; }
    }
}
