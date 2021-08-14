using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using EmployeeHandlerLibrary.Model;

namespace EmployeeHandlerLibrary.EmployeeService
{
    public class Employees
    {
        private string _csvFile;
        List<EmployeeModel> employees= new List<EmployeeModel>();

        public Employees(string csvFile)
        {
            _csvFile = csvFile;
            Console.WriteLine(ParseFile());
           // employees = ReadEmployeesFromCSV();
        }


        public string GetSalaryByEmployeeNumber(string managerId)
        {

            var employeesUnderThisManager = employees.Where(x => x.ManagerId == managerId.Trim());//.Sum(x=>Convert.ToInt64(x.Salary));
            var managerSalary= employees.Where(x => x.EmployeeId == managerId.Trim()).FirstOrDefault().Salary;

            if (!employeesUnderThisManager.Any())
            {
                return "No Employees Under This manager";
            }
            var totalBudget = Convert.ToInt64(employeesUnderThisManager.Sum(x => Convert.ToInt64(x.Salary))) + Convert.ToInt64(managerSalary); //Int64 for long

            return totalBudget.ToString();
        }

        string ParseFile()
        {
            var isValidDir = ValidateDirectory();
            if (isValidDir == false)
            {
                return "Invalid directory";
            }
            var isValudExt = ValidateFileExtension();
            if (isValudExt == false)
            {
                return "Unsupported format";
            }
            employees = ReadEmployeesFromCSV();

            var isThereDuplicates = FindDuplicateEmployee(employees);
            if (isValudExt == false)
            {
                return "Some employee(s) are/is reporting to one manager";
            }
            var isThereOneEmployeeWithNoManager = OneEmployeeWithNoManager(employees); //Check CEO
            if (isValudExt == false)
            {
                return "More than one employee(s) are/is doesnt have managers, only ceo should not";
            }
            var levels = UpdateLevels(employees);
            var validateLevels = ValidateLevels(employees);
            return "Success";
        }

        bool ValidateFileExtension()
        {
            var extension = Path.GetExtension(_csvFile);
            if(extension.Equals(".csv",StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }
        bool UpdateLevels(List<EmployeeModel> employees)
        {

            
            var newEmployeeList = employees.Where(c => c.ManagerId == "").Select(c => { c.Level = "1"; return c; }).ToList(); //reporting to ceo Level 2
            var ceoId = newEmployeeList.Where(x => x.ManagerId == string.Empty).FirstOrDefault().ManagerId;

            int i = 2;
            while (newEmployeeList.Where(a => a.ManagerId == "").Count()>1) // since ceo
            {
                
                foreach (var item in newEmployeeList.Where(c => c.Level == (i - 1).ToString()).ToList())
                {
                    newEmployeeList = newEmployeeList.Where(c => c.ManagerId == item.EmployeeId).Select(c => { c.Level = i.ToString(); return c; }).ToList();
                } 
                i++;
            }



            return employees
                    .GroupBy(s => s.EmployeeId)
                    .Where(g => g.Count() > 1).Any();

        }
        private bool ValidateLevels(List<EmployeeModel> employees)
        {
            return employees
                    .GroupBy(s => s.EmployeeId)
                    .Where(g => g.Count() > 1).Any();

        }

        private bool FindDuplicateEmployee(List<EmployeeModel> employees)
        {
            return employees
                    .GroupBy(s => s.EmployeeId)
                    .Where(g => g.Count() > 1).Any();
           
        }

        private bool ValidateSalaries(List<EmployeeModel> employees)
        {
            return employees
                    .GroupBy(s => s.EmployeeId)
                    .Where(g => g.Count() > 1).Any();

        }
        private bool OneEmployeeWithNoManager(List<EmployeeModel> employees) // See if one employee does not report to anybody
        {
            return employees.Where(x=>x.ManagerId==string.Empty).Count()==1;

        }
        private bool ValidateDirectory()
        {

            string result;
            return TryGetFullPath(_csvFile, out result);
        }
        private bool TryGetFullPath(string path, out string result)
        {
            result = String.Empty;
            if (String.IsNullOrWhiteSpace(path)) { return false; }
            bool status = false;

            try
            {
                result = Path.GetFullPath(path);                
                status = true;
            }
            catch (ArgumentException) { }
            catch (NotSupportedException) { }
            catch (PathTooLongException) { }

            return status;
        }

        private List<EmployeeModel> ReadEmployeesFromCSV()
        {
            // PaySlips\combinedpayslip
            var employees = new List<EmployeeModel>();
            try
            {

                using (var reader = new StreamReader(_csvFile))
                {
                    int i = 0;
                    var linee = reader.ReadToEnd();

                    string[] lines = linee.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

                    foreach (var item in lines)
                    {
                        if (i > 0)
                        {
                            var values = item.Split(new char[] { ',' });
                            if (values.Length == 3)
                            {
                                var employee = new EmployeeModel()
                                {
                                    EmployeeId = values[0].Trim(),
                                    ManagerId = values[1].Trim(),
                                    Salary = values[2].Trim(),
                                };
                                employees.Add(employee);
                            }
                        }
                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error While reading the File!\n" + ex.Message);
                return employees;
            }

            return employees;
        }
    }
}
