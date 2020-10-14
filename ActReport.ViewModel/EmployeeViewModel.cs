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
        private string _firstname;
        private string _lastname;
        private Employee _selectEmployee;
        private ObservableCollection<Employee> _employees;
        private ICommand _cmdSaveChanges;
        private ICommand _cmdNewEmployee;

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
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            using (IUnitOfWork uow = new UnitOfWork())
            {
                var employees = uow.EmployeeRepository
                    .Get(
                        orderBy:
                            coll => coll.OrderBy(employees => employees.LastName))
                    .ToList();

                Employees = new ObservableCollection<Employee>(employees);
            }
        }
        public ICommand CmdSaveChanges
        {
            get
            {
                if(_cmdSaveChanges == null)
                {
                    _cmdSaveChanges = new RelayCommand(
                        execute: _ =>
                        {
                            using IUnitOfWork uow = new UnitOfWork();
                            _selectEmployee.FirstName = _firstname;
                            _selectEmployee.LastName = _lastname;
                            uow.EmployeeRepository.Update(_selectEmployee);
                            uow.Save();

                            LoadEmployees();
                        },
                        canExecute: _ => _selectEmployee != null);
                        
                }
                return _cmdSaveChanges;
            }
            set { _cmdSaveChanges = value; }
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
                            using IUnitOfWork uow = new UnitOfWork();
                            Employee emp = new Employee
                            {
                                FirstName = _firstname,
                                LastName = _lastname
                            };
                            uow.EmployeeRepository.Insert(emp);
                            uow.Save();

                            LoadEmployees();
                        },
                        canExecute: _ => _firstname != null);

                }
                return _cmdNewEmployee;
            }
            set { _cmdNewEmployee = value; }
        }
    }
}