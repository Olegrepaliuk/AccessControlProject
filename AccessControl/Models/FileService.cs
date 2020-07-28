using AccessControlModels;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AccessControl.Models
{
    public class FileService
    {
        private string baseAddress = "https://localhost:44381/api";
        public async Task UpdatePeopleFile(string token)
        {
            var client = new HttpClient();
            List<Person> allPeople = new List<Person>();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync($"{baseAddress}/people");
            if (response.IsSuccessStatusCode)
            {
                var people = await response.Content.ReadAsAsync<IEnumerable<Person>>();
                allPeople = people.ToList();
            }

            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                excelPackage.Workbook.Properties.Author = "Admin";
                excelPackage.Workbook.Properties.Title = "People";
                excelPackage.Workbook.Properties.Subject = "Information";
                excelPackage.Workbook.Properties.Created = DateTime.Now;

 
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");

                worksheet.Cells["A1"].Value = "ID";
                worksheet.Cells["B1"].Value = "Full name";
                worksheet.Cells["C1"].Value = "Card key";
                worksheet.Cells["D1"].Value = "Card valid til";
                worksheet.Cells["E1"].Value = "Email";
                worksheet.Cells["F1"].Value = "Phone";
                worksheet.Cells["G1"].Value = "Position";

                worksheet.Cells["D2:D"].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;

                int i = 2;
                foreach(var person in allPeople)
                {
                    worksheet.Cells[i, 1].Value = person.Id;
                    worksheet.Cells[i, 2].Value = person.Name;
                    worksheet.Cells[i, 3].Value = person.CardKey;
                    worksheet.Cells[i, 4].Value = person.CardValidTil.ToString();
                    worksheet.Cells[i, 5].Value = person.Email;
                    worksheet.Cells[i, 6].Value = person.Phone;
                    worksheet.Cells[i, 7].Value = person.Position;
                    i++;
                }

                FileInfo fi = new FileInfo("wwwroot/Files/People.xlsx");
                excelPackage.SaveAs(fi);
            }

        }
        public async Task UpdateReadersFile(string token)
        {

            var client = new HttpClient();
            List<Reader> allReaders = new List<Reader>();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync($"{baseAddress}/readers");
            if (response.IsSuccessStatusCode)
            {
                allReaders = await response.Content.ReadAsAsync<List<Reader>>();
            }

            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                excelPackage.Workbook.Properties.Author = "Admin";
                excelPackage.Workbook.Properties.Title = "Readers";
                excelPackage.Workbook.Properties.Subject = "Information";
                excelPackage.Workbook.Properties.Created = DateTime.Now;


                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");

                worksheet.Cells["A1"].Value = "ID";
                worksheet.Cells["B1"].Value = "Current location";
                worksheet.Cells["C1"].Value = "Next location";
                worksheet.Cells["D1"].Value = "Description";

                int i = 2;
                foreach (var reader in allReaders)
                {
                    worksheet.Cells[i, 1].Value = reader.Id;
                    worksheet.Cells[i, 2].Value = reader.CurrentLoc != null ? reader.CurrentLoc.ToString() : "Outdoors";
                    worksheet.Cells[i, 3].Value = reader.NextLoc != null ? reader.NextLoc.ToString() : "Outdoors";
                    worksheet.Cells[i, 4].Value = reader.Description;
                    i++;
                }

                FileInfo fi = new FileInfo("wwwroot/Files/Readers.xlsx");
                excelPackage.SaveAs(fi);
            }
        }
        public async Task UpdateRoomsFile(string token)
        {
            var client = new HttpClient();
            List<Room> allRooms = new List<Room>();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync($"{baseAddress}/rooms");
            if (response.IsSuccessStatusCode)
            {
                var rooms = await response.Content.ReadAsAsync<IEnumerable<Room>>();
                allRooms = rooms.ToList();
            }

            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                excelPackage.Workbook.Properties.Author = "Admin";
                excelPackage.Workbook.Properties.Title = "Rooms";
                excelPackage.Workbook.Properties.Subject = "Information";
                excelPackage.Workbook.Properties.Created = DateTime.Now;


                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");

                worksheet.Cells["A1"].Value = "ID";
                worksheet.Cells["B1"].Value = "Name";
                worksheet.Cells["C1"].Value = "Area";
                worksheet.Cells["D1"].Value = "Type";
                worksheet.Cells["E1"].Value = "Description";

                int i = 2;
                foreach (var room in allRooms)
                {
                    worksheet.Cells[i, 1].Value = room.Id;
                    worksheet.Cells[i, 2].Value = room.Name;
                    worksheet.Cells[i, 3].Value = room.Area;
                    worksheet.Cells[i, 4].Value = room.Type.ToString();
                    worksheet.Cells[i, 5].Value = room.Description;

                    i++;
                }

                FileInfo fi = new FileInfo("wwwroot/Files/Rooms.xlsx");
                excelPackage.SaveAs(fi);
            }
        }
        public async Task UpdateReportFile(string token)
        {
            var client = new HttpClient();
            List<Relocation> allRelocations = new List<Relocation>();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync($"{baseAddress}/relocations");
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                allRelocations = JsonConvert.DeserializeObject<IEnumerable<Relocation>>(json).ToList();
            }

            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                excelPackage.Workbook.Properties.Author = "Admin";
                excelPackage.Workbook.Properties.Title = "Relocations";
                excelPackage.Workbook.Properties.Subject = "History";
                excelPackage.Workbook.Properties.Created = DateTime.Now;


                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");

                worksheet.Cells["A1"].Value = "Date";
                worksheet.Cells["B1"].Value = "Time";
                worksheet.Cells["C1"].Value = "Person";
                worksheet.Cells["D1"].Value = "From";
                worksheet.Cells["E1"].Value = "To";
                worksheet.Cells["F1"].Value = "Status";

                worksheet.Cells["A2:A"].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                worksheet.Cells["B2:B"].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortTimePattern;

                int i = 2;
                foreach (var relocation in allRelocations)
                {
                    worksheet.Cells[i, 1].Value = relocation.DateAndTime.Date.ToString("dd.MM.yyyy");
                    worksheet.Cells[i, 2].Value = relocation.DateAndTime.ToString("HH:mm:ss");
                    worksheet.Cells[i, 3].Value = relocation.Person != null ? relocation.Person.Name : "Unknown";
                    worksheet.Cells[i, 4].Value = relocation.FromLoc != null ? relocation.FromLoc.ToString() : "Outdoors";
                    worksheet.Cells[i, 5].Value = relocation.ToLoc != null ? relocation.ToLoc.ToString() : "Outdoors";
                    worksheet.Cells[i, 6].Value = relocation.Success ? "Access allowed" : "Access denied";
                    i++;
                }

                FileInfo fi = new FileInfo("wwwroot/Files/Report.xlsx");
                excelPackage.SaveAs(fi);
            }
        }
        public async Task UpdateDatesReportFile(string token)
        {
            var client = new HttpClient();
            var allGroupedRelocations = new List<IGrouping<DateTime, Relocation>>();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync(baseAddress);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<IEnumerable<Relocation>>(json);
                allGroupedRelocations = result.GroupBy(rel => rel.DateAndTime.Date).OrderByDescending(rel => rel.Key).ToList();
            }
        }
        public async Task UpdatePeopleReportFile(string token)
        {

        }
        public async Task UpdateRoomsEnteringReportFile(string token)
        {

        }
        public async Task UpdateRoomsLeavingReportFile(string token)
        {

        }
    }
}
