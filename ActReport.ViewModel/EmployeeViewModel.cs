using ActReport.Core.Contracts;
using ActReport.Core.Entities;
using ActReport.Persistence;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ActReport.ViewModel
{
    public class EmployeeViewModel : BaseViewModel
    {
        private string _firstName;
        private string _lastName;
        private Employee _selectedEmployee;
        private ObservableCollection<Employee> _employees;
        private ICommand _cmdSaveChanges;
        private ICommand _cmdNewEmployee;

        public string FirstName
        {
            get => _firstName;
            set
            {
                _firstName = value;
                OnPropertyChanged(nameof(FirstName));
            }
        }

        public string LastName
        {
            get => _lastName;
            set
            {
                _lastName = value;
                OnPropertyChanged(nameof(LastName));
            }
        }
        public Employee SelectedEmployee
        {
            get => _selectedEmployee;
            set
            {
                _selectedEmployee = value;
                FirstName = _selectedEmployee?.FirstName;
                LastName = _selectedEmployee?.LastName;
                OnPropertyChanged(nameof(SelectedEmployee));
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
        public ICommand CmdSaveChanges
        {
            get
            {
                if (_cmdSaveChanges == null)
                {
                    _cmdSaveChanges = new RelayCommand(
                        execute: _ =>
                        {
                            using UnitOfWork uow = new UnitOfWork();
                            _selectedEmployee.FirstName = _firstName;
                            _selectedEmployee.LastName = _lastName;
                            uow.EmployeeRepository.Update(_selectedEmployee);
                            uow.Save();

                            LoadEmployees();
                        },
                        canExecute: _ => _selectedEmployee != null && _lastName.Length >= 3);
                }

                return _cmdSaveChanges;
            }
            set
            {
                _cmdSaveChanges = value;
            }
        }
        public ICommand CmdNewEmployee
        {
            get
            {
                if (_cmdNewEmployee == null)
                {
                    _cmdNewEmployee = new RelayCommand(
                        execute: _ =>
                        {
                            using UnitOfWork uow = new UnitOfWork();
                            Employee emp = new Employee
                            {
                                FirstName = _firstName,
                                LastName = _lastName
                            };
                          /*  if (FirstName != null && LastName != null)
                            {*/
                                uow.EmployeeRepository.Insert(emp);
                                uow.Save();
                                LoadEmployees();
                           // }
                            
                        },
                        canExecute: _ => _firstName?.Length >= 3 && _lastName?.Length >= 3);
                }
                return _cmdNewEmployee;
            }
            set
            {
                _cmdNewEmployee = value;
            }
        }

        public EmployeeViewModel()
        {
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            using (UnitOfWork uow = new UnitOfWork())
            {
                var employees = uow.EmployeeRepository
                    .Get(
                        orderBy:
                            coll => coll.OrderBy(employees => employees.LastName),
                           filter: 
                             filterByLastName => filterByLastName.LastName.StartsWith(FilterText))
                    .ToList();

                Employees = new ObservableCollection<Employee>(employees);
            }
        }
        private string _filterText = "";
        public string FilterText
        {
            get
            {
                return _filterText;
            }

            set
            {
                _filterText = value;
                LoadEmployees();
                
            }
        }
    }
}