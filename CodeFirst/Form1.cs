using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using StudentLibrary;
using AcademyGroupContextLib;

namespace CodeFirst
{
    public partial class Form1 : Form
    {
        // Для роботи з БД MS SQL Server необхідно додати пакет:
        // Microsoft.EntityFrameworkCore.SqlServer (представляє функціональність Entity Framework для роботи з MS SQL Server)

        // Lazy loading або ліниве завантаження передбачає неявне автоматичне завантаження пов'язаних даних при зверненні до навігаційної властивості.
        // Microsoft.EntityFrameworkCore.Proxies

        /*
         Migrations надає впорядкований набір кроків, які описують процес оновлення схеми бази даних. 
         У кожному з цих кроків (так звана міграція) міститься певний код, який описує зміни, що застосовуються.
         
         Для використання міграцій у Visual Studio необхідно додати в проєкт через менеджер Nuget пакет Microsoft.EntityFrameworkCore.Tools.
         Tools -> NuGet Package Manager -> Package Manager Console
         Команда Add-Migration перевіряє зміни з моменту останньої міграції та формує шаблон для нової міграції з будь-якими виявленими змінами. 
         Можна дати міграціям ім'я. Наприклад, createDB. Add-Migration createDB
         Команда Update-Database застосовує всі очікувані міграції в базі даних. 

         Після виконання міграцій до проєкту буде додано папку Migrations із класом міграції:
         Папка містить три файли:
             XXXXXXXXXXXXXX_createDB.cs: основний файл міграції, який містить усі дії, що застосовуються.
             [Ім'я_контексту_даних]ModelSnapshot.cs: містить поточний стан моделі, використовується при створенні наступної міграції.

         Окрім основних таблиць, база даних також міститиме додаткову таблицю _EFMigrationsHistory, 
         яка зберігатиме інформацію про міграції.
         */

        public Form1()
        {
            InitializeComponent();
            try
            {
                using (var db = new AcademyGroupContext())
                {
                    var query = from b in db.AcademyGroups
                                select b;
                    comboBoxGroup.DataSource = query.ToList();
                    comboBoxGroup.DisplayMember = "Name";

                    var query2 = from b in db.Students
                                 select b;
                    comboBoxStudent.DataSource = query2.ToList();
                    comboBoxStudent.DisplayMember = "LastName";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AddGroupClick(object sender, EventArgs e)
        {
            try
            {
                string groupname = textBoxGroup.Text.Trim();
                if (groupname == "")
                {
                    MessageBox.Show("Не задано назву групи!");
                    return;
                }
                using (var db = new AcademyGroupContext())
                {
                    var academygroup = new AcademyGroup { Name = groupname };
                    db.AcademyGroups.Add(academygroup);
                    db.SaveChanges();
                    textBoxGroup.Text = "";

                    var query = from b in db.AcademyGroups
                                select b;
                    comboBoxGroup.DataSource = query.ToList();
                    comboBoxGroup.DisplayMember = "Name";

                    MessageBox.Show("Групу додано!");

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RemoveGroupClick(object sender, EventArgs e)
        {
            if (comboBoxGroup.Items.Count == 0)
                return;
            try
            {
                using (var db = new AcademyGroupContext())
                {
                    List<AcademyGroup> list = comboBoxGroup.DataSource as List<AcademyGroup>;
                    string academygroup = list[comboBoxGroup.SelectedIndex].Name;
                    var query = from b in db.AcademyGroups
                                where b.Name == academygroup
                                select b;
                    db.AcademyGroups.RemoveRange(query);
                    db.SaveChanges();

                    query = from b in db.AcademyGroups
                            select b;
                    comboBoxGroup.DataSource = query.ToList();
                    comboBoxGroup.DisplayMember = "Name";

                    var query2 = from b in db.Students
                                 select b;
                    comboBoxStudent.DataSource = query2.ToList();
                    comboBoxStudent.DisplayMember = "LastName";

                    if (comboBoxStudent.Items.Count == 0)
                    {
                        textBoxFirstName.Text = "";
                        textBoxLastName.Text = "";
                        textBoxAverage.Text = "";
                        textBoxAge.Text = "";
                        textBoxGr.Text = "";
                    }

                    MessageBox.Show("Групу видалено!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void EditGroupClick(object sender, EventArgs e)
        {
            try
            {
                string groupname = textBoxGroup.Text.Trim();
                if (groupname == "")
                {
                    MessageBox.Show("Не задано назву групи!");
                    return;
                }
                using (var db = new AcademyGroupContext())
                {
                    List<AcademyGroup> list = comboBoxGroup.DataSource as List<AcademyGroup>;
                    string academygroup = list[comboBoxGroup.SelectedIndex].Name;
                    var query = (from b in db.AcademyGroups
                                 where b.Name == academygroup
                                 select b).Single();
                    query.Name = groupname;
                    db.SaveChanges();
                    textBoxGroup.Text = "";

                    var query2 = from b in db.AcademyGroups
                                 select b;
                    comboBoxGroup.DataSource = query2.ToList();
                    comboBoxGroup.DisplayMember = "Name";

                    MessageBox.Show("Групу перейменовано!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AddStudentClick(object sender, EventArgs e)
        {
            try
            {
                string firstname = textBoxFirstName.Text.Trim();
                string lastname = textBoxLastName.Text.Trim();
                if (firstname == "" || lastname == "")
                {
                    MessageBox.Show("Не вказано ім'я або прізвище студента!");
                    return;
                }
                if (comboBoxGroup.Items.Count == 0)
                {
                    MessageBox.Show("Не створено жодної групи!");
                    return;
                }
                double? average = null;
                if (textBoxAverage.Text != "")
                    average = Convert.ToDouble(textBoxAverage.Text);

                int? age = null;
                if (textBoxAge.Text != "")
                    age = Convert.ToInt32(textBoxAge.Text);

                using (var db = new AcademyGroupContext())
                {
                    List<AcademyGroup> list = comboBoxGroup.DataSource as List<AcademyGroup>;
                    string academygroup = list[comboBoxGroup.SelectedIndex].Name;
                    var query = (from b in db.AcademyGroups
                                 where b.Name == academygroup
                                 select b).Single();

                    var student = new Student
                    {
                        FirstName = firstname,
                        LastName = lastname,
                        Age = age,
                        PointAverage = average,
                        AcademyGroup = query
                    };
                    db.Students.Add(student);
                    db.SaveChanges();

                    var query2 = from b in db.Students
                                 select b;
                    comboBoxStudent.DataSource = query2.ToList();
                    comboBoxStudent.DisplayMember = "LastName";

                    MessageBox.Show("Студента додано!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RemoveStudentClick(object sender, EventArgs e)
        {
            if (comboBoxStudent.Items.Count == 0)
                return;
            try
            {
                using (var db = new AcademyGroupContext())
                {
                    List<Student> list = comboBoxStudent.DataSource as List<Student>;
                    string student = list[comboBoxStudent.SelectedIndex].LastName;
                    var query = from b in db.Students
                                where b.LastName == student
                                select b;
                    db.Students.RemoveRange(query);
                    db.SaveChanges();

                    var query2 = from b in db.Students
                                 select b;
                    comboBoxStudent.DataSource = query2.ToList();
                    comboBoxStudent.DisplayMember = "LastName";

                    if (comboBoxStudent.Items.Count == 0)
                    {
                        textBoxFirstName.Text = "";
                        textBoxLastName.Text = "";
                        textBoxAverage.Text = "";
                        textBoxAge.Text = "";
                        textBoxGr.Text = "";
                    }

                    MessageBox.Show("Студента видалено!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void EditStudentClick(object sender, EventArgs e)
        {
            try
            {
                string firstname = textBoxFirstName.Text.Trim();
                string lastname = textBoxLastName.Text.Trim();
                if (firstname == "" || lastname == "")
                {
                    MessageBox.Show("Не вказано ім'я або прізвище студента!");
                    return;
                }

                double? average = null;
                if (textBoxAverage.Text != "")
                    average = Convert.ToDouble(textBoxAverage.Text);

                int? age = null;
                if (textBoxAge.Text != "")
                    age = Convert.ToInt32(textBoxAge.Text);

                using (var db = new AcademyGroupContext())
                {
                    List<AcademyGroup> list = comboBoxGroup.DataSource as List<AcademyGroup>;
                    string academygroup = list[comboBoxGroup.SelectedIndex].Name;
                    var query = (from b in db.AcademyGroups
                                 where b.Name == academygroup
                                 select b).Single();
                    if (query == null)
                        return;

                    List<Student> studentlist = comboBoxStudent.DataSource as List<Student>;
                    string student = studentlist[comboBoxStudent.SelectedIndex].LastName;
                    var query2 = (from b in db.Students
                                  where b.LastName == student
                                  select b).Single();

                    query2.AcademyGroup = query;
                    query2.FirstName = firstname;
                    query2.LastName = lastname;
                    query2.Age = age;
                    query2.PointAverage = average;
                    db.SaveChanges();

                    var query3 = from b in db.Students
                                 select b;
                    comboBoxStudent.DataSource = query3.ToList();
                    comboBoxStudent.DisplayMember = "LastName";

                    MessageBox.Show("Дані про студента змінено!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void comboBoxStudent_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxStudent.Items.Count == 0)
                return;
            try
            {
                using (var db = new AcademyGroupContext())
                {
                    List<Student> studentlist = comboBoxStudent.DataSource as List<Student>;
                    if (studentlist == null)
                        return;
                    string student = studentlist[comboBoxStudent.SelectedIndex].LastName;
                    var query = (from b in db.Students
                                 where b.LastName == student
                                 select b).Single();

                    textBoxFirstName.Text = query.FirstName;
                    textBoxLastName.Text = query.LastName;
                    textBoxAverage.Text = query.PointAverage.ToString();
                    textBoxAge.Text = query.Age.ToString();
                    textBoxGr.Text = query.AcademyGroup.Name;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void comboBoxGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxGroup.Items.Count == 0)
                return;
            try
            {
                using (var db = new AcademyGroupContext())
                {
                    List<AcademyGroup> list = comboBoxGroup.DataSource as List<AcademyGroup>;
                    string academygroup = list[comboBoxGroup.SelectedIndex].Name;
                    var query = (from b in db.AcademyGroups
                                 where b.Name == academygroup
                                 select b).Single();
                    foreach (var s in query.Students)
                        MessageBox.Show(s.LastName);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

    }
}