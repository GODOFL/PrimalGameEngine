﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PrimalEditor.Utilities{
    public interface IUndoRedo { 
        string Name { get; }
        void Undo();
        void Redo();
    }

    public class UndoRedoAction : IUndoRedo {
        private Action _undoAction;
        private Action _redoAction;
        public string Name { get; set; }

        public void Redo() => _redoAction(); 

        public void Undo() => _undoAction();

        public UndoRedoAction(string name) { 
            Name = name;
        }

        public UndoRedoAction(Action undo, Action redo, string name) 
            : this(name)
        {
            Debug.Assert(undo != null && redo != null);
            _undoAction = undo;
            _redoAction = redo;
        }

        public UndoRedoAction(string property, object instance, object undoValue, object redoValue, string name) 
            : this(
                    () => instance.GetType().GetProperty(property).SetValue(instance, undoValue),
                    () => instance.GetType().GetProperty(property).SetValue(instance, redoValue),
                    name
                  )    
        { 

        }
    }

/*
 * **************************************************************************
 * ********************                                  ********************
 * ********************      COPYRIGHT INFORMATION       ********************
 * ********************                                  ********************
 * **************************************************************************
 *                                                                          *
 *                                   _oo8oo_                                *
 *                                  o8888888o                               *
 *                                  88" . "88                               *
 *                                  (| -_- |)                               *
 *                                  0\  =  /0                               *
 *                                ___/'==='\___                             *
 *                              .' \\|     |// '.                           *
 *                             / \\|||  :  |||// \                          *
 *                            / _||||| -:- |||||_ \                         *
 *                           |   | \\\  -  /// |   |                        *
 *                           | \_|  ''\---/''  |_/ |                        *
 *                           \  .-\__  '-'  __/-.  /                        *
 *                         ___'. .'  /--.--\  '. .'___                      *
 *                      ."" '<  '.___\_<|>_/___.'  >' "".                   *
 *                     | | :  `- \`.:`\ _ /`:.`/ -`  : | |                  *
 *                     \  \ `-.   \_ __\ /__ _/   .-` /  /                  *
 *                 =====`-.____`.___ \_____/ ___.`____.-`=====              *
 *                                   `=---=`                                *
 * **************************************************************************
 * ********************                                  ********************
 * ********************      				             ********************
 * ********************         佛祖保佑 永远无BUG       ********************
 * ********************                                  ********************
 * **************************************************************************
 */

    public class UndoRedo{
        private bool _enableAdd = true;
        private readonly ObservableCollection<IUndoRedo> _redoList = new ObservableCollection<IUndoRedo>();
        private readonly ObservableCollection<IUndoRedo> _undoList = new ObservableCollection<IUndoRedo>();

        //<ItemsControl ItemsSource = "{Binding RedoList}" >         RedoList未绑定?????????    public也不行???????    为什么?????
        public ReadOnlyObservableCollection<IUndoRedo> RedoList { get; }

        //<ItemsControl ItemsSource = "{Binding UndoList}" >         UndoList未绑定?????????    public也不行???????    为什么?????
        public ReadOnlyObservableCollection<IUndoRedo> UndoList { get; }

        public void Reset() { 
            _redoList.Clear();
            _undoList.Clear();
        }

        public void Add(IUndoRedo cmd) {
            if (_enableAdd) { 
                _undoList.Add(cmd);
                _redoList.Clear();
            }
        }

        public void Undo() {
            if (_undoList.Any()) { 
                var cmd = _undoList.Last();
                _undoList.RemoveAt(_undoList.Count - 1);
                _enableAdd = false;
                cmd.Undo();
                _enableAdd = true;
                _redoList.Insert(0, cmd);
            }
        }

        public void Redo() {
            if (_redoList.Any()) { 
                var cmd = _redoList.First();
                _redoList.RemoveAt(0);
                _enableAdd = false;
                cmd.Redo();
                _enableAdd = true;
                _undoList.Add(cmd);
            }
        }

        public UndoRedo() {
            RedoList = new ReadOnlyObservableCollection<IUndoRedo>(_redoList);
            UndoList = new ReadOnlyObservableCollection<IUndoRedo>(_undoList);
        }
    }
}
