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
        // Для работы с БД MS SQL Server необходимо добавить пакет:
        // Microsoft.EntityFrameworkCore.SqlServer(представляет функциональность Entity Framework для работы с MS SQL Server)

        // Lazy loading или ленивая загрузка предполагает неявную автоматическую загрузку связанных данных при обращении к навигационному свойству.
        // Microsoft.EntityFrameworkCore.Proxies

        /*
         Migrations предоставляет упорядоченный набор шагов, которые описывают процесс обновления схемы базы данных. 
         В каждом из этих шагов (так называемая миграция) содержится определенный код, который описывает применяемые изменения.
         
         Для использования миграций в Visual Studio необходимо добавить в проект через менеджер Nuget пакет Microsoft.EntityFrameworkCore.Tools.
        
        Команда Add-Migration проверяет изменения с момента последней миграции и формирует шаблон для новой миграции с любыми обнаруженными изменениями. 
        Можно дать миграциям имя. Например, createDB. Add-Migration createDB
        Команда Update-Database применяет все ожидающие миграции в базе данных. 

        После выполнения миграций в проект будет добавлена папка Migrations с классом миграции:
        Папка содержит три файла:
            XXXXXXXXXXXXXX_createDB.cs: основной файл миграции, который содержит все применяемые действия.
            [Имя_контекста_данных]ModelSnapshot.cs: содержит текущее состояние модели, используется при создании следующей миграции.

        Кроме основных таблиц база данных также будет содержать дополнительную таблицу _EFMigrationsHystory, 
        которая будет хранить информацию о миграциях.
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
                    MessageBox.Show("Не задано название группы!");
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

                    MessageBox.Show("Группа добавлена!");
                 
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

                    MessageBox.Show("Группа удалена!");
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
                    MessageBox.Show("Не задано название группы!");
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

                    MessageBox.Show("Группа переименована!");
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
                    MessageBox.Show("Не указано имя или фамилия студента!");
                    return;
                }
                if (comboBoxGroup.Items.Count == 0)
                {
                    MessageBox.Show("Не создано ни одной группы!");
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

                    MessageBox.Show("Студент добавлен!");
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

                    MessageBox.Show("Студент удалён!");
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
                    MessageBox.Show("Не указано имя или фамилия студента!");
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

                    MessageBox.Show("Данные о студенте изменены!");
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
