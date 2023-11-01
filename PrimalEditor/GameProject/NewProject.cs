using PrimalEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PrimalEditor.GameProject {
    [DataContract]
    public class ProjectTemplate {
        [DataMember]
        public string ProjectType { get; set; }
        [DataMember]
        public string ProjectFile { get; set; }
        [DataMember]
        public List<string> Folders { get; set; }

        public byte[] Icon { get; set; }
        public byte[] Screenshot { get; set; }
        public string IconFilePath { get; set; }
        public string ScreenshotFilePath { get; set; }
        public string ProjectFilePath { get; set; }

    }

    class NewProject : Common.ViewModelBase {
        //TODO: Get the path from the installation location
        private readonly string _templatePath = @"..\..\PrimalEditor\ProjectTemplates";

        private string _projectName = "NewProject";
        public string ProjectName {
            get => _projectName;
            set {
                if (_projectName != value) {
                    _projectName = value;
                    ValidDateProjectPath();
                    OnProperChanged(nameof(ProjectName));
                }
            }
        }

        //获取窗口输入的目标路径
        private string _projectPath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\PrimalProjects\";
        public string ProjectPath {
            get => _projectPath;
            set {
                if (_projectPath != value) {
                    _projectPath = value;
                    ValidDateProjectPath();
                    OnProperChanged(nameof(ProjectPath));
                }
            }
        }

        private bool _isValid;
        public bool IsVaild {
            get => _isValid;
            set {
                if (_isValid != value) {
                    _isValid = value;
                    OnProperChanged(nameof(IsVaild));
                }
            }
        }

        private string _errorMsg;     
        public string ErrorMsg {
            get => _errorMsg;
            set {
                if (_errorMsg != value) {
                    _errorMsg = value;
                    OnProperChanged(nameof(ErrorMsg));
                }
            }
        }

        //ProjectTemplate的动态数据收集
        private ObservableCollection<ProjectTemplate> _projectTemplate = new ObservableCollection<ProjectTemplate>();
        public ReadOnlyCollection<ProjectTemplate> ProjectTemplates { get; private set; }

        //检查路径有效值
        private bool ValidDateProjectPath() {                
            var path = ProjectPath;
            if (!Path.EndsInDirectorySeparator(path)) path += @"\";
            path += $@"{ProjectName}\";

            IsVaild = false;
            if (string.IsNullOrWhiteSpace(ProjectName.Trim())) {
                ErrorMsg = "Type is a project name.";
            }
            else if (ProjectName.LastIndexOfAny(Path.GetInvalidFileNameChars()) != -1) {
                ErrorMsg = "Invaild character(s) used in project name.";
            }
            else if (string.IsNullOrWhiteSpace(ProjectPath.Trim())) {
                ErrorMsg = "Select a valid project folder";
            }
            else if (ProjectPath.IndexOfAny(Path.GetInvalidPathChars()) != -1) {
                ErrorMsg = "Invaild character(s) used in project name.";
            }
            else if (Directory.Exists(path) && Directory.EnumerateFileSystemEntries(path).Any()) {
                ErrorMsg = "Select project folder already exists and is not empty";
            }
            else {
                ErrorMsg = string.Empty;
                IsVaild = true;
            }

            return IsVaild;
        }

        //创建新项目文件夹
        public string CreateProject(ProjectTemplate template) {
            ValidDateProjectPath();
            if (!IsVaild) { 
                return string .Empty;
            }
            if (!Path.EndsInDirectorySeparator(ProjectPath)) ProjectPath += @"\";
            var path = $@"{ProjectPath}{ProjectName}\";

            //检查路径是否存在，不存在则创建
            try {
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                foreach (var folder in template.Folders) {
                    Directory.CreateDirectory(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path), folder)));
                }
                var dirInfo = new DirectoryInfo(path + @".Primal\");
                dirInfo.Attributes |= FileAttributes.Hidden;                                                                         //隐藏项目原始文件夹
                File.Copy(template.IconFilePath, Path.GetFullPath(Path.Combine(dirInfo.FullName, "Icon.png")));
                File.Copy(template.ScreenshotFilePath, Path.GetFullPath(Path.Combine(dirInfo.FullName, "Screenshot.png")));

                var projectXml = File.ReadAllText(template.ProjectFilePath);
                projectXml = string.Format(projectXml, ProjectName, ProjectPath);
                var projectPath = Path.GetFullPath(Path.Combine(path, $"{ProjectName}{Project.Extension}"));
                File.WriteAllText(projectPath, projectXml);

                return path;
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.Message);

                //TODO log error
                Logger.Log(MessageType.Error, $"Failed to create {ProjectName}");

                throw;
            }
        }

        //新建项目模块内容展示
        public NewProject() {
            ProjectTemplates = new ReadOnlyObservableCollection<ProjectTemplate>(_projectTemplate);
            try {
                var templateFiles = Directory.GetFiles(_templatePath, "template.xml", SearchOption.AllDirectories);
                Debug.Assert(templateFiles.Any());

                //展示项目模板图片
                foreach (var file in templateFiles) {
                    var template = Serializer.FromFile<ProjectTemplate>(file);
                    template.IconFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "Icon.png"));
                    template.Icon = File.ReadAllBytes(template.IconFilePath);
                    template.ScreenshotFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "Screenshot.png"));
                    template.Screenshot = File.ReadAllBytes(template.ScreenshotFilePath);
                    template.ProjectFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), template.ProjectFile));

                    _projectTemplate.Add(template);
                }
                ValidDateProjectPath();
            }
            catch(Exception ex) {
                Debug.WriteLine(ex.Message);

                //TODO: log error
                Logger.Log(MessageType.Error, $"Failed to read project templates");

                throw;
            }
        }
    }

}
