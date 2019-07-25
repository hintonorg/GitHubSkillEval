using Octokit;
using System;
using System.Threading.Tasks;

namespace GitHubSkillEvalConsoleApp
{
    class Program
    {
        private static GitHubClient client;
        private static string MyOrganization;

        static void Main(string[] args)
        {
            string  GetHubIdentity;
            Credentials credentials;

            GetHubIdentity = "GitHubSkillEval";
            MyOrganization = "hintonorg";
            
            client = new GitHubClient(new ProductHeaderValue(GetHubIdentity));

            //credentials = new Credentials("c50f8727eb4fe6c0ca2612a1a8c9a6f7fd7a08c0");  //My personal access token
            credentials = new Credentials("dwaynehinton", "Mygithubpwd1");      //Login name and password
            client.Credentials = credentials;

            Task.Run(async () => { await GetRepos(); }).Wait();
        }

        private static async Task GetRepos()
        {
            string LicenseBody;
            int iCnt = 0;

            //Get a MIT License
            var mitlicense = await client.Miscellaneous.GetLicense("mit");

            try
            {
                //Get all the repositories in the Organization.
                var Repositories = await client.Repository.GetAllForOrg(MyOrganization);
                foreach (var repo in Repositories)
                {
                    if (repo.License == null)
                    {
                        LicenseBody = mitlicense.Body;

                        //Change the [year], in the license body, to the current year. 
                        LicenseBody = LicenseBody.Replace("[year]", DateTime.Now.Year.ToString());

                        //Change the [fullname], in the license body, to the name of the repo.
                        LicenseBody = LicenseBody.Replace("[fullname]", repo.Name);

                        //Create a commit
                        var createChangeSet = await client.Repository.Content.CreateFile(
                                MyOrganization,
                                repo.Name,
                                "LICENSE",
                                new CreateFileRequest("File creation",
                                                      LicenseBody,
                                                      "master"));

                        Console.WriteLine(repo.Name + ": MIT License added");
                    }
                    else
                    {
                        Console.WriteLine(repo.Name + ": " + repo.License.Name);
                    }

                    iCnt++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Problems..." + e.Message); 
            }
        }
    }
}
