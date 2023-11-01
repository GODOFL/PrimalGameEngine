﻿using PrimalEditor.Common;
using PrimalEditor.GameProject;
using PrimalEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PrimalEditor.Components {
    //游戏引擎实体类
    [DataContract]
    [KnownType(typeof(Transform))]
    class GameEntity : ViewModelBase {
        private bool _isEnabled = true;
        [DataMember]
        public bool IsEnabled {
            get => _isEnabled;
            set {
                if (_isEnabled != value) { 
                    _isEnabled = value;
                    OnProperChanged(nameof(IsEnabled));
                }
            }
        }

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
        public Scene ParentScene { get; private set; }

        [DataMember(Name = nameof(Components))]
        private readonly ObservableCollection<Component> _components = new ObservableCollection<Component> ();
        public ReadOnlyObservableCollection<Component> Components { get; private set; }

        //public ICommand RenameCommand { set; private get; }
        //public ICommand IsEnabledCommand { set; private get; }

        //显示游戏实体属性
        [OnDeserialized]
        void OnDeserialized(StreamingContext context) {
            if (_components != null) {
                Components = new ReadOnlyObservableCollection<Component>(_components);
                OnProperChanged(nameof(Components));
            }

            //RenameCommand = new RelayCommand<string>(x => {
            //    var oldName = _name;
            //    Name = x;

            //    Project.UndoRedo.Add(new UndoRedoAction(nameof(Name), this, oldName, x, $"Rename entity '{oldName}' to '{x}'"));
            //}, x => x != _name);

            //IsEnabledCommand = new RelayCommand<bool>(x => {
            //    var oldValue = _isEnabled;
            //    IsEnabled = x;

            //    Project.UndoRedo.Add(new UndoRedoAction(nameof(IsEnabled), this, oldValue, x, x ? $"Enable {Name}" : $"Disable {Name}"));
            //});
        }

        public GameEntity(Scene scene) {
            Debug.Assert(scene != null);
            ParentScene = scene;
            _components.Add(new Transform(this));
            OnDeserialized(new StreamingContext());
        }
    }

    abstract class MSEntity : ViewModelBase {
        //Enables updates to selected entities
        private bool _enableUpdates = true;
        private bool? _isEnabled;
        public bool? IsEnabled { 
            get => _isEnabled;
            set {
                if (_isEnabled != value) {
                    _isEnabled = value;
                    OnProperChanged(nameof(IsEnabled));
                }
            }
        }

        private string? _name;
        public string? Name { 
            get => _name;
            set {
                if (_name != value) { 
                    _name = value;
                    OnProperChanged(nameof(Name));
                }
            }
        }

        private readonly ObservableCollection<IMSComponent> _components = new ObservableCollection<IMSComponent>();
        public ReadOnlyObservableCollection<IMSComponent> Components { get; }

        public List<GameEntity> SelectedEntities { get; }

        public static float? GetMixedValue(List<GameEntity> entities, Func<GameEntity, float> getProperty) {
            var value = getProperty(entities.First());
            foreach (var entity in entities.Skip(1)) {
                if (!value.IsTheSameAs(getProperty(entity))) {
                    return null;
                }
            }
            return value;
        }

        public static bool? GetMixedValue(List<GameEntity> entities, Func<GameEntity, bool> getProperty) {
            var value = getProperty(entities.First());
            foreach (var entity in entities.Skip(1)) {
                if (value != getProperty(entity)) {
                    return null;
                }
            }
            return value;
        }

        public static string? GetMixedValue(List<GameEntity> entities, Func<GameEntity, string> getProperty) {
            var value = getProperty(entities.First());
            foreach (var entity in entities.Skip(1)) {
                if (value != getProperty(entity)) {
                    return null;
                }
            }
            return value;
        }

        protected virtual bool UpdateGameEntities(string propertyName) {
            switch (propertyName) {
                case nameof(IsEnabled): SelectedEntities.ForEach(x => x.IsEnabled = IsEnabled.Value); return true;
                case nameof(Name): SelectedEntities.ForEach(x => x.Name = Name); return true;
            }
            return false;
        }

        protected virtual bool UpdateMSGameEntity() {
            IsEnabled = GetMixedValue(SelectedEntities, new Func<GameEntity, bool>(x => x.IsEnabled));
            Name = GetMixedValue(SelectedEntities, new Func<GameEntity, string>(x => x.Name));

            return true;
        }

        public void Refresh() {
            _enableUpdates = false;
            UpdateMSGameEntity();
            _enableUpdates = false;
        }

        public MSEntity(List<GameEntity> entities) {
            Debug.Assert(entities?.Any() == true);
            Components = new ReadOnlyObservableCollection<IMSComponent>(_components);
            SelectedEntities = entities;
            PropertyChanged += (s, e) => { if(_enableUpdates) UpdateGameEntities(e.PropertyName); };
        }
    }

    class MSGameEntity : MSEntity {
        public MSGameEntity(List<GameEntity> entities) : base(entities) { 
            Refresh();
        }
    }
}



//GameEntity是NULL进行序列化时会出错，可能要有实体后再去设置添加GameEntity