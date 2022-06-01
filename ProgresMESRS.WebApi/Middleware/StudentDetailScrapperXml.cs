using HtmlAgilityPack;
using ProgresMESRS.Middleware.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreLinq.Extensions;
using ProgresMESRS.Middleware.API.Helpers;
using Akavache;
using Akavache.Core;
using Akavache.Internal;
using System.Reactive;
using System.Reactive.Linq;
using System.Net.Http;
using CefSharp;

namespace ProgresMESRS.Middleware.API
{
    public class StudentDetailsExtractor
    {
        static decimal GetElementDecimal(HtmlNode node, string id)
        {
            try
            {
                return Convert.ToDecimal(GetElementValue(node, id));
            }
            catch
            {
                return 0;
            }
        }
        static string GetElementValue(HtmlNode node, string id)
        {
            try
            {
                var config = Config.GetConfig();

                var temp = node.Descendants("td")
                    .SkipUntil(x => x.InnerText.Contains(id, StringComparison.InvariantCultureIgnoreCase));
                var value = temp
                    .Take(1)
                    .First();
                if (config.StudentDetailsLabels.Contains(value.InnerText, StringComparer.InvariantCultureIgnoreCase))
                {
                    return string.Empty;
                }
                else
                {
                    return value.InnerText;
                }
            }
            catch
            {
                return string.Empty;
            }
        }
        static List<SportsSituation> GetSportsSituations(HtmlNode node)
        {
            try
            {
                var array = node.Descendants("tbody")
                    .Where(x => x.Id.Contains("tabView:sportifDataTable_data"))
                    .SelectMany(x => x.Descendants("tr"))
                    .ToList();

                var needs = array
                    .Select(x =>
                    {
                        var columns = x.Descendants("td").ToArray();
                        return new SportsSituation()
                        {
                            Number = columns[0].InnerText,
                            Discipline = columns[1].InnerText,
                            DisciplineArabic = columns[2].InnerText,
                            StartDate = columns[3].InnerText,
                            EndDate = columns[4].InnerText,
                            Observation = columns[5].InnerText
                        };
                    }).ToList();
                return needs;
            }
            catch
            {
                return new List<SportsSituation>();
            }
        }
        static List<SpecialNeed> GetSpecialNeeds(HtmlNode node)
        {
            try
            {
                var array = node.Descendants("tbody")
                    .Where(x => x.Id.Contains("tabView:handicapDataTable_data"))
                    .SelectMany(x => x.Descendants("tr"))
                    .ToList();

                var needs = array
                    .Select(x =>
                    {
                        var columns = x.Descendants("td").ToArray();
                        return new SpecialNeed()
                        {
                            NeedType = columns[0].InnerText,
                            NeedTypeArabic = columns[1].InnerText,
                            StartDate = columns[2].InnerText,
                            EndDate = columns[3].InnerText,
                            Observation = columns[4].InnerText
                        };
                    }).ToList();
                return needs;
            }
            catch
            {
                return new List<SpecialNeed>();
            }
        }
        static List<StudentTransfer> GetTransfers(HtmlNode node)
        {
            try
            {
                var array = node.Descendants("tbody")
                    .Where(x => x.Id.Contains("tabView:j_idt635_data"))
                    .SelectMany(x => x.Descendants("tr"))
                    .ToList();

                var transfers = array
                    .Select(x =>
                    {
                        var columns = x.Descendants("td").ToArray();
                        return new StudentTransfer()
                        {
                            AcademicYear = columns[0].InnerText,
                            Date = columns[1].InnerText,
                            TransferType = columns[2].InnerText,
                            Reason = columns[3].InnerText,
                            OriginDomain = columns[4].InnerText,
                            OriginBranch = columns[5].InnerText,
                            RequestedDomain = columns[6].InnerText,
                            RequestedBranch = columns[7].InnerText,
                            OriginInstitution = columns[8].InnerText,
                            RequestedInstitution = columns[9].InnerText,
                            FinalDecision = columns[10].InnerText,
                        };
                    }).ToList();
                return transfers;
            }
            catch
            {
                return new List<StudentTransfer>();
            }
        }
        static List<AnnualReview> GetAnnualReviews(HtmlNode node)
        {
            try
            {
                var array = node.Descendants("tbody")
                    .Where(x => x.Id.Contains("tabView:j_idt617_data"))
                    .SelectMany(x => x.Descendants("tr"))
                    .ToList();

                var reviews = array
                    .Select(x =>
                    {
                        var columns = x.Descendants("td").ToArray();
                        return new AnnualReview()
                        {
                            AcademicYear = columns[0].InnerText,
                            University = columns[1].InnerText,
                            Level = columns[2].InnerText,
                            Cycle = columns[3].InnerText,
                            Score = Convert.ToDecimal(columns[4].InnerText),
                            Credit = Convert.ToDecimal(columns[5].InnerText),
                            Mention = columns[6].InnerText,
                            Decision = columns[7].InnerText
                        };
                    }).ToList();
                return reviews;
            }
            catch
            {
                return new List<AnnualReview>();
            }
        }
        static List<Grade> GetGrades(HtmlNode node)
        {
            try
            {
                var array = node.Descendants("tbody")
                    .Where(x => x.Id.Contains("lignesreleveData_data"))
                    .SelectMany(x => x.Descendants("tr"))
                    .ToList();

                var grades = array
                    .Select(x =>
                    {
                        var columns = x.Descendants("td").ToArray();
                        return new Grade()
                        {
                            ClassSubject = columns[0].InnerText,
                            ClassSubjectArabic = columns[1].InnerText,
                            Score = Convert.ToDecimal(columns[2].InnerText),
                            BaseScore = Convert.ToDecimal(columns[3].InnerText)
                        };
                    }).ToList();
                return grades;
            }
            catch
            {
                return new List<Grade>();
            }
        }
        static List<StudentEmail> GetEmails(HtmlNode node)
        {
            try
            {
                var array = node.Descendants("tbody")
                    .Where(x => x.Id.Contains("coordonnesEmailData_data"))
                    .SelectMany(x => x.Descendants("tr"))
                    .ToList();

                var emails = array
                    .Select(x =>
                    {
                        var columns = x.Descendants("td").ToArray();
                        return new StudentEmail()
                        {
                            EmailType = columns[0].InnerText,
                            AddressType = columns[1].InnerText,
                            Email = columns[2].InnerText
                        };
                    }).ToList();
                return emails;
            }
            catch
            {
                return new List<StudentEmail>();
            }
        }
        static List<StudentPhone> GetPhones(HtmlNode node)
        {
            try
            {
                var array = node.Descendants("tbody")
                    .Where(x => x.Id.Contains("coordonnesTelephoneData_data"))
                    .SelectMany(x => x.Descendants("tr"))
                    .ToList();
                var phones = array
                    .Select(x =>
                    {
                        var columns = x.Descendants("td").ToArray();
                        return new StudentPhone()
                        {
                            PhoneType = columns[0].InnerText,
                            AddressType = columns[1].InnerText,
                            PhoneNumber = columns[2].InnerText
                        };
                    }).ToList();
                return phones;
            }
            catch
            {
                return new List<StudentPhone>();
            }
        }
        static List<StudentAddress> GetAddresses(HtmlNode node)
        {
            try
            {
                var array = node.Descendants("tbody")
                    .Where(x => x.Id.Contains("coordonnesAdresseData_data"))
                    .SelectMany(x => x.Descendants("tr"))
                    .ToList();
                var addresses = array
                    .Select(x =>
                    {
                        var columns = x.Descendants("td").ToArray();
                        return new StudentAddress()
                        {
                            AddressType = columns[0].InnerText,
                            Country = columns[5].InnerText,
                            State = columns[4].InnerText,
                            City = columns[3].InnerText,
                            Province =  columns[2].InnerText,
                            Address = columns[1].InnerText
                        };
                    }).ToList();
                return addresses;
            }
            catch
            {
                return new List<StudentAddress>();
            }
        }
        static List<Inscription> GetInscriptions(HtmlNode node)
        {
            try
            {
                var array = node.Descendants("tbody")
                    .Where(x => x.Id == "tabView:j_idt603_data")
                    .SelectMany(x => x.Descendants("tr"))
                    .ToList();
                var inscriptions = array
                    .Select(x =>
                    {
                        var columns = x.Descendants("td").ToArray();
                        return new Inscription()
                        {
                            AcademicYear = columns[0].InnerText,
                            Cycle = columns[4].InnerText,
                            Formation = columns[5].InnerText,
                            Level = columns[3].InnerText,
                            RegistrationNumber = columns[2].InnerText,
                            University = columns[1].InnerText,
                        };
                    }).ToList();
                return inscriptions;
            }
            catch
            {
                return new List<Inscription>();
            }
        }
        public static StudentDetail Extract(HtmlNode node)
        {
            StudentDetail model = new StudentDetail();
            model.Matricule = GetElementValue(node, "Matricule");
            model.NIN = GetElementValue(node, "Identifiant");
            model.DateOfBirth = GetElementValue(node, "Date de naissance");

            model.FamilyNameArabic = GetElementValue(node, "Nom arabe");
            model.FirstNameArabic = GetElementValue(node, "Prenom arabe");
            model.FamilyName = GetElementValue(node, "Nom");
            model.FirstName = GetElementValue(node, "Prenom");

            model.RegistrationDetails = new RegistrationDetails()
            {
                RegisterDate = GetElementValue(node, "Date création"),
                CurrentAcademicYear = GetElementValue(node, "Année academique"),
                CurrentBranch = GetElementValue(node, "Filière"),
                CurrentCycle = GetElementValue(node, "Cycle"),
                CurrentFormation = GetElementValue(node, "Offre de formation"),
                CurrentLevel = GetElementValue(node, "Niveau"),
                CurrentSpeciality = GetElementValue(node, "Spécialité"),
                CurrentDomain = GetElementValue(node, "domaine"),
                CurrentUniversity = GetElementValue(node, "Etablissement d'affectation"),
                PreviousUniversity = GetElementValue(node, "Etablissement d'origine (FR)"),
                PreviousUniversityArabic = GetElementValue(node, "Etablissement d'origine (AR)"),
                RegistrationNumber = GetElementValue(node, "Numéro dossier d'inscription"),
            };
            model.individualDetails = new IndividualDetails()
            {
                ArmyService = GetElementValue(node, "Service national"),
                BloodType = GetElementValue(node, "Groupe Sanguin"),
                Civility = GetElementValue(node, "Civilité"),
                FamilySituation = GetElementValue(node, "Situation familiale"),
                Nationality = GetElementValue(node, "Nationalité"),
                Presumed = GetElementValue(node, "Présumé"),
                Quality = GetElementValue(node, "Qualité")
            };
            model.Parents = new StudentParents()
            {
                FatherFamilyName = GetElementValue(node, "Nom"),
                FatherFamilyNameArabic = GetElementValue(node, "Nom arabe"),
                FatherFirstName = GetElementValue(node, "Prénom du père"),
                FatherFirstNameArabic = GetElementValue(node, "Prénom du père arabe"),
                MotherFamilyName = GetElementValue(node, "Nom de la mère"),
                MotherFamilyNameArabic = GetElementValue(node, "Nom de la mère arabe"),
                MotherFirstName = GetElementValue(node, "Prènom de la mère"),
                MotherFirstNameArabic = GetElementValue(node, "Prènom de la mère arabe"),
            };
            model.AccessDetails = new AccessDetails()
            {
                AccessTitleType = GetElementValue(node, "Type titre d'accès"),
                Equivalence = GetElementValue(node, "Equivalence"),
                FirstForeignLanguage = GetElementValue(node, "Première langue étrangère"),
                Mention = GetElementValue(node, "Mention"),
                ObtainingInstitution = GetElementValue(node, "Etablissement d'obtention"),
                ObtainingYear = GetElementValue(node, "Année d'obtention"),
                Score = GetElementDecimal(node, "Moyenne"),
                SecondForeignLanguage = GetElementValue(node, "Deuxième langue étrangère"),
                Speciality = GetElementValue(node, "Spécialité"),
                TitleNumber = GetElementValue(node, "N° Titre")
            };
            model.Inscriptions = GetInscriptions(node);
            model.Addresses = GetAddresses(node);
            model.AnnualReviews = GetAnnualReviews(node);
            model.EmailAddresses = GetEmails(node);
            model.SportsSituations = GetSportsSituations(node);
            model.SpecialNeeds = GetSpecialNeeds(node);
            model.Transfers = GetTransfers(node);
            model.Grades = GetGrades(node);
            model.PhoneNumbers = GetPhones(node);

            return model;
        }
    }
    public class StudentDetailScrapperXml : IPageScrapper<StudentDetail>
    {
        private Worker _browser;
        public StudentDetailScrapperXml(Worker browser)
        {
            _browser = browser;
        }
        public IEnumerable<StudentDetail> ExtractData(string pageHtml)
        {

            throw new NotImplementedException();
        }

        public StudentDetail ExtractSingleData(string pageHtml, string id)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(pageHtml);


            var tables = htmlDoc
                .DocumentNode
                .Descendants()
                .Where(x => x.Id == "form_view")
                .Select(x =>
                {
                    x.InnerHtml = x.InnerHtml.Replace("<![CDATA[", "").Replace("]]>", "");
                    x.RemoveClass();
                    return x;
                })
                .SelectMany(x => x.DescendantsAndSelf())
                .Where(x =>
                {
                    return x.Name != "select" &&
                    x.Name != "checkbox" &&
                    x.Name != "button" &&
                    x.Name != "script" &&
                    x.Name != "style" &&
                    x.Name != "input" &&
                    x.Name != "#div";
                })
                .SelectMany(x => x.Descendants("table"));

            var detailTables = tables
               .Where(x => x.Descendants("tbody").Any() && x.Descendants("thead").Any())
               .Select(x => (x.Descendants("thead").First().GetAttributes("id").First().Value, x.Descendants("tbody")))
               .ToList();

            detailTables = Enumerable.DistinctBy(detailTables, x => x.Value)
                .ToList();

            var itemsTables = tables
                .Where(x => x.Descendants("tbody").Any() && !x.Descendants("thead").Any())
                .Select(x => x.Descendants("tbody"))
                .ToList();

            var result = StudentDetailsExtractor.Extract(htmlDoc.DocumentNode);
            result.Picture = DownloadStudentPicture(htmlDoc.DocumentNode, _browser);
            return result;
        }
        static byte[] DownloadStudentPicture(HtmlNode node, Worker browser)
        {
            try
            {
                var imageUrl = node
                    .Descendants("img")
                    .Where(x => x.Id.Contains("nomphotoView"))
                    .First()
                    .GetAttributes()
                    .Where(x => x.Name == "src")
                    .First()
                    .Value
                    .Replace(";", "&");

                var client = new HttpClient();

                var result = Akavache.BlobCache.LocalMachine.DownloadUrl($"https://progres.mesrs.dz{imageUrl}").GetAwaiter().GetResult();
                if(result.Length == 0)
                {
                    browser.Page.StartDownload($"https://progres.mesrs.dz{imageUrl}");
                }

                return result;
            }
            catch
            {
                return null;
            }
        }
    }
}
