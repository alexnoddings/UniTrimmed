using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EduLocate.Core;
using EduLocate.Services.ServiceInterfaces.Coordinates;
using EduLocate.Services.ServiceInterfaces.School;

namespace EduLocate.Server.Data
{
    /// <inheritdoc />
    /// <summary>Provides a school repository from the Entity Framework DbContext.</summary>
    public class EfSchoolRepository : ISchoolRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ICoordinatesService _coordinatesService;

        public EfSchoolRepository(ApplicationDbContext dbContext, ICoordinatesService coordinatesService)
        {
            _dbContext = dbContext;
            _coordinatesService = coordinatesService;
        }

        /// <inheritdoc />
        public async Task<School> GetSchoolAsync(int id)
        {
            School schoolEntity = await _dbContext.Schools.FindAsync(id);
            // FindAsync can't be used on AsNoTracking DbSets, so instead find it as tracking
            // and set the entry state to detached so it's updates won't be tracked
            if (schoolEntity != null) _dbContext.Entry(schoolEntity).State = EntityState.Detached;
            return schoolEntity;
        }

        /// <inheritdoc />
        public Task<IEnumerable<School>> GetAllSchoolsAsync()
        {
            return Task.FromResult<IEnumerable<School>>(_dbContext.Schools.AsNoTracking());
        }

        /// <inheritdoc />
        public Task<IEnumerable<School>> GetSchoolsInRadiusAsync(double latitude, double longitude, double radiusKm)
        {
            return Task.FromResult<IEnumerable<School>>(
                _dbContext.Schools.AsNoTracking().Where(s =>
                    _coordinatesService.DistanceBetween(s, latitude, longitude) < radiusKm));
        }

        /// <inheritdoc />
        public async Task SelectiveUpdateSchoolAsync(School school)
        {
            await SelectiveUpdateSchoolsAsync(new[] {school});
        }

        /// <inheritdoc />
        public async Task SelectiveUpdateSchoolsAsync(IEnumerable<School> schools)
        {
            const int maxBatchSize = 100;
            var batchSize = 0;
            foreach (School school in schools)
            {
                batchSize++;
                School existingSchool = await GetSchoolTrackedAsync(school.Id);

                if (existingSchool == null)
                {
                    await _dbContext.Schools.AddAsync(school);
                }
                else
                {
                    PropertyInfo[] properties = typeof(School).GetProperties();
                    foreach (PropertyInfo prop in properties)
                    {
                        object newValue = prop.GetValue(school);

                        // Don't set for null values
                        if (newValue == null)
                            continue;
                        // No setter exists for property
                        if (prop.SetMethod == null) continue;
                        switch (newValue)
                        {
                            case int i:
                                prop.SetValue(existingSchool, i);
                                break;
                            case string s:
                                if (!string.IsNullOrWhiteSpace(s)) prop.SetValue(existingSchool, s);
                                break;
                            case EducationStages e:
                                if (e != EducationStages.None) prop.SetValue(existingSchool, e);
                                break;
                            case SchoolGender g:
                                if (g != SchoolGender.None) prop.SetValue(existingSchool, g);
                                break;
                            case double d:
                                prop.SetValue(existingSchool, d);
                                break;
                            case DateTime dt:
                                if (dt != default(DateTime)) prop.SetValue(existingSchool, dt);
                                break;
                        }
                    }
                }

                if (batchSize < maxBatchSize) continue;

                batchSize = 0;
                await _dbContext.SaveChangesAsync();
            }

            await _dbContext.SaveChangesAsync();
        }

        private async Task<School> GetSchoolTrackedAsync(int id)
        {
            return await _dbContext.Schools.FindAsync(id);
        }
    }
}