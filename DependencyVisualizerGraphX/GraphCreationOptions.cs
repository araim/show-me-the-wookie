using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DependencyVisualizerGraphX
{
    [Flags]
    internal enum GraphCreationOptions
    {
        None,
        IncludeSolutionsReferencingSelectedProjects
    }
}
