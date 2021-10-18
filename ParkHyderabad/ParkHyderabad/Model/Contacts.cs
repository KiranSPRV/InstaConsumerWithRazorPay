using System;
using System.Collections.Generic;
using System.Text;
using ParkHyderabad.Model.APIOutPutModel;

namespace ParkHyderabad.Model
{
    public class Contacts
    {
        public static IEnumerable<Contact> Get()
        {
            IEnumerable<Contact> lstEnumerable;
            List<Contact> lstContact = new List<Contact>();
            Contact objcontact = new Contact();
            objcontact.UserId = "1";
            objcontact.FirstName = "HSBC";
            objcontact.LastName = "No 7 HSBC Center, Mahatma Gandhi Rd";
            objcontact.Email = "contact.one@gmail.com";
            objcontact.Phone = "1234131332";
            objcontact.PhotoUrl = "profilepic.png";
            objcontact.Country = "India";

            objcontact.BikeWash = "resource://ParkHyderabad.Resources.bikewash.svg";
            objcontact.CarWash = "resource://ParkHyderabad.Resources.carwashgreen.svg";
            objcontact.CoveredParking = "resource://ParkHyderabad.Resources.shelter.svg";
            objcontact.DisabledParking = "resource://ParkHyderabad.Resources.disabledparking.svg";
            objcontact.Petrol = "resource://ParkHyderabad.Resources.petrol.svg";
            objcontact.Mechanic = "resource://ParkHyderabad.Resources.servicegreen.svg";

            objcontact.lstServices = new List<LocationParkingLotService>();

            LocationParkingLotService objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.bikewash.svg";            

            objcontact.lstServices.Add(objService);

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.carwashgreen.svg";

            objcontact.lstServices.Add(objService);

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.shelter.svg";

            objcontact.lstServices.Add(objService);

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.disabledparking.svg";

            objcontact.lstServices.Add(objService);

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.petrol.svg";

            objcontact.lstServices.Add(objService);

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.servicegreen.svg";

            objcontact.lstServices.Add(objService);

            lstContact.Add(objcontact);

            objcontact = new Contact();
            objcontact.UserId = "2";
            objcontact.FirstName = "Chikkadpally";
            objcontact.LastName = "No 7 Chikkadpally Center, Mahatma Gandhi Rd";
            objcontact.Email = "contact.one@gmail.com";
            objcontact.Phone = "1234131332";
            objcontact.PhotoUrl = "profilepic.png";
            objcontact.Country = "India";

            objcontact.BikeWash = "resource://ParkHyderabad.Resources.bikewashgreen.svg";
            objcontact.CarWash = "resource://ParkHyderabad.Resources.carwashgreen.svg";
            objcontact.CoveredParking = "resource://ParkHyderabad.Resources.shelter.svg";
            objcontact.DisabledParking = "resource://ParkHyderabad.Resources.disabledparking.svg";
            objcontact.Petrol = "resource://ParkHyderabad.Resources.petrol.svg";
            objcontact.Mechanic = "resource://ParkHyderabad.Resources.service.svg";

            objcontact.lstServices = new List<LocationParkingLotService>();

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.bikewashgreen.svg";

            objcontact.lstServices.Add(objService);

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.carwashgreen.svg";

            objcontact.lstServices.Add(objService);

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.shelter.svg";

            objcontact.lstServices.Add(objService);

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.disabledparking.svg";

            objcontact.lstServices.Add(objService);

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.petrol.svg";

            objcontact.lstServices.Add(objService);

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.service.svg";

            objcontact.lstServices.Add(objService);

            lstContact.Add(objcontact);
           
            objcontact = new Contact();
            objcontact.UserId = "3";
            objcontact.FirstName = "Dilsukhnagar";
            objcontact.LastName = "No 7 Dilsukhnagar Center, Mahatma Gandhi Rd";
            objcontact.Email = "contact.one@gmail.com";
            objcontact.Phone = "1234131332";
            objcontact.PhotoUrl = "profilepic.png";
            objcontact.Country = "India";

            objcontact.BikeWash = "resource://ParkHyderabad.Resources.bikewash.svg";
            objcontact.CarWash = "resource://ParkHyderabad.Resources.carwash.svg";
            objcontact.CoveredParking = "resource://ParkHyderabad.Resources.shelter.svg";
            objcontact.DisabledParking = "resource://ParkHyderabad.Resources.disabledparkinggreen.svg";
            objcontact.Petrol = "resource://ParkHyderabad.Resources.petrolgreen.svg";
            objcontact.Mechanic = "resource://ParkHyderabad.Resources.servicegreen.svg";

            objcontact.lstServices = new List<LocationParkingLotService>();

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.bikewash.svg";

            objcontact.lstServices.Add(objService);

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.carwash.svg";

            objcontact.lstServices.Add(objService);

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.shelter.svg";

            objcontact.lstServices.Add(objService);

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.disabledparkinggreen.svg";

            objcontact.lstServices.Add(objService);

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.petrolgreen.svg";

            objcontact.lstServices.Add(objService);

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.servicegreen.svg";

            objcontact.lstServices.Add(objService);

            lstContact.Add(objcontact);

            objcontact = new Contact();
            objcontact.UserId = "4";
            objcontact.FirstName = "Ammerpet";
            objcontact.LastName = "No 7 Ammerpet Center, Mahatma Gandhi Rd";
            objcontact.Email = "contact.one@gmail.com";
            objcontact.Phone = "1234131332";
            objcontact.PhotoUrl = "profilepic.png";
            objcontact.Country = "India";

            objcontact.BikeWash = "resource://ParkHyderabad.Resources.bikewash.svg";
            objcontact.CarWash = "resource://ParkHyderabad.Resources.carwash.svg";
            objcontact.CoveredParking = "resource://ParkHyderabad.Resources.sheltergreen.svg";
            objcontact.DisabledParking = "resource://ParkHyderabad.Resources.disabledparking.svg";
            objcontact.Petrol = "resource://ParkHyderabad.Resources.petrolgreen.svg";
            objcontact.Mechanic = "resource://ParkHyderabad.Resources.service.svg";

            objcontact.lstServices = new List<LocationParkingLotService>();

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.bikewash.svg";

            objcontact.lstServices.Add(objService);

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.carwash.svg";

            objcontact.lstServices.Add(objService);

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.sheltergreen.svg";

            objcontact.lstServices.Add(objService);

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.disabledparking.svg";

            objcontact.lstServices.Add(objService);

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.petrolgreen.svg";

            objcontact.lstServices.Add(objService);

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.service.svg";

            objcontact.lstServices.Add(objService);

            lstContact.Add(objcontact);

            objcontact = new Contact();
            objcontact.UserId = "5";
            objcontact.FirstName = "Panjagutta";
            objcontact.LastName = "No 7 Panjagutta Center, Mahatma Gandhi Rd";
            objcontact.Email = "contact.one@gmail.com";
            objcontact.Phone = "1234131332";
            objcontact.PhotoUrl = "profilepic.png";
            objcontact.Country = "India";

            objcontact.BikeWash = "resource://ParkHyderabad.Resources.bikewashgreen.svg";
            objcontact.CarWash = "resource://ParkHyderabad.Resources.carwashgreen.svg";
            objcontact.CoveredParking = "resource://ParkHyderabad.Resources.shelter.svg";
            objcontact.DisabledParking = "resource://ParkHyderabad.Resources.disabledparking.svg";
            objcontact.Petrol = "resource://ParkHyderabad.Resources.petrol.svg";
            objcontact.Mechanic = "resource://ParkHyderabad.Resources.service.svg";

            objcontact.lstServices = new List<LocationParkingLotService>();

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.bikewashgreen.svg";

            objcontact.lstServices.Add(objService);

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.carwashgreen.svg";

            objcontact.lstServices.Add(objService);

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.shelter.svg";

            objcontact.lstServices.Add(objService);

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.disabledparking.svg";

            objcontact.lstServices.Add(objService);

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.petrol.svg";

            objcontact.lstServices.Add(objService);

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.service.svg";
            objcontact.lstServices.Add(objService);

            lstContact.Add(objcontact);

            objcontact = new Contact();
            objcontact.UserId = "6";
            objcontact.FirstName = "JNTU";
            objcontact.LastName = "No 7 JNTU Center, Mahatma Gandhi Rd";
            objcontact.Email = "contact.one@gmail.com";
            objcontact.Phone = "1234131332";
            objcontact.PhotoUrl = "profilepic.png";
            objcontact.Country = "India";

            objcontact.BikeWash = "resource://ParkHyderabad.Resources.bikewash.svg";
            objcontact.CarWash = "resource://ParkHyderabad.Resources.carwash.svg";
            objcontact.CoveredParking = "resource://ParkHyderabad.Resources.shelter.svg";
            objcontact.DisabledParking = "resource://ParkHyderabad.Resources.disabledparking.svg";
            objcontact.Petrol = "resource://ParkHyderabad.Resources.petrol.svg";
            objcontact.Mechanic = "resource://ParkHyderabad.Resources.service.svg";

            objcontact.lstServices = new List<LocationParkingLotService>();

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.bikewash.svg";

            objcontact.lstServices.Add(objService);

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.carwash.svg";

            objcontact.lstServices.Add(objService);

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.shelter.svg";

            objcontact.lstServices.Add(objService);

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.disabledparking.svg";

            objcontact.lstServices.Add(objService);

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.petrol.svg";

            objcontact.lstServices.Add(objService);

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.service.svg";

            objcontact.lstServices.Add(objService);

            lstContact.Add(objcontact);

            objcontact = new Contact();
            objcontact.UserId = "7";
            objcontact.FirstName = "Kukatpally";
            objcontact.LastName = "No 7 Kukatpally Center, Mahatma Gandhi Rd";
            objcontact.Email = "contact.one@gmail.com";
            objcontact.Phone = "1234131332";
            objcontact.PhotoUrl = "profilepic.png";
            objcontact.Country = "India";

            objcontact.BikeWash = "resource://ParkHyderabad.Resources.bikewashgreen.svg";
            objcontact.CarWash = "resource://ParkHyderabad.Resources.carwashgreen.svg";
            objcontact.CoveredParking = "resource://ParkHyderabad.Resources.sheltergreen.svg";
            objcontact.DisabledParking = "resource://ParkHyderabad.Resources.disabledparkinggreen.svg";
            objcontact.Petrol = "resource://ParkHyderabad.Resources.petrolgreen.svg";
            objcontact.Mechanic = "resource://ParkHyderabad.Resources.servicegreen.svg";

            objcontact.lstServices = new List<LocationParkingLotService>();

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.bikewashgreen.svg";

            objcontact.lstServices.Add(objService);

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.carwashgreen.svg";

            objcontact.lstServices.Add(objService);

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.sheltergreen.svg";

            objcontact.lstServices.Add(objService);

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.disabledparkinggreen.svg";

            objcontact.lstServices.Add(objService);

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.petrolgreen.svg";

            objcontact.lstServices.Add(objService);

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.servicegreen.svg";

            objcontact.lstServices.Add(objService);

            lstContact.Add(objcontact);

            objcontact = new Contact();
            objcontact.UserId = "8";
            objcontact.FirstName = "Miyapur";
            objcontact.LastName = "No 7 Miyapur Center, Mahatma Gandhi Rd";
            objcontact.Email = "contact.one@gmail.com";
            objcontact.Phone = "1234131332";
            objcontact.PhotoUrl = "profilepic.png";
            objcontact.Country = "India";

            objcontact.BikeWash = "resource://ParkHyderabad.Resources.bikewashgreen.svg";
            objcontact.CarWash = "resource://ParkHyderabad.Resources.carwashgreen.svg";
            objcontact.CoveredParking = "resource://ParkHyderabad.Resources.shelter.svg";
            objcontact.DisabledParking = "resource://ParkHyderabad.Resources.disabledparking.svg";
            objcontact.Petrol = "resource://ParkHyderabad.Resources.petrol.svg";
            objcontact.Mechanic = "resource://ParkHyderabad.Resources.service.svg";

            objcontact.lstServices = new List<LocationParkingLotService>();

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.bikewashgreen.svg";

            objcontact.lstServices.Add(objService);

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.carwashgreen.svg";

            objcontact.lstServices.Add(objService);

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.shelter.svg";

            objcontact.lstServices.Add(objService);

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.disabledparking.svg";

            objcontact.lstServices.Add(objService);

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.petrol.svg";

            objcontact.lstServices.Add(objService);

            objService = new LocationParkingLotService();
            objService.ServiceTypeName = "resource://ParkHyderabad.Resources.service.svg";

            objcontact.lstServices.Add(objService);

            lstContact.Add(objcontact);

            lstEnumerable = lstContact;

            return lstEnumerable;

            /*
            return new List<Contact>
            {
              new Contact() {UserId="1", FirstName="HSBC", LastName="No 7 HSBC Center, Mahatma Gandhi Rd", 
                  Email="contact.one@gmail.com", Phone="1234131332", PhotoUrl="profilepic.png",Country="India" },
              new Contact() {UserId="2", FirstName="Chikkadpally", LastName="No 7 HSBC Center, Mahatma Gandhi Rd",
                  Email="contact.two@gmail.com", Phone="1289413132", PhotoUrl="profile.png",Country="India" },
              new Contact() {UserId="3",FirstName="Dilsukhnagar", LastName="No 7 HSBC Center, Mahatma Gandhi Rd",
                  Email="contact.three@gmail.com",Phone="4234242235", PhotoUrl="profilepic.png",Country="India" },
              new Contact() {UserId="4", FirstName="Ammerpet", LastName="No 7 HSBC Center, Mahatma Gandhi Rd", 
                  Email="contact.four@gmail.com",Phone="6443245633", PhotoUrl="profile.png",Country="India" },
              new Contact() {UserId="5", FirstName="Panjagutta", LastName="No 7 HSBC Center, Mahatma Gandhi Rd",
                  Email="contact.five@gmail.com",Phone="4234242235", PhotoUrl="profilepic.png",Country="India" },
              new Contact() {UserId="6", FirstName="JNTU", LastName="No 7 HSBC Center, Mahatma Gandhi Rd", 
                  Email="contact.six@gmail.com",Phone="2344324443", PhotoUrl="profile.png",Country="India" },
              new Contact() {UserId="7", FirstName="Kukatpally", LastName="No 7 HSBC Center, Mahatma Gandhi Rd",
                  Email="contact.seven@gmail.com",Phone="2344234234", PhotoUrl="profilepic.png",Country="India" },
              new Contact() {UserId="8", FirstName="Miyapur", LastName="No 7 HSBC Center, Mahatma Gandhi Rd",
                  Email="contact.eight@gmail.com",Phone="4234242235", PhotoUrl="profile.png",Country="India" },
            };
            */
        }

        public static IEnumerable<Contact> GetVehicleTypes()
        {
            return new List<Contact>
            {
              new Contact() {UserId="1", FirstName="HSBC", LastName="No 7 HSBC Center, Mahatma Gandhi Rd", 
                  Email="contact.one@gmail.com", Phone="1234131332", PhotoUrl="resource://ParkHyderabad.Resources.Carselect.svg",Country="India" },
              new Contact() {UserId="2", FirstName="Chikkadpally", LastName="No 7 HSBC Center, Mahatma Gandhi Rd",
                  Email="contact.two@gmail.com", Phone="1289413132", PhotoUrl="resource://ParkHyderabad.Resources.Bikeunselect.svg",Country="India" }
             };           
        }
    }
}
