﻿using PrimalEditor.Components;
using PrimalEditor.GameProject;
using PrimalEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PrimalEditor.Editors {
    /// <summary>
    /// GameEntityView.xaml 的交互逻辑
    /// </summary>
    public partial class GameEntityView : UserControl {
        private Action _undoAction;
        private string _propertyName;
        public static GameEntityView Instance { get; private set; }

        public GameEntityView() {
            InitializeComponent();
            DataContext = null;
            Instance = this;

            //666,模板？感觉不像
            DataContextChanged += (_, __) => {
                if (DataContext != null) {
                    (DataContext as MSEntity).PropertyChanged += (s, e) => _propertyName = e.PropertyName;
                }
            };
        }

        private Action GetRenameAction() {
            var vm = DataContext as MSEntity;
            var selection = vm.SelectedEntities.Select(entity => (entity, entity.Name)).ToList();
            return new Action(() => {
                selection.ForEach(item => item.entity.Name = item.Name);
                (DataContext as MSEntity).Refresh();
            });
        }

        private Action GetIsEnabledAction() {
            var vm = DataContext as MSEntity;
            var selection = vm.SelectedEntities.Select(entity => (entity, entity.IsEnabled)).ToList();
            return new Action(() => {
                selection.ForEach(item => item.entity.IsEnabled = item.IsEnabled);
                (DataContext as MSEntity).Refresh();
            });
        }

        private void OnName_TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
            //var vm = DataContext as MSEntity;
            //var selection = vm.SelectedEntities.Select(entity => (entity, entity.Name)).ToList();
            //_undoAction = new Action(() => {
            //    selection.ForEach(item => item.entity.Name = item.Name);
            //    (DataContext as MSEntity).Refresh();
            //});

            _undoAction = GetRenameAction();
        }

        private void OnName_TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
            if (_propertyName == nameof(MSEntity.Name) && _undoAction != null) {
                //var vm = DataContext as MSEntity;
                //var selection = vm.SelectedEntities.Select(entity => (entity, entity.Name)).ToList();
                //var redoAction = new Action(() => {
                //    selection.ForEach(item => item.entity.Name = item.Name);
                //});

                var redoAction = GetRenameAction();
                Project.UndoRedo.Add(new UndoRedoAction(_undoAction, redoAction, "Rename game entity"));
                _propertyName = null;
            }
            _undoAction = null;
        }

        private void OnIsEnabled_CheckBox_Click(object sender, RoutedEventArgs e) {
            var undoAction = GetIsEnabledAction();
            var vm = DataContext as MSEntity;
            vm.IsEnabled = (sender as CheckBox).IsChecked == true;
            var redoAction = GetIsEnabledAction();
            Project.UndoRedo.Add(new UndoRedoAction(undoAction, redoAction, 
                vm.IsEnabled == true ? "Enable game entity" : "Disable game entity"));
        }
    }
}
