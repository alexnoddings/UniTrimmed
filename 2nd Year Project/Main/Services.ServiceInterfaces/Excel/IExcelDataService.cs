using System.Collections.Generic;
using System.IO;

namespace EduLocate.Services.ServiceInterfaces.Excel
{
    /// <summary>Handles working with Excel sheets.</summary>
    public interface IExcelDataService
    {
        /// <summary>Gets a list of schools from a stream.</summary>
        /// <param name="data">The data stream to use.</param>
        /// <returns>A list of schools found in the stream.</returns>
        IEnumerable<Core.School> GetSchoolsFromWorkbook(Stream data);
    }
}