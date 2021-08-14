using EmployeeHandlerLibrary.EmployeeService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void QueryEmployees()
        {
            var csvFile = "D://employees.csv";
            var employee = new Employees(csvFile);
            var salary = employee.GetSalaryByEmployeeNumber("Employee1");
            Console.WriteLine(salary);

        }
    }
}
