﻿using Exceptions;
using Mappers;
using models;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Net.Http;

namespace cepService
{
    public class WebManiaCepService : CepService
    {
        private readonly HttpClient _client;
        private readonly string _appKey = ConfigurationManager.AppSettings.Get("appKeyWebManiaCep");
        private readonly string _appSecret = ConfigurationManager.AppSettings.Get("appSecretWebManiaCep");

        public WebManiaCepService()
        {
            try
            {
                _client = new HttpClient
                {
                    BaseAddress = new Uri("https://webmaniabr.com/api/1/cep/")
                };
                _client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            }
            catch (Exception ex)
            {
                if (_client != null)
                    _client.Dispose();

                throw new WebManiaCepServiceException(ex);
            }

        }
        public override AdressCep GetAdressCep(string cep)
        {
            try
            {
                HttpResponseMessage response = _client.GetAsync(String.Format("{0}/?app_key={1}&app_secret={2}", cep, _appKey, _appSecret)).Result;

                if (response.IsSuccessStatusCode)
                {
                    var adressWMCep = JsonConvert.DeserializeObject<AdressWebManiaCep>(response.Content.ReadAsStringAsync().Result);

                    return AdressWebManiaCepMapper.ToAdressCep(adressWMCep);
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw new WebManiaCepServiceException(ex);
            }
            finally
            {
                if (_client != null)
                    _client.Dispose();
            }
        }
    }
}
