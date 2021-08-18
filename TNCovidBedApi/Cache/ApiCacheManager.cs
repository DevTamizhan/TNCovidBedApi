using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using TNCovidBedApi.Models;

namespace TNCovidBedApi.Cache
{
    internal class ApiCacheManager
    {
        private static ApiCacheManager cacheManagerInstance;
        private readonly DistrictCache districtCache;
        private readonly HospitalCache hospitalCache;
        private readonly ILogger logger;
        private string DirectoryPath;
        public static ApiCacheManager GetCacheManagerInstance { get => cacheManagerInstance ?? ApiCacheManager.CreateCacheManager(); }

        readonly JsonSerializerOptions options = new JsonSerializerOptions() { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) };
        public bool WriteToFile { get; private set; }
        private ApiCacheManager()
        {
            logger = ApiLogger.GetLogger();
            districtCache = DistrictCache.CreateDistrictCache();
            hospitalCache = HospitalCache.CreateHospitalCache();
        }

        /// <summary>
        /// Creates the instance of APICacheManager.
        /// <para>
        /// This keeps only one instance of cache in memory and wherever we create cache manager all
        /// cache will be same
        /// </para>
        /// </summary>
        /// <returns>The instance of APCacheManager</returns>
        public static ApiCacheManager CreateCacheManager()
        {
            if (cacheManagerInstance is null)
                cacheManagerInstance = new ApiCacheManager();
            cacheManagerInstance.WriteToFile = true;
            cacheManagerInstance.DirectoryPath = Directory.GetCurrentDirectory();
            return cacheManagerInstance;
        }


        /// <summary>
        /// Creates the instance of APICacheManager.
        /// <para>
        /// This keeps only one instance of cache in memory and wherever we create cache manager all
        /// cache will be same
        /// </para>
        /// </summary>
        /// <param name="createFileCache">Specifies whether fileCache need to be created or not</param>
        /// <param name="directoryPath">Speicfies the directory where the cache need to be stored. Default will be null incase of createFileCache is false or else will be the current directory of the library dll file</param>
        /// <returns>The instance of APCacheManager</returns>
        public static ApiCacheManager CreateCacheManager(bool createFileCache, string directoryPath = null)
        {
            cacheManagerInstance = ApiCacheManager.CreateCacheManager();
            cacheManagerInstance.WriteToFile = createFileCache;
            cacheManagerInstance.DirectoryPath = directoryPath ?? Directory.GetCurrentDirectory();
            return cacheManagerInstance;
        }

        /// <summary>
        /// Gets the cached district data
        /// </summary>
        /// <returns>ReadOnlyCollection of districts from cache</returns>
        public ReadOnlyCollection<District> GetCachedDistricts()
        {
            var allDistricts = districtCache.AllDistricts;
            logger.Info($"Districts data fetched from cache and count of district is {allDistricts.Count}");
            return allDistricts;
        }

        /// <summary>
        /// Gets all the cached hospital details
        /// </summary>
        /// <returns>
        /// ReadOnlyCollection of hospitals that are cached
        /// <para>May or may not have entire hospital details.</para>
        /// /returns>
        public ReadOnlyCollection<Hospital> GetCachedHospitals()
        {
            var allHospitals = hospitalCache.AllHospitals;
            logger.Info($"Hospitals data fetched from cache and count of district is {allHospitals.Count}");
            return allHospitals;
        }

        /// <summary>
        /// Updates the district cache data with new data
        /// </summary>
        /// <param name="districts">List of districts fetched from API</param>
        public void UpdateDistrictCache(List<District> districts)
        {
            logger.Info($"Updating district cache with the new data of count {districts.Count}");
            districtCache.UpdateCache(DateTime.Now, districts);
            if (WriteToFile)
                WriteDistrictsToFile();
        }
        /// <summary>
        /// Adds the list of hospitals that were fetched from API
        /// </summary>
        /// <param name="hospitals">
        /// List of hospitals from API.
        /// <para>List may not may not contain entire hospital details</para>
        /// </param>
        public void UpdateHospitalCache(List<Hospital> hospitals)
        {
            logger.Info($"Updating hospital cache with the new data of count {hospitals.Count}");
            hospitalCache.UpdateCache(DateTime.Now, hospitals);
            if (WriteToFile)
                WriteHospitalsToFile();
        }

        #region FileCache


        /// <summary>
        /// Writes the hospital cache data in memory to file
        /// </summary>
        private void WriteHospitalsToFile()
        {
            string filePath = Path.Combine(DirectoryPath , "hospitalCache.json");
            try
            {
                using (FileStream cacheStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (StreamWriter streamWriter = new StreamWriter(cacheStream))
                    {
                        RootBed bed = new RootBed();
                        bed.Result = new List<Hospital>(hospitalCache.AllHospitals);
                        string fileData = JsonSerializer.Serialize<RootBed>(bed,options);
                        streamWriter.Write(fileData);
                        logger.Info("Hospital cache written");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                DeleteHospitalFileCache();
            }
        }

        /// <summary>
        /// Writes the district cache data from memory to file
        /// </summary>
        private void WriteDistrictsToFile()
        {
            string filePath = Path.Combine(DirectoryPath, "districtCache.json");
            try
            {
                using (FileStream cacheStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                {
                    using (StreamWriter streamWriter = new StreamWriter(cacheStream))
                    {
                        RootDistrict district = new RootDistrict();
                        district.Result = new List<District>(districtCache.AllDistricts);
                        string fileData = JsonSerializer.Serialize<RootDistrict>(district,options);
                        streamWriter.Write(fileData);
                        logger.Info("District cache written");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
        }

        /// <summary>
        /// Deletes the hospital cache file
        /// </summary>
        /// <returns>boolean value denoting file is deleted or not</returns>
        public bool DeleteHospitalFileCache()
        {
            string filePath = Path.Combine(DirectoryPath, "hospitalCache.json");
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                logger.Info("Hospital Cache deeleted");
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return File.Exists(filePath);
            }
        }

        /// <summary>
        /// Deletes the district cache file
        /// </summary>
        /// <returns>boolean value denoting file is deleted or not</returns>
        public bool DeleteDistrictFileCache()
        {
            string filePath = Path.Combine(DirectoryPath, "districtCache.json");
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                logger.Info("District Cache Deleted");
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return File.Exists(filePath);
            }
        }

        /// <summary>
        /// Reads the district cache file and returns the districts
        /// </summary>
        /// <returns>ReadOnlyCollection of district</returns>
        /// <exception cref="FileCacheException">Thrown when file cannot be parsed or any error while reading file</exception>
        public ReadOnlyCollection<District> GetCachedDistrictsFromFile()
        {
            string filePath = Path.Combine(DirectoryPath, "districtCache.json");
            try
            {
                if (!File.Exists(filePath))
                    return new List<District>().AsReadOnly();
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        string data = reader.ReadToEnd();
                        RootDistrict rootDistrict = JsonSerializer.Deserialize<RootDistrict>(data);
                        logger.Info($"Retrived {rootDistrict.Result.Count} district data from file");
                        return rootDistrict.Result.AsReadOnly();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Reading district file cache error\n"+ex.Message);
                throw new FileCacheException("Reading district file cache error", ex);
            }
        }

        /// <summary>
        /// Reads the hospitalCache file and return the hospitals from it
        /// </summary>
        /// <returns>ReadOnlyCollection of hospitals</returns>
        /// <exception cref="FileCacheException">Thrown when file cannot be parsed or any error while reading file</exception>
        public ReadOnlyCollection<Hospital> GetCachedHospitalsFromFile()
        {
            string filePath = Path.Combine(DirectoryPath, "hospitalCache.json");
            try
            {
                if (!File.Exists(filePath))
                    return new List<Hospital>().AsReadOnly();
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        string data = reader.ReadToEnd();
                        RootBed bed = JsonSerializer.Deserialize <RootBed>(data);
                        logger.Info($"Retrieved {bed.Result.Count} hospitals from cache");
                        return bed.Result.AsReadOnly();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Reading hospital file cache error\n" + ex.Message);
                throw new FileCacheException("Reading hospital file cache error", ex);
            }
        }

        #endregion FileCache
    }
}