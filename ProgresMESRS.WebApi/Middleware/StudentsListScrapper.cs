using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using UniversityStudent = ProgresMESRS.Middleware.API.Models.UniversityStudent;

namespace ProgresMESRS.Middleware.API
{
    // XPATH : //body/div/div/form/div/div/table
    public class StudentsListScrapper : IPageScrapper<UniversityStudent>
    {
        public IEnumerable<UniversityStudent> ExtractData(IEnumerable<string> pages)
        {
            IEnumerable<UniversityStudent> studentList = new List<UniversityStudent>();
            studentList = pages.AsParallel().SelectMany(page => ExtractData(page)).ToList();

            return studentList;
        }
        public IEnumerable<UniversityStudent> ExtractData(string pageHtml)
        {
            var tableXpath = "//body/div/div/form/div/div/table";

            IEnumerable<UniversityStudent> studentList = new List<UniversityStudent>();
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(pageHtml);

            var studentsRows = htmlDoc
                .DocumentNode
                .SelectNodes(tableXpath)
                .Descendants("tr")
                .ToList();

            var students = studentsRows
                .Select(row => row.Descendants("td").ToArray())
                .Where(x => x.Length >= 6)
                .ToList();

            var result = students.Select(column =>
                {
                    try 
                    {
                        return new UniversityStudent()
                        {
                            Index = Convert.ToInt32(column[0].InnerText),
                            Matricule = column[1].InnerText.Trim().ToUpper(),
                            NIN = column[2].InnerText.Trim().ToUpper(),
                            FamilyName = column[3].InnerText.Trim().ToUpper(),
                            FirstName = column[4].InnerText.Trim().ToUpper(),
                            DateOfBirth = column[5].InnerText.Trim()
                        };
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return null;
                    }        
                })
                .ToList();

            return result;
        }

        public UniversityStudent ExtractSingleData(string pageHtml, string id)
        {
            throw new NotImplementedException();
        }
    }
    public class StudentsListScrapperXml : IPageScrapper<UniversityStudent>
    {
        public IEnumerable<UniversityStudent> ExtractData(string pageHtml)
        {
            var tableXpath = "//update";

            IEnumerable<UniversityStudent> studentList = new List<UniversityStudent>();
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(pageHtml);

            var studentsRows = htmlDoc
                .DocumentNode
                .SelectNodes(tableXpath)
                .Descendants("tr")
                .ToList();

            var students = studentsRows
                .Select(row => row.Descendants("td").ToArray())
                .Where(x => x.Length >= 6)
                .ToList();

            var result = students.Select(column =>
            {
                try
                {
                    return new UniversityStudent()
                    {
                        Index = Convert.ToInt32(column[0].InnerText),
                        Matricule = column[1].InnerText.Trim().ToUpper(),
                        NIN = column[2].InnerText.Trim().ToUpper(),
                        FamilyName = column[3].InnerText.Trim().ToUpper(),
                        FirstName = column[4].InnerText.Trim().ToUpper(),
                        DateOfBirth = column[5].InnerText.Trim()
                    };
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            })
                .ToList();

            return result;
        }

        public UniversityStudent ExtractSingleData(string pageHtml, string id)
        {
            throw new NotImplementedException();
        }
    }

}
