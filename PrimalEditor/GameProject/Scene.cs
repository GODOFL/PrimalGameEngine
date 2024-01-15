﻿using PrimalEditor.Common;
using PrimalEditor.Components;
using PrimalEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PrimalEditor.GameProject {
    [DataContract]
    class Scene : ViewModelBase {
        private string _name;
        [DataMember]
        public string Name {
            get => _name;
            set {
                if (_name != value) { 
                    _name = value;
                    OnProperChanged(nameof(Name));
                }
            }
        }
        [DataMember]
        public Project Project { get; private set; }

        //待改default scene能被删除？
        //之前忘记将数据传入客户端
        private bool _isActive;
        [DataMember]
        public bool IsActive {
            get => _isActive;
            set {
                if (_isActive != value) { 
                    _isActive = value;
                    OnProperChanged(nameof(IsActive));
                }
            }
        }

        [DataMember(Name = nameof(GameEntities))]
        private readonly ObservableCollection<GameEntity> _gameEntities = new ObservableCollection<GameEntity>();
        public ReadOnlyObservableCollection<GameEntity> GameEntities { get; private set; }

        public ICommand AddGameEntityCommand { get; private set; }
        public ICommand RemoveGameEntityCommand { get; private set; }

        private void AddGameEntity(GameEntity entity, int index = -1) {
            Debug.Assert(!_gameEntities.Contains(entity));
            entity.IsActive = IsActive;
            if (index == -1) {
                _gameEntities.Add(entity);
            }
            else { 
                _gameEntities.Insert(index, entity);
            }

            //AddGameEntity中多了此行导致添加多了一个游戏实体
            //_gameEntities.Add(entity);
        }

        private void RemoveGameEntity(GameEntity entity) {
            Debug.Assert(!_gameEntities.Contains(entity));
            entity.IsActive = false;
            _gameEntities.Remove(entity);
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) {
            if (_gameEntities != null) {
                GameEntities = new ReadOnlyObservableCollection<GameEntity>(_gameEntities);
                OnProperChanged(nameof(GameEntities));
            }
            foreach (var entity in _gameEntities) {
                entity.IsActive = IsActive;
            }

            AddGameEntityCommand = new RelayCommand<GameEntity>(x => {
                AddGameEntity(x);
                var entityIndex = _gameEntities.Count - 1;

                Project.UndoRedo.Add(new UndoRedoAction(
                    () => RemoveGameEntity(x),
                    () => AddGameEntity(x, entityIndex),
                    $"Add{x.Name} to {Name}"
                    ));
            });

            RemoveGameEntityCommand = new RelayCommand<GameEntity>(x => {
                var entityIndex = _gameEntities.IndexOf(x);
                RemoveGameEntity(x);

                Project.UndoRedo.Add(new UndoRedoAction(
                    () => AddGameEntity(x, entityIndex),
                    () => RemoveGameEntity(x),
                    $"Remove {x.Name}"
                    ));
            });
        }

        public Scene(Project project, string name) {
            Debug.Assert(project != null);
            Project = project;
            Name = name;
            OnDeserialized(new StreamingContext());
        }
    }
}
