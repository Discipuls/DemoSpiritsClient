﻿using AutoMapper;
using SpiritsClassLibrary.DTOs.SpiritDTOs;
using SpiritsFirstTry.Models;
using SpiritsFirstTry.Services.Interfaces;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace SpiritsFirstTry.Services
{
    public class SpiritService : ISpiritService
    {
        private IMapper _mapper;
        private IRestService _restService;

        string dataDirectory;

        public SpiritService(IMapper mapper, IRestService restService) { 
            _mapper = mapper;
            _restService = restService;
        }

        public async Task<List<MapSpirit>> LoadSpirits(ProgressBar progressBar, List<MapHabitat> habitats)
        {
            dataDirectory = FileSystem.AppDataDirectory;
            bool APISpiritsAvailible = true;
            bool JSONSpiritsAvailible = true;

            List<GetSpiritBasicsDTO> APISpirits = new List<GetSpiritBasicsDTO>();
            try
            {
                APISpirits = await _restService.GetAllSpiritsAsync();
            }
            catch (Exception ex)
            {
                APISpiritsAvailible = false;
                Debug.WriteLine(ex.Message);

            }
            List<GetSpiritBasicsDTO> JSONSpirits = new List<GetSpiritBasicsDTO>();
            try
            {
                JSONSpirits = await LoadJSONSpirits();
            }
            catch (Exception ex)
            {
                JSONSpiritsAvailible = false;
                Debug.WriteLine(ex.Message);
                Application.Current.MainPage.DisplayAlert("Exception", ex.Message, "Ok");


            }
            List<GetSpiritBasicsDTO> resultSpiritsBasicsDTOs = new List<GetSpiritBasicsDTO>();

            if (APISpiritsAvailible && JSONSpiritsAvailible)
            {
                foreach (var apiSpirit in APISpirits)
                {
                    var jsonSpirit = JSONSpirits.Where(s => s.Id == apiSpirit.Id).FirstOrDefault();
                    if (jsonSpirit != null)
                    {
                        if (jsonSpirit.LastUpdated < apiSpirit.LastUpdated)
                        {
                            var newSpirit = await UpdateMissedSpirit(jsonSpirit);
                        }
                    }
                    else
                    {
                        var newSpirit = await UpdateMissedSpirit(apiSpirit);
                    }

                    resultSpiritsBasicsDTOs.Add(apiSpirit);

                    await progressBar.ProgressTo(progressBar.Progress + (0.75 / APISpirits.Count), 5, Easing.Linear);
                }

                SaveSpiritsToJson(resultSpiritsBasicsDTOs);
            }
            else if (JSONSpiritsAvailible)
            {
                resultSpiritsBasicsDTOs = JSONSpirits;
            }
            else if (APISpiritsAvailible)
            {
                foreach (var s in APISpirits)
                {
                    try
                    {
                        await UpdateMissedSpirit(s);
                    }
                    catch (Exception ex)
                    {
                        Application.Current.MainPage.DisplayAlert("Exception", ex.Message, "Ok");
                    }

                    await progressBar.ProgressTo(progressBar.Progress + (0.75 / APISpirits.Count), 5, Easing.Linear);
                }



                resultSpiritsBasicsDTOs = APISpirits;
                SaveSpiritsToJson(resultSpiritsBasicsDTOs);
            }
            else
            {
                throw new Exception("No sources to retrieve spirits available!");
            }

            if (APISpiritsAvailible)
            {
                foreach(var spirit in resultSpiritsBasicsDTOs)
                {
                    try
                    {
                        string localFilePath = Path.Combine(dataDirectory, "MarkerImage_" + spirit.Id.ToString() + "_.png");
                        FileStream localFileStream = File.OpenRead(localFilePath);
                        localFileStream.Close();

                        localFilePath = Path.Combine(dataDirectory, "CardImage_" + spirit.Id.ToString() + "_.png");
                        localFileStream = File.OpenRead(localFilePath);
                        localFileStream.Close();
                    }
                    catch (Exception ex)
                    {
                        Application.Current.MainPage.DisplayAlert("Exception", ex.Message, "Ok");
                        await UpdateMissedSpirit(spirit);
                    }

                }
            }

            var Spirits = resultSpiritsBasicsDTOs.Select(s => _mapper.Map<MapSpirit>(s)).ToList();

            foreach(var spirit in Spirits)
            {
                spirit.CardImageRoute = dataDirectory + "/CardImage_" + spirit.Id + "_.png";
                spirit.MarkerImageRoute = dataDirectory + "/MarkerImage_" + spirit.Id + "_.png";
            }

            foreach(var spirit in Spirits)
            {
                var spiritDTO = resultSpiritsBasicsDTOs.Where(s => s.Id == spirit.Id).FirstOrDefault();
                foreach(var habitatId in spiritDTO.HabitatsIds)
                {
                    spirit.Habitats.Add(habitats.Where(h => h.Id == habitatId).FirstOrDefault());
                }
            }

            foreach (var spirit in Spirits)
            {
                spirit.HabitatsNames = spirit.Habitats[0].Name;
                for (int i = 1; i < spirit.Habitats.Count(); i++)
                {
                    spirit.HabitatsNames += ", " + spirit.Habitats[i].Name;
                }
            }
            return Spirits;
        }


        private async Task<List<GetSpiritBasicsDTO>> LoadJSONSpirits()
        {
            string localFilePath = Path.Combine(dataDirectory, "Spirits.json");
            using FileStream fileStream = File.OpenRead(localFilePath);
            var buffer = new byte[fileStream.Length];
            await fileStream.ReadAsync(buffer, 0, buffer.Length);
            string s = Encoding.Default.GetString(buffer);

            var JSONSpirits = JsonSerializer.Deserialize<List<GetSpiritBasicsDTO>>(s);
            return JSONSpirits;
        }

        private async Task SaveSpiritsToJson(List<GetSpiritBasicsDTO> spirits)
        {
            var json = JsonSerializer.Serialize(spirits);
            var spiritsMS = new MemoryStream();
            var spiritsSW = new StreamWriter(spiritsMS);
            spiritsSW.Write(json);
            spiritsSW.Flush();
            spiritsMS.Position = 0;



            try
            {
                string localFilePath = Path.Combine(dataDirectory, "Spirits.json");
                try
                {
                    File.Decrypt(localFilePath);
                }
                catch
                {

                }
                using FileStream fileStream = File.OpenWrite(localFilePath);
                await spiritsMS.CopyToAsync(fileStream);
            }catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private async Task<GetSpiritDTO> UpdateMissedSpirit(GetSpiritBasicsDTO spiritBasicsDTO)
        {
            var spirit = await _restService.GetSpiritAsync(spiritBasicsDTO.Id);

            MemoryStream markerImageMS = new MemoryStream(spirit.MarkerImage);
            string markerFilePath = Path.Combine(dataDirectory, "MarkerImage_" + spirit.Id.ToString() + "_.png");
            using FileStream markerFileStream = File.OpenWrite(markerFilePath);
            await markerImageMS.CopyToAsync(markerFileStream);
            markerFileStream.Close();

            MemoryStream cardImageMS = new MemoryStream(spirit.CardImage);
            string cardFilePath = Path.Combine(dataDirectory, "CardImage_" + spirit.Id.ToString() + "_.png");
            using FileStream cardFileStream = File.OpenWrite(cardFilePath);
            await cardImageMS.CopyToAsync(cardFileStream);
            cardFileStream.Close();


            return spirit;
        }

    }
}
