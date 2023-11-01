﻿using PrimalEditor.Common;
using PrimalEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace PrimalEditor.GameProject {
    [DataContract]
    public class ProjectData {
        [DataMember]
        public string ProjectName { get; set; }
        [DataMember]
        public string ProjectPath { set; get; }
        [DataMember]
        public DateTime Date { get; set; }
        public string FullPath { get => $@"{ProjectPath}{ProjectName}\{ProjectName}{Project.Extension}"; }
        public byte[] Icon { get; set; }
        public byte[] Screenshot { set; get; }
    }

    [DataContract]
    public class ProjectDataList {
        [DataMember]
        public List<ProjectData> Projects { get; set; }
    }

    //打开项目待改
    class OpenProject {
        private static readonly string _applicationDataPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\PrimalEditor\";
        private static readonly string _projectDataPath;
        private static readonly ObservableCollection<ProjectData> _projects = new ObservableCollection<ProjectData>();
        public static ReadOnlyObservableCollection<ProjectData> Projects { get; }

        //读取项目数据文件，从foreach直接到_projects.Add()中间的if判断过程都直接跳过
        private static void ReadProjectData() {
            if (File.Exists(_projectDataPath)) {
                var projects = Serializer.FromFile<ProjectDataList>(_projectDataPath).Projects.OrderByDescending(x => x.Date);
                _projects.Clear();
                foreach(var project in projects) {
                    if (File.Exists(project.FullPath)) {
                        project.Icon = File.ReadAllBytes($@"{project.ProjectPath}{project.ProjectName}\.Primal\Icon.png");
                        project.Screenshot = File.ReadAllBytes($@"{project.ProjectPath}{project.ProjectName}\.Primal\Screenshot.png");
                        _projects.Add(project);
                    }
                }
            }
        }

        private static void WriteProjectData() {
            var projects = _projects.OrderBy(x => x.Date).ToList();
            Serializer.ToFile(new ProjectDataList() { Projects = projects }, _projectDataPath);
        }

        public static Project Open(ProjectData data) { 
            ReadProjectData();
            var project = _projects.FirstOrDefault(x => x.FullPath == data.FullPath);
            if (project != null) {
                project.Date = DateTime.Now;
            }
            else {
                project = data;
                project.Date = DateTime.Now;
                _projects.Add(project);
            }
            WriteProjectData();

            return Project.Load(project.FullPath);
        }

        static OpenProject() {
            try{ 
                if(!Directory.Exists(_applicationDataPath)) Directory.CreateDirectory(_applicationDataPath);
                _projectDataPath = $@"{_applicationDataPath}ProjectData.xml";
                Projects = new ReadOnlyObservableCollection<ProjectData>(_projects);
                ReadProjectData();
            }
            catch(Exception ex) {
                Debug.WriteLine(ex.Message);

                //TODO Error
                Logger.Log(MessageType.Error, $"Failed to read project data");

                throw;
            }
        }
    }
}
