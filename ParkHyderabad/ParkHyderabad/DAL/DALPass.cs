using ParkHyderabad.Model.APIInputModel;
using ParkHyderabad.Model.APIOutPutModel;
using ParkHyderabad.Model.APIResponse;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace ParkHyderabad.DAL
{
    public class DALPass
    {
        public List<PassPrice> GetPassTypesByVehicleTypeID(string accessToken, CustomerVehicle objCustomerVehicle)
        {
            List<PassPrice> obj_PassPrice = new List<PassPrice>();
            try
            {
                string baseUrl = Convert.ToString(App.Current.Properties["BaseURL"]);
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // Add the Authorization header with the AccessToken.
                    client.DefaultRequestHeaders.Add("Authorization", "bearer  " + accessToken);
                    // create the URL string.
                    string url = "api/InstaConsumer/GetPassTypesByVehicleTypeID";
                    // make the request

                    var json = JsonConvert.SerializeObject(objCustomerVehicle);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = client.PostAsync(url, content).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = response.Content.ReadAsStringAsync().Result;
                        if (jsonString != null)
                        {
                            APIResponse apiResult = JsonConvert.DeserializeObject<APIResponse>(jsonString);

                            if (apiResult.Result)
                            {
                                obj_PassPrice = JsonConvert.DeserializeObject<List<PassPrice>>(Convert.ToString(apiResult.Object));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return obj_PassPrice;
        }
        public OCustomerVehiclePass InsertCustomerVehiclePass(string accessToken, CustomerVehiclePass objCustomerVehiclePass)
        {
            OCustomerVehiclePass obj_OCustomerVehiclePass = new OCustomerVehiclePass();
            try
            {
                string baseUrl = Convert.ToString(App.Current.Properties["BaseURL"]);
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // Add the Authorization header with the AccessToken.
                    client.DefaultRequestHeaders.Add("Authorization", "bearer  " + accessToken);
                    // create the URL string.
                    string url = "api/InstaConsumer/InsertCustomerVehiclePass";
                    // make the request

                    var json = JsonConvert.SerializeObject(objCustomerVehiclePass);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = client.PostAsync(url, content).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = response.Content.ReadAsStringAsync().Result;
                        if (jsonString != null)
                        {
                            APIResponse apiResult = JsonConvert.DeserializeObject<APIResponse>(jsonString);

                            if (apiResult.Result)
                            {
                                obj_OCustomerVehiclePass = JsonConvert.DeserializeObject<OCustomerVehiclePass>(Convert.ToString(apiResult.Object));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return obj_OCustomerVehiclePass;
        }
        public List<OCustomerVehiclePass> GetListOfPassesByCustomerID(string accessToken, CustomerVehicle objCustomerVehicle)
        {
            List<OCustomerVehiclePass> obj_listOCustomerVehiclePass = new List<OCustomerVehiclePass>();
            try
            {
                string baseUrl = Convert.ToString(App.Current.Properties["BaseURL"]);
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // Add the Authorization header with the AccessToken.
                    client.DefaultRequestHeaders.Add("Authorization", "bearer  " + accessToken);
                    // create the URL string.
                    string url = "api/InstaConsumer/GetListOfPassesByCustomerID";
                    // make the request

                    var json = JsonConvert.SerializeObject(objCustomerVehicle);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = client.PostAsync(url, content).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = response.Content.ReadAsStringAsync().Result;
                        if (jsonString != null)
                        {
                            APIResponse apiResult = JsonConvert.DeserializeObject<APIResponse>(jsonString);

                            if (apiResult.Result)
                            {
                                obj_listOCustomerVehiclePass = JsonConvert.DeserializeObject<List<OCustomerVehiclePass>>(Convert.ToString(apiResult.Object));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return obj_listOCustomerVehiclePass;
        }
        public OCustomerVehiclePass GetCustomerVehiclePassDetails(string accessToken, CustomerVehicle objCustomerVehicle)
        {
            OCustomerVehiclePass obj_OCustomerVehiclePass = new OCustomerVehiclePass();
            try
            {
                string baseUrl = Convert.ToString(App.Current.Properties["BaseURL"]);
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // Add the Authorization header with the AccessToken.
                    client.DefaultRequestHeaders.Add("Authorization", "bearer  " + accessToken);
                    // create the URL string.
                    string url = "api/InstaConsumer/GetCustomerVehiclePassDetails";
                    // make the request

                    var json = JsonConvert.SerializeObject(objCustomerVehicle);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = client.PostAsync(url, content).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = response.Content.ReadAsStringAsync().Result;
                        if (jsonString != null)
                        {
                            APIResponse apiResult = JsonConvert.DeserializeObject<APIResponse>(jsonString);

                            if (apiResult.Result)
                            {
                                obj_OCustomerVehiclePass = JsonConvert.DeserializeObject<OCustomerVehiclePass>(Convert.ToString(apiResult.Object));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return obj_OCustomerVehiclePass;
        }
        public OCustomerVehiclePass ValidateCustomerVehiclePass(string accessToken, CustomerVehicle objCustomerVehicle)
        {
            OCustomerVehiclePass obj_OCustomerVehiclePass = new OCustomerVehiclePass();
            try
            {
                string baseUrl = Convert.ToString(App.Current.Properties["BaseURL"]);
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // Add the Authorization header with the AccessToken.
                    client.DefaultRequestHeaders.Add("Authorization", "bearer  " + accessToken);
                    // create the URL string.
                    string url = "api/InstaConsumer/ValidateCustomerVehiclePass";
                    // make the request

                    var json = JsonConvert.SerializeObject(objCustomerVehicle);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = client.PostAsync(url, content).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = response.Content.ReadAsStringAsync().Result;
                        if (jsonString != null)
                        {
                            APIResponse apiResult = JsonConvert.DeserializeObject<APIResponse>(jsonString);

                            if (apiResult.Result)
                            {
                                obj_OCustomerVehiclePass = JsonConvert.DeserializeObject<OCustomerVehiclePass>(Convert.ToString(apiResult.Object));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return obj_OCustomerVehiclePass;
        }
        public OCustomerVehiclePass InsertMultiStationCustomerVehiclePass(string accessToken, CustomerVehiclePass objCustomerVehiclePass)
        {
            OCustomerVehiclePass obj_OCustomerVehiclePass = new OCustomerVehiclePass();
            try
            {
                string baseUrl = Convert.ToString(App.Current.Properties["BaseURL"]);
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(baseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    // Add the Authorization header with the AccessToken.
                    client.DefaultRequestHeaders.Add("Authorization", "bearer  " + accessToken);
                    // create the URL string.
                    string url = "api/InstaConsumer/InsertMultiStationCustomerVehiclePass";
                    // make the request

                    var json = JsonConvert.SerializeObject(objCustomerVehiclePass);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = client.PostAsync(url, content).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonString = response.Content.ReadAsStringAsync().Result;
                        if (jsonString != null)
                        {
                            APIResponse apiResult = JsonConvert.DeserializeObject<APIResponse>(jsonString);

                            if (apiResult.Result)
                            {
                                obj_OCustomerVehiclePass = JsonConvert.DeserializeObject<OCustomerVehiclePass>(Convert.ToString(apiResult.Object));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return obj_OCustomerVehiclePass;
        }
                
    }
}
