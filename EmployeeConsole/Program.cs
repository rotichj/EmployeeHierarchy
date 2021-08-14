using EmployeeHandlerLibrary.EmployeeService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EmployeeConsole
{
    class Program
    {

        static void Main(string[] args)
        {
            QueryEmployees();
        }

        static void QueryEmployees()
        {
            var csvFile = "D://employees.csv";
            var employee = new Employees(csvFile);
            var salary = employee.GetSalaryByEmployeeNumber("Employee1");
            Console.WriteLine(salary);
        }
    }
}
