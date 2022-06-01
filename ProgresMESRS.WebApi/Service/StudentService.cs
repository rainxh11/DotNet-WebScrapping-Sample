using ProgresMESRS.Middleware.API.Models;
using LiteDB.Async;
using ProgresMESRS.Middleware.API;

namespace ProgresMESRS.WebApi.Service
{
    public interface IStudentService
    {
        Task<StudentDetail> ExtractStudent(string matricule, string nin = "", bool useCacheFirst = true);

        Task<IEnumerable<StudentDetail>> ExtractStudents(string search = "", bool useCacheFirst = true);
    }

    public class StudentService : IStudentService
    {
        private readonly IConfiguration _config;

        private ILiteDatabaseAsync _db;
        private Worker _browser;
        private StudentsListScrapperXml _scrapperXml;
        private StudentDetailScrapperXml _detailScrapperXml;
        private ApiClient _progresApiClient;
        public StudentService(ILiteDatabaseAsync db,
            IConfiguration Configuration,
            Worker worker,
            StudentsListScrapperXml scrapperXml,
            StudentDetailScrapperXml detailScrapperXml,
            ApiClient apiClient
            )
        {
            _db = db;
            _browser = worker;
            _scrapperXml = scrapperXml;
            _detailScrapperXml = detailScrapperXml;
            _progresApiClient = apiClient;
            _config = Configuration;
        }
        public async Task<string> FirstPage()
        {
            await _browser.OpenUrl("https://progres.mesrs.dz/webfve/login.xhtml");
            await _browser.SignIn(_config["ProgresUsername"], _config["ProgresPassword"]);

            await _browser.OpenUrl("https://progres.mesrs.dz/webfve/pages/cursus/dossier/DossiersEtudiants.xhtml");
            var cookies = await _browser.GetCookies("https://progres.mesrs.dz/webfve/pages/cursus/dossier/DossiersEtudiants.xhtml");

            //await _browser.PressNextAndGetData(true);
            return cookies;
        }
        public async Task<StudentDetail> ExtractStudent(string matricule, string nin = "", bool useCacheFirst = true)
        {
            StudentDetail student = null;
            if (useCacheFirst)
            {
                student = await _db.GetCollection<StudentDetail>().FindOneAsync(x => x.Matricule == matricule);
                if(!string.IsNullOrEmpty(nin))
                    student = await _db.GetCollection<StudentDetail>().FindOneAsync(x => x.Matricule == matricule && x.NIN == nin);
            }
            if(student != null)
                return student;
            else
            {
                await _browser.ClearCookies();
                var cookies = await FirstPage();
                await _browser.SearchForStudent(matricule, nin);
                var javaViewState = _browser.ExtractJavaViewStates();

                var pageSource = await _progresApiClient.GetStudentSearchList(javaViewState.First(), 5, cookies, matricule, nin);
                var students = _scrapperXml.ExtractData(pageSource);
                var keys = Worker.ExtractRowKeysSearchList(pageSource);
                var viewSate = Worker.ExtractJavaViewStatesFromXml(pageSource);

                var pageResponse = await _progresApiClient.GetStudentDetails(javaViewState.First(), keys.First().Value, cookies);
                var details = _detailScrapperXml.ExtractSingleData(pageResponse, "");

                return details;
            }
        }

        public Task<IEnumerable<StudentDetail>> ExtractStudents(string search = "", bool useCacheFirst = true)
        {
            throw new NotImplementedException();
        }
    }
}
