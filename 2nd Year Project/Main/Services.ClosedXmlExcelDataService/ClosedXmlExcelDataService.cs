using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using EduLocate.Core;
using EduLocate.Services.ServiceInterfaces.Coordinates;
using EduLocate.Services.ServiceInterfaces.Excel;

namespace EduLocate.Services.ClosedXmlExcelDataService
{
    /// <inheritdoc />
    /// <summary>Used the ClosedXML package to provide access to excel sheets.</summary>
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification =
        "It is instantiated by the Dependency Injection service at run-time.")]
    public class ClosedXmlExcelDataService : IExcelDataService
    {
        private readonly ICoordinatesService _coordinatesService;

        /// <summary>Constructs the service.</summary>
        /// <param name="coordinatesService">The service used for converting coordinates.</param>
        public ClosedXmlExcelDataService(ICoordinatesService coordinatesService)
        {
            _coordinatesService = coordinatesService;
        }

        /// <inheritdoc />
        public IEnumerable<School> GetSchoolsFromWorkbook(Stream data)
        {
            var workbook = new XLWorkbook(data);
            IXLWorksheets worksheets = workbook.Worksheets;
            return worksheets
                .Select(GetSchoolsFromWorksheet) // Get all schools from the worksheets
                .SelectMany(s => s); // Flattens list
        }

        /// <summary>Gets schools from a worksheets.</summary>
        /// <param name="worksheet">The worksheets to check.</param>
        /// <returns>The schools found.</returns>
        private IEnumerable<School> GetSchoolsFromWorksheet(IXLWorksheet worksheet)
        {
            IXLRows allRows = worksheet.Rows();
            IXLRow firstRow = allRows.First();
            IEnumerable<IXLRow> otherRows = allRows.Skip(1);

            List<string> headers = firstRow.CellsUsed().Select(c => c.GetValue<string>()).ToList();
            // Some excel files have odd, empty worksheets that mess with the dictionary
            if (headers.All(string.IsNullOrWhiteSpace)) return new List<School>();

            int firstColumn = firstRow.CellsUsed().First().Address.ColumnNumber;
            int lastColumn = firstRow.CellsUsed().Last().Address.ColumnNumber;
            // Need to know address of columns to use. Just calling .Cells() will only return those with values
            string columnsToUse = $"{firstColumn}:{lastColumn}";

            var schools = new List<School>();
            foreach (IXLRow row in otherRows)
            {
                var schoolDict = new Dictionary<string, string>();
                List<object> cells = row.Cells(columnsToUse).Select(c => c.Value).ToList();
                // Need to use Math.Min as sometimes in the Excel sheets there can be more headers or cells in the row than the other
                for (var i = 0; i < Math.Min(cells.Count, headers.Count); i++)
                    schoolDict.Add(headers[i], cells[i].ToString());

                School school = ToSchool(schoolDict);
                if (school != null) schools.Add(school);
            }

            return schools;
        }

        /// <summary>Converts a dictionary of key/values from a sheet(s) into a school.</summary>
        /// <param name="data">The data from the sheet(s).</param>
        /// <returns>A school constructed with the valid fields from data.</returns>
        private School ToSchool(IReadOnlyDictionary<string, string> data)
        {
            #region General Data

            // Can't use a school without an ID (URN)
            if (!data.ContainsKey("URN") || string.IsNullOrWhiteSpace(data["URN"]) || data["URN"] == "0")
                return null;
            bool validId = int.TryParse(data["URN"], out int id);
            if (!validId) return null;

            data.TryGetValue("SCHNAME", out string name);
            data.TryGetValue("TELNUM", out string telephone);
            data.TryGetValue("RELDENOM", out string religion);

            data.TryGetValue("ISPRIMARY", out string isPrimary);
            data.TryGetValue("ISSECONDARY", out string isSecondary);
            data.TryGetValue("ISPOST16", out string isPost16);
            EducationStages educationStages = GetEducationStages(isPrimary, isSecondary, isPost16);

            data.TryGetValue("GENDER", out string genderStr);
            SchoolGender gender = GetSchoolGenderFromString(genderStr);

            data.TryGetValue("Overall effectiveness", out string ofstedRatingStr);
            TryParseNullAbleInt(ofstedRatingStr, out int? ofstedRating);

            // Different headers are used between schools and colleges for inspection dates
            data.TryGetValue("Inspection date", out string ofstedInspectionStr);
            if (string.IsNullOrWhiteSpace(ofstedInspectionStr))
                data.TryGetValue("First day of inspection", out ofstedInspectionStr);
            DateTime.TryParse(ofstedInspectionStr, out DateTime lastOfstedInspection);

            // Null values are preferred for links over empty ones
            data.TryGetValue("FacebookLink", out string facebookLink);
            facebookLink = string.IsNullOrWhiteSpace(facebookLink) ? null : facebookLink;
            data.TryGetValue("TwitterLink", out string twitterLink);
            twitterLink = string.IsNullOrWhiteSpace(twitterLink) ? null : twitterLink;

            #endregion

            #region Pupil Stats

            data.TryGetValue("LEARNINGRESOURCES", out string moneySpentPerStudentStr);
            TryParseNullAbleInt(moneySpentPerStudentStr, out int? moneySpentPerStudent);

            data.TryGetValue("NOR", out string numberOfPupilsStr);
            TryParseNullAbleInt(numberOfPupilsStr, out int? numberOfPupils);

            data.TryGetValue("PNORG", out string percentGirlsStr);
            TryParseNullAbleDouble(percentGirlsStr, out double? percentGirls);

            #endregion

            #region Location

            data.TryGetValue("Northing", out string northingStr);
            TryParseNullAbleInt(northingStr, out int? northing);

            data.TryGetValue("Easting", out string eastingStr);
            TryParseNullAbleInt(eastingStr, out int? easting);

            double? latitude = null;
            double? longitude = null;
            if ((northing ?? easting) != null)
                (latitude, longitude) =
                    _coordinatesService.GridToLatitudeLongitude((double) northing, (double) easting);

            data.TryGetValue("STREET", out string street);
            data.TryGetValue("LOCALITY", out string locality);
            string address;
            if (!string.IsNullOrWhiteSpace(street))
                address = !string.IsNullOrWhiteSpace(locality) ? $"{street}, {locality}" : street;
            else if (!string.IsNullOrWhiteSpace(locality))
                address = locality;
            else
                address = "Not specified";

            data.TryGetValue("TOWN", out string town);
            data.TryGetValue("POSTCODE", out string postcode);

            #endregion

            return new School
            {
                Id = id,
                IsOpen = true,
                Name = name,
                Telephone = telephone,
                EducationStages = educationStages,
                Gender = gender,
                Religion = religion,
                OfstedRating = ofstedRating,
                LastOfstedInspection = lastOfstedInspection,
                FacebookLink = facebookLink,
                TwitterLink = twitterLink,

                MoneySpentPerStudent = moneySpentPerStudent,
                NumberOfPupils = numberOfPupils,
                PercentGirls = percentGirls,

                Latitude = latitude,
                Longitude = longitude,
                Address = address,
                Town = town,
                Postcode = postcode
            };
        }

        /// <summary>Tries to parse a string to a null-able double.</summary>
        /// <param name="s">The string to parse.</param>
        /// <param name="i">The int from the string, or null if the string was not a valid int.</param>
        private static void TryParseNullAbleInt(string s, out int? i)
        {
            i = int.TryParse(s, out int value) ? (int?) value : null;
        }

        /// <summary>Tries to parse a string to a null-able double.</summary>
        /// <param name="s">The string to parse.</param>
        /// <param name="d">The double from the string, or null if the string was not a valid double.</param>
        private static void TryParseNullAbleDouble(string s, out double? d)
        {
            d = double.TryParse(s, out double value) ? (double?) value : null;
        }

        /// <summary>Converts the education stage strings from a sheet into a <see cref="EducationStages"/>.</summary>
        /// <param name="isPrimary">The string representation of whether primary education is offered.</param>
        /// <param name="isSecondary">The string representation of whether secondary education is offered.</param>
        /// <param name="isPost16">The string representation of whether post-16 (college) education is offered.</param>
        /// <returns>The education stages as an enum.</returns>
        private static EducationStages GetEducationStages(string isPrimary, string isSecondary, string isPost16)
        {
            EducationStages stages = 0;
            isPrimary = isPrimary?.ToLower().Trim();
            isSecondary = isSecondary?.ToLower().Trim();
            isPost16 = isPost16?.ToLower().Trim();

            if (!string.IsNullOrWhiteSpace(isPrimary) &&
                (isPrimary == "1" || isPrimary.Contains("true") || isPrimary.Contains("yes")))
                stages |= EducationStages.Primary;
            if (!string.IsNullOrWhiteSpace(isSecondary) &&
                (isSecondary == "1" || isSecondary.Contains("true") || isSecondary.Contains("yes")))
                stages |= EducationStages.Secondary;
            if (!string.IsNullOrWhiteSpace(isPost16) &&
                (isPost16 == "1" || isPost16.Contains("true") || isPost16.Contains("yes")))
                stages |= EducationStages.College;

            return stages;
        }

        /// <summary>Converts a gender string from a sheet into a <see cref="SchoolGender"/>.</summary>
        /// <param name="genderString">The gender from the sheet.</param>
        /// <returns>The gender as an enum.</returns>
        private static SchoolGender GetSchoolGenderFromString(string genderString)
        {
            SchoolGender gender;
            switch (genderString.ToLower())
            {
                case "mixed":
                case "not applicable":
                    gender = SchoolGender.Mixed;
                    break;
                case "male":
                case "boys":
                    gender = SchoolGender.BoysOnly;
                    break;
                case "female":
                case "girls":
                    gender = SchoolGender.GirlsOnly;
                    break;
                default:
                    gender = SchoolGender.None;
                    break;
            }

            return gender;
        }
    }
}