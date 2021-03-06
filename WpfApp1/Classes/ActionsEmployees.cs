﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfApp1
{
    public class ActionsEmployees
    {
        private Employee Employee { get; set; }

        public ActionsEmployees() { }

        public ActionsEmployees(int idUser)
        {
            SearchEmployee(idUser);
        }

        public ActionsEmployees(Employee employee)
        {
            Employee = employee;
        }

        //Поиск сотрудника по id пользователя
        private void SearchEmployee(int idUser)
        {
            using var db = new CafeEntities();
            var employee = db.Users.Where(userId => userId.ID == idUser).FirstOrDefault();

            Employee = db.Employees.Include(empStatus => empStatus.Status_employees)
                                   .Include(empPost => empPost.Posts_employees)
                                   .Include(cont => cont.Contracts)
                                   .Include(shiftList => shiftList.Shift_list)
                                   .Include(table => table.Tables)
                                   .Include(user => user.Users)
                                   .FirstOrDefault(emp => emp.ID == employee.Fk_employee);
        }

        //Проверка на количество должностей у сотрудника и получение название должности
        public string CountPostAndTheirNames()
        {
            string SelectedPost;
            using var db = new CafeEntities();

            if (Employee.Posts_employees.Count > 1)
            {
                ChoicePostWindow choicePost = new ChoicePostWindow(GettingIDEmployee());
                choicePost.ShowDialog();

                SelectedPost = choicePost.GetPost;
            }
            else
                SelectedPost = Employee.Posts_employees.ToArray()[0].Post.Name;

            return SelectedPost;
        }

        #region Вывод информации о сотруднике
        //Вывод всех сотрудников
        public Employee[] GettingAllEmployees()
        {
            using var db = new CafeEntities();
            var employees = db.Employees.Include(status => status.Status_employees).ToArray();

            return employees;
        }

        //Вывод только официантов
        public List<Employee> GettingAllEmployeesWaiter()
        {
            using var db = new CafeEntities();
            var employees = db.Employees.Include(postEmp => postEmp.Posts_employees).ToArray();

            List<Employee> emp = new List<Employee>();

            foreach (var waiter in employees)
                for (int i = 0; i < waiter.Posts_employees.Count; i++)
                    if (waiter.Posts_employees.ToArray()[i].Post.Name == "Официант")
                        emp.Add(waiter);

            return emp;
        }

        // Вывод ID сотрудника
        public int GettingIDEmployee() => Employee.ID;

        //Вывод сокращенного ФИО сотрудника
        public string GettingLFMEmployee() => Employee.MName == "Не указано"
                                                             ? Employee.LName + " " + Employee.FName.Substring(0, 1) + "."
                                                             : Employee.LName + " " + Employee.FName.Substring(0, 1) + ". "
                                                             + Employee.FName.Substring(0, 1) + ".";

        //Вывод ФИО сотрудника полностью
        public string GettingFullLFMEmployee() => Employee.MName == "Не указано"
                                                                 ? Employee.LName + " " + Employee.FName
                                                                 : Employee.LName + " " + Employee.FName
                                                                 + Employee.FName;

        //Вывод номера телефона сотрудника
        public string GettingPhoneNumberEmployee() => Employee.Phone_number;

        //Вывод статуса сотрудника
        public string GettingStatusName() => Employee.Status_employees.Name;

        //Вывод фотографии сотрудника
        public ImageSource GettingPhoto()
        {
            ImageSource image;

            try
            {
                image = new BitmapImage(new Uri(Employee.Photo));
                return image;
            }
            catch
            {
                MessageBox.Show("Ошибка! Фотография отсутствует!", "Фотография не обнаружена", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        #endregion

        //Заполнение ComboBox`а "Статус сотрудников"
        public List<Status_employees> FillingComboBoxStatus_employees()
        {
            using var db = new CafeEntities();
            var status = db.Status_employees.ToList();

            return status;
        }

        //Добавление сотрудника
        public void AddEmployee(Dictionary<string, string> emp)
        {
            using var db = new CafeEntities();

            Employee employee = new Employee()
            {
                LName = emp["lName"],
                FName = emp["fName"],
                MName = emp["mName"],
                Fk_status_employee = int.Parse(emp["fkStatus"]),
                Photo = emp["photo"],
                Phone_number = emp["phoneNumber"]
            };

            db.Employees.Add(employee);
            db.SaveChanges();
        }


        #region Изменение информации о сотруднике
        //Изменение фотографии сотрудника
        public void ChangePhoto(out ImageSource image)
        {
            using var db = new CafeEntities();
            var changePhoto = db.Employees.Where(emp => emp.ID == Employee.ID).FirstOrDefault();

            Microsoft.Win32.OpenFileDialog file = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Картинки(*.JPG; *.PNG)| *.JPG; *.PNG",
                CheckFileExists = true,
                Title = "Выберете изображение"
            };

            image = null;

            if (file.ShowDialog() == true)
            {
                image = new BitmapImage(new Uri(file.FileName));
                changePhoto.Photo = file.FileName;

                db.SaveChanges();
            }
        }

        //Изменение статуса сотрудника
        public void EditStatus(int status)
        {
            using var db = new CafeEntities();
            int id = GettingIDEmployee();
            var employee = db.Employees.Where(emp => emp.ID == id).FirstOrDefault();

            employee.Fk_status_employee = status;

            db.SaveChanges();
        }
        #endregion
    }
}
