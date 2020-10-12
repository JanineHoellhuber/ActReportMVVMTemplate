using ActReport.Core.Entities;
using ActReport.Persistence;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ActReport.ViewModel
{
    public class EmployeeViewModel : BaseViewModel
    {
        private string _firstname;
        private string _lastname;
        private Employee _selectEmployee;
        private ObservableCollection<Employee> _employees;

        public string FirstName
        {
            get => _firstname;
            set
            {
                _firstname = value;
                OnPropertyChanged(nameof(FirstName));
            }
        }
        public string LastName
        {
            get => _lastname;
            set
            {
                _lastname = value;
                OnPropertyChanged(nameof(LastName));
            }
        }

        public Employee SelectEmployee
        {
            get => _selectEmployee;
            set
            {
                _selectEmployee = value;
                FirstName = _selectEmployee?.FirstName;
                LastName = _selectEmployee?.LastName;
                OnPropertyChanged(nameof(SelectEmployee));
            }
        }
        public ObservableCollection<Employee> Employees
        {
            get => _employees;
            set
            {
                _employees = value;
                OnPropertyChanged(nameof(Employees));
            }
        }
        public EmployeeViewModel()
        {
            LoadEmployee();
        }

        private void LoadEmployee()
        {
            using (UnitOfWork uow = new UnitOfWork())
            {
                var employees = uow.EmployeeRepository
                    .Get(
                        orderBy:
                            coll => coll.OrderBy(employees => employees.LastName))
                    .ToList();

                Employees = new ObservableCollection<Employee>(employees);
            }
        }
    }
}